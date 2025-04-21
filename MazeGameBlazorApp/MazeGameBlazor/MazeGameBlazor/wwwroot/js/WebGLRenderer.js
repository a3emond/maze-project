// Optimized WebGLRenderer.js with Debug Statements, Alpha Fix, and Defensive Checks

window.itemData = [];
window.itemTextureCache = {};
window.playerTextureCache = {};
window.tileTextures = {}; // Global texture cache

let sharedQuadBuffer;
let needsRedraw = true;

window.cameraX = 0;
window.cameraY = 0;
window.tileSize = 16;
window.zoomLevel = 3;

window.initWebGL = function (tileDataInput, width, height) {
    console.log("Initializing WebGL...");

    const canvas = document.getElementById("mazeCanvas");
    if (!canvas) return console.error("Canvas element not found!");

    const gl = canvas.getContext("webgl");
    if (!gl) return console.error("WebGL not supported!");

    window.gl = gl;
    window.canvas = canvas;
    window.tileData = tileDataInput;
    window.mazeWidth = width;
    window.mazeHeight = height;

    canvas.width = 800;
    canvas.height = 600;

    console.log("Canvas and GL initialized", { width, height });

    gl.viewport(0, 0, canvas.width, canvas.height);
    gl.clearColor(0.0, 0.0, 0.0, 1.0);
    gl.clear(gl.COLOR_BUFFER_BIT);

    setupShaders(gl);
    setupBuffers(gl);

    gl.enable(gl.BLEND);
    gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);

    loadTexturesWebGL(tileDataInput).then(() => {
        console.log("All textures loaded. Starting render.");
        requestRedraw();
        window.renderIfNeeded();
    });
};

console.log("initWebGL function is ready:", typeof window.initWebGL);

function setupShaders(gl) {
    console.log("Setting up shaders...");

    const vertexShaderSource = `
        attribute vec2 a_position;
        attribute vec2 a_texCoord;
        uniform vec2 u_resolution;
        uniform vec2 u_offset;
        varying vec2 v_texCoord;

        void main() {
            vec2 pos = a_position + u_offset;
            vec2 zeroToOne = pos / u_resolution;
            vec2 zeroToTwo = zeroToOne * 2.0;
            vec2 clipSpace = zeroToTwo - 1.0;

            gl_Position = vec4(clipSpace * vec2(1, -1), 0, 1);
            v_texCoord = a_texCoord;
        }
    `;

    const fragmentShaderSource = `
        precision mediump float;
        varying vec2 v_texCoord;
        uniform sampler2D u_image;

        void main() {
            gl_FragColor = texture2D(u_image, v_texCoord);
        }
    `;

    const vertexShader = createShader(gl, gl.VERTEX_SHADER, vertexShaderSource);
    const fragmentShader = createShader(gl, gl.FRAGMENT_SHADER, fragmentShaderSource);
    const program = createProgram(gl, vertexShader, fragmentShader);

    gl.useProgram(program);
    gl.program = program;

    gl.a_position = gl.getAttribLocation(program, "a_position");
    gl.a_texCoord = gl.getAttribLocation(program, "a_texCoord");
    gl.u_resolution = gl.getUniformLocation(program, "u_resolution");
    gl.u_image = gl.getUniformLocation(program, "u_image");
    gl.u_offset = gl.getUniformLocation(program, "u_offset");

    gl.enableVertexAttribArray(gl.a_position);
    gl.enableVertexAttribArray(gl.a_texCoord);

    console.log("Shader setup complete", {
        a_position: gl.a_position,
        a_texCoord: gl.a_texCoord,
        u_resolution: gl.u_resolution,
        u_image: gl.u_image,
        u_offset: gl.u_offset
    });
}

function createShader(gl, type, source) {
    const shader = gl.createShader(type);
    gl.shaderSource(shader, source);
    gl.compileShader(shader);
    if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
        console.error("Shader compile failed:", gl.getShaderInfoLog(shader));
        gl.deleteShader(shader);
        return null;
    }
    return shader;
}

