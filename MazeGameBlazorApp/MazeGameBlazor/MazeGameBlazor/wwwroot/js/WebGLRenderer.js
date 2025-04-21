// WebGLRenderer.js
/*
    Optional later upgrades:

    Add shadows, animations, or minimap via WebGL.

    Use gl.LINEAR filtering for smoother look on scale-up.

    Use a tile atlas texture (sprite sheet) for performance.
 */

window.itemData = [];
window.itemTextureCache = {};
//log to console
console.log("WebGLRenderer.js loaded.");

window.initWebGL = function (tileDataInput, width, height) {
    console.log("Initializing WebGL...");

    const canvas = document.getElementById("mazeCanvas");
    if (!canvas) {
        console.error("Canvas element not found!");
        return;
    }

    const gl = canvas.getContext("webgl");
    if (!gl) {
        console.error("WebGL not supported!");
        return;
    }

    window.gl = gl;
    window.canvas = canvas;
    window.tileData = tileDataInput;
    window.mazeWidth = width;
    window.mazeHeight = height;

    canvas.width = 800;
    canvas.height = 600;

    // Store rendering settings
    gl.viewport(0, 0, canvas.width, canvas.height);
    gl.clearColor(0.0, 0.0, 0.0, 1.0);
    gl.clear(gl.COLOR_BUFFER_BIT);

    setupShaders(gl);
    setupBuffers(gl);
    loadTexturesWebGL(tileDataInput).then(() => {
        renderWebGL(); // First draw
    });
};

//-----------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------SHADER SETUP-------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------

function setupShaders(gl) {
    const vertexShaderSource = `
        attribute vec2 a_position;
        attribute vec2 a_texCoord;
        uniform vec2 u_resolution;
        varying vec2 v_texCoord;

        void main() {
            vec2 zeroToOne = a_position / u_resolution;
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

//-----------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------BUFFER SETUP-------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------

function setupBuffers(gl) {
    // Create a buffer for positions
    const positionBuffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
    gl.enableVertexAttribArray(gl.a_position);
    gl.vertexAttribPointer(gl.a_position, 2, gl.FLOAT, false, 16, 0); // 2 floats = 8 bytes

    // Create a buffer for texture coords
    const texCoordBuffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, texCoordBuffer);
    gl.enableVertexAttribArray(gl.a_texCoord);
    gl.vertexAttribPointer(gl.a_texCoord, 2, gl.FLOAT, false, 16, 8);
}


//------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------TEXTURE LOADING-----------------------------------------------------------------
//------------------------------------------------------------------------------------------------------------------------------------

function loadTexturesWebGL(tileData) {
    return new Promise((resolve) => {
        const gl = window.gl;
        const tileTextures = {};
        let loadedCount = 0;

        const uniqueTextures = [...new Set(tileData.filter(url => url))];

        uniqueTextures.forEach(url => {
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

                tileTextures[url] = texture;
                loadedCount++;

                if (loadedCount === uniqueTextures.length) {
                    window.tileTextures = tileTextures;
                    resolve();
                }
            };

            image.onerror = () => {
                console.error(`Failed to load WebGL texture: ${url}`);
            };

            image.src = url;
        });
    });
}

//------------------------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------RENDERING-----------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------------------------------------
window.cameraX = window.cameraX || 0;
window.cameraY = window.cameraY || 0;
window.tileSize = window.tileSize || 16;
window.zoomLevel = window.zoomLevel || 3;

window.renderWebGL = function () {
    const gl = window.gl;
    const tileSize = window.tileSize * window.zoomLevel;

    const screenWidth = window.canvas.width;
    const screenHeight = window.canvas.height;

    gl.viewport(0, 0, screenWidth, screenHeight);
    gl.clear(gl.COLOR_BUFFER_BIT);
    gl.uniform2f(gl.u_resolution, screenWidth, screenHeight);

    const tilesAcross = Math.ceil(screenWidth / tileSize) + 2;
    const tilesDown = Math.ceil(screenHeight / tileSize) + 2;

    const startX = Math.floor(window.cameraX / tileSize);
    const startY = Math.floor(window.cameraY / tileSize);

    for (let y = startY; y < startY + tilesDown; y++) {
        for (let x = startX; x < startX + tilesAcross; x++) {
            if (x < 0 || y < 0 || x >= window.mazeWidth || y >= window.mazeHeight) continue;

            const index = y * window.mazeWidth + x;
            const textureKey = window.tileData[index];
            const texture = window.tileTextures[textureKey];
            if (!texture) continue;

            drawTile(gl, texture, x, y, tileSize);
        }
    }
    // Draw items
    const items = window.itemData;
    if (items && items.length > 0) {
        for (const item of items) {
            const texture = window.itemTextureCache[item.sprite];
            if (texture) {
                drawTile(gl, texture, item.x, item.y, tileSize);
            }
        }
    }

    // Draw the player
    if (window.player.texture) {
        drawTile(gl, window.player.texture, window.player.x, window.player.y, tileSize);
    }

};

function drawTile(gl, texture, tileX, tileY, tileSize) {
    const screenX = (tileX * tileSize) - window.cameraX;
    const screenY = (tileY * tileSize) - window.cameraY;

    const x1 = screenX;
    const y1 = screenY;
    const x2 = screenX + tileSize;
    const y2 = screenY + tileSize;

    const positions = new Float32Array([
        x1, y1, 0, 0,
        x2, y1, 1, 0,
        x1, y2, 0, 1,
        x1, y2, 0, 1,
        x2, y1, 1, 0,
        x2, y2, 1, 1
    ]);

    const buffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
    gl.bufferData(gl.ARRAY_BUFFER, positions, gl.STATIC_DRAW);

    gl.vertexAttribPointer(gl.a_position, 2, gl.FLOAT, false, 16, 0);
    gl.enableVertexAttribArray(gl.a_position);

    gl.vertexAttribPointer(gl.a_texCoord, 2, gl.FLOAT, false, 16, 8);
    gl.enableVertexAttribArray(gl.a_texCoord);

    gl.activeTexture(gl.TEXTURE0);
    gl.bindTexture(gl.TEXTURE_2D, texture);
    gl.uniform1i(gl.u_image, 0);

    gl.drawArrays(gl.TRIANGLES, 0, 6);
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

window.spawnPlayerWebGL = function (gridX, gridY, sprite) {
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

window.updatePlayerPositionWebGL = function (x, y, sprite) {
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
window.updateItemDataWebGL = function (items) {
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