function createProgram(gl, vShader, fShader) {
    const program = gl.createProgram();
    gl.attachShader(program, vShader);
    gl.attachShader(program, fShader);
    gl.linkProgram(program);
    if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
        console.error("Program link failed:", gl.getProgramInfoLog(program));
        return null;
    }
    return program;
}

function setupBuffers(gl) {
    console.log("Setting up shared quad buffer...");
    sharedQuadBuffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, sharedQuadBuffer);

    const size = window.tileSize * window.zoomLevel;
    const quadData = new Float32Array([
        0, 0, 0, 0,
        size, 0, 1, 0,
        0, size, 0, 1,
        0, size, 0, 1,
        size, 0, 1, 0,
        size, size, 1, 1
    ]);


    gl.bufferData(gl.ARRAY_BUFFER, quadData, gl.STATIC_DRAW);
    console.log("Buffer initialized");
}


function requestRedraw() {
    needsRedraw = true;
}

window.renderIfNeeded = function () {
    if (needsRedraw) {
        console.log("Calling renderWebGL...");
        renderWebGL();
        needsRedraw = false;
    }
};

function loadTexturesWebGL(tileData) {
    return new Promise((resolve) => {
        const gl = window.gl;
        let loadedCount = 0;
        const uniqueTextures = [...new Set(tileData.filter(url => url))];
        const total = uniqueTextures.length;


        if (total === 0) return resolve();

        uniqueTextures.forEach(url => {
            if (window.tileTextures[url]) {
                loadedCount++;
                if (loadedCount === total) resolve();
                return;
            }

            const image = new Image();
            image.crossOrigin = "anonymous";

            image.onload = () => {
                const texture = gl.createTexture();
                gl.bindTexture(gl.TEXTURE_2D, texture);
                gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);

                window.tileTextures[url] = texture;

                loadedCount++;
                if (loadedCount === total) resolve();
            };

            image.onerror = () => {
                console.error("Failed to load texture:", url);
                loadedCount++;
                if (loadedCount === total) resolve();
            };

            image.src = url;
        });
    });
}

function drawTile(gl, texture, tileX, tileY, tileSize) {
    const screenX = (tileX * tileSize) - window.cameraX;
    const screenY = (tileY * tileSize) - window.cameraY;

    if (screenX + tileSize < 0 || screenX > gl.canvas.width ||
        screenY + tileSize < 0 || screenY > gl.canvas.height) return;


    gl.bindBuffer(gl.ARRAY_BUFFER, sharedQuadBuffer);
    gl.vertexAttribPointer(gl.a_position, 2, gl.FLOAT, false, 16, 0);
    gl.vertexAttribPointer(gl.a_texCoord, 2, gl.FLOAT, false, 16, 8);

    gl.uniform2f(gl.u_resolution, gl.canvas.width, gl.canvas.height);
    gl.uniform2f(gl.u_offset, screenX, screenY);
    gl.activeTexture(gl.TEXTURE0);
    gl.bindTexture(gl.TEXTURE_2D, texture);
    gl.uniform1i(gl.u_image, 0);

    gl.drawArrays(gl.TRIANGLES, 0, 6);
}

function renderWebGL() {
    const gl = window.gl;
    const tileSize = window.tileSize * window.zoomLevel;
    const tilesAcross = Math.ceil(gl.canvas.width / tileSize) + 2;
    const tilesDown = Math.ceil(gl.canvas.height / tileSize) + 2;
    const startX = Math.floor(window.cameraX / tileSize);
    const startY = Math.floor(window.cameraY / tileSize);


    gl.viewport(0, 0, gl.canvas.width, gl.canvas.height);
    gl.clear(gl.COLOR_BUFFER_BIT);

    for (let y = startY; y < startY + tilesDown; y++) {
        for (let x = startX; x < startX + tilesAcross; x++) {
            if (x < 0 || y < 0 || x >= window.mazeWidth || y >= window.mazeHeight) continue;
            const index = y * window.mazeWidth + x;
            const textureKey = window.tileData[index];
            if (!textureKey || !window.tileTextures[textureKey]) {
                console.warn("Missing texture:", textureKey, "at", x, y);
                continue;
            }
            drawTile(gl, window.tileTextures[textureKey], x, y, tileSize);
        }
    }

    for (const item of window.itemData) {
        const texture = window.itemTextureCache[item.sprite];
        if (texture) drawTile(gl, texture, item.x, item.y, tileSize);
    }

    if (window.player.texture) {
        drawTile(gl, window.player.texture, window.player.x, window.player.y, tileSize);
    }

}


//------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------PLAYER SUPPORT------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------------------------------------

    window.player = {
        x: 0,
        y: 0,
        sprite: null,
        texture: null // WebGL texture
    };

    window.playerTextureCache = {}; // Cache sprite textures by URL

    window.spawnPlayerWebGL = function(gridX, gridY, sprite) {
        window.player.x = gridX;
        window.player.y = gridY;
        window.player.sprite = sprite;

        const cached = window.playerTextureCache[sprite];
        if (cached) {
            window.player.texture = cached;
            centerCameraOnPlayer(gridX, gridY);
            renderWebGL();
            return;
        }

        const img = new Image();
        img.crossOrigin = "anonymous";
        img.onload = () => {
            const texture = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, texture);
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, img);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);

            window.player.texture = texture;
            window.playerTextureCache[sprite] = texture;

            gl.enable(gl.BLEND);
            gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);

            centerCameraOnPlayer(gridX, gridY);
            renderWebGL();
        };
        img.src = sprite;
    };

    window.updatePlayerPositionWebGL = function(x, y, sprite) {
        if (window.player.sprite !== sprite) {
            // Re-invoke spawn with new sprite (will use cache if available)
            window.spawnPlayerWebGL(x, y, sprite);
            return;
        }

        window.player.x = x;
        window.player.y = y;

        centerCameraOnPlayer(x, y);
        renderWebGL();
    };

    function centerCameraOnPlayer(x, y) {
        const canvas = window.canvas;
        window.cameraX = (x * window.tileSize * window.zoomLevel) - (canvas.width / 2);
        window.cameraY = (y * window.tileSize * window.zoomLevel) - (canvas.height / 2);
        enforceCameraBoundsWebGL();
    }

    function enforceCameraBoundsWebGL() {
        const mazePixelWidth = window.mazeWidth * window.tileSize * window.zoomLevel;
        const mazePixelHeight = window.mazeHeight * window.tileSize * window.zoomLevel;

        const maxCameraX = Math.max(0, mazePixelWidth - window.canvas.width);
        const maxCameraY = Math.max(0, mazePixelHeight - window.canvas.height);

        window.cameraX = Math.max(0, Math.min(window.cameraX, maxCameraX));
        window.cameraY = Math.max(0, Math.min(window.cameraY, maxCameraY));
    }

//------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------ITEM SUPPORT--------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------------------------------------
    window.updateItemDataWebGL = function(items) {
        window.itemData = items;

        const gl = window.gl;
        const neededTextures = [...new Set(items.map(i => i.sprite))];
        let loaded = 0;

        neededTextures.forEach(sprite => {
            if (window.itemTextureCache[sprite]) {
                loaded++;
                return;
            }

            const img = new Image();
            img.crossOrigin = "anonymous";
            img.onload = () => {
                const texture = gl.createTexture();
                gl.bindTexture(gl.TEXTURE_2D, texture);
                gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, img);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
                window.itemTextureCache[sprite] = texture;

                loaded++;
                if (loaded === neededTextures.length) {
                    renderWebGL(); // Trigger draw when all loaded
                }
            };
            img.src = sprite;
        });

        if (neededTextures.length === 0) {
            renderWebGL();
        }
    };

