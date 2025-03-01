// Global WebGL context, shader program, textures, camera, and maze data
window.gl = null;
window.shaderProgram = null;
window.tileTextures = {};
window.cameraX = 0;
window.cameraY = 0;
window.zoomLevel = 1;
window.tileSize = 16;
window.tileData = [];  // ✅ Fix: Store `tileData` globally
window.mazeWidth = 0;
window.mazeHeight = 0;

// Initialize WebGL
window.initWebGL = function (tileDataInput, width, height) {
    console.log("Initializing WebGL...");
    const canvas = document.getElementById("mazeCanvas");

    if (!canvas) {
        console.error("❌ Error: Canvas element not found!");
        return;
    }

    window.gl = canvas.getContext("webgl");
    if (!window.gl) {
        console.error("❌ Error: WebGL is not supported!");
        return;
    }

    const gl = window.gl;
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    gl.viewport(0, 0, gl.drawingBufferWidth, gl.drawingBufferHeight);

    gl.clearColor(0.0, 0.0, 0.0, 1.0);
    gl.clear(gl.COLOR_BUFFER_BIT);

    console.log("✅ WebGL Initialized Successfully!");

    // ✅ Fix: Store tileData globally for future access
    window.tileData = tileDataInput;
    window.mazeWidth = width;
    window.mazeHeight = height;

    initShaders(gl);

    loadTextures(tileDataInput).then(textures => {
        console.log("✅ Textures Loaded:", textures);
        window.tileTextures = textures;

        setupInputListeners();  // Now `tileData` is available globally
        renderMaze(gl, textures);
    }).catch(error => {
        console.error("❌ Error loading textures:", error);
    });
};

// Initialize shaders
function initShaders(gl) {
    console.log("🔹 Compiling shaders...");
    const vertexShaderSource = `
        attribute vec2 aPosition;
        attribute vec2 aTexCoord;
        varying vec2 vTexCoord;
        void main() {
            gl_Position = vec4(aPosition, 0.0, 1.0);
            vTexCoord = aTexCoord;
        }
    `;

    const fragmentShaderSource = `
        precision mediump float;
        varying vec2 vTexCoord;
        uniform sampler2D uTexture;
        void main() {
            gl_FragColor = texture2D(uTexture, vTexCoord);
        }
    `;

    const vertexShader = compileShader(gl, gl.VERTEX_SHADER, vertexShaderSource);
    const fragmentShader = compileShader(gl, gl.FRAGMENT_SHADER, fragmentShaderSource);

    if (!vertexShader || !fragmentShader) {
        console.error("❌ Shader compilation failed!");
        return;
    }

    const shaderProgram = gl.createProgram();
    gl.attachShader(shaderProgram, vertexShader);
    gl.attachShader(shaderProgram, fragmentShader);
    gl.linkProgram(shaderProgram);

    if (!gl.getProgramParameter(shaderProgram, gl.LINK_STATUS)) {
        console.error("❌ Shader program failed to link:", gl.getProgramInfoLog(shaderProgram));
        return;
    }

    gl.useProgram(shaderProgram);
    window.shaderProgram = shaderProgram;

    console.log("✅ Shaders compiled and linked successfully!");
}

// Compile shader helper function
function compileShader(gl, type, source) {
    const shader = gl.createShader(type);
    gl.shaderSource(shader, source);
    gl.compileShader(shader);

    if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
        console.error(`❌ Shader compilation error (${type}):`, gl.getShaderInfoLog(shader));
        return null;
    }

    return shader;
}

// Load textures
function loadTextures(tileData) {
    return new Promise(resolve => {
        const gl = window.gl;
        const tileTextures = {};
        let loadedCount = 0;
        const uniqueTextures = [...new Set(tileData)];

        console.log("🔹 Loading textures:", uniqueTextures);

        function onLoad() {
            loadedCount++;
            if (loadedCount === uniqueTextures.length) {
                console.log("✅ All textures loaded successfully!");
                resolve(tileTextures);
            }
        }

        uniqueTextures.forEach(url => {
            let texture = gl.createTexture();
            let image = new Image();

            image.onload = function () {
                gl.bindTexture(gl.TEXTURE_2D, texture);
                gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
                gl.generateMipmap(gl.TEXTURE_2D);
                tileTextures[url] = texture;
                onLoad();
            };

            image.onerror = function () {
                console.error(`❌ Failed to load texture: ${url}`);
                onLoad();
            };

            image.src = url;
        });
    });
}

// Render the maze
function renderMaze(gl, textures) {
    gl.clear(gl.COLOR_BUFFER_BIT);

    for (let y = 0; y < window.mazeHeight; y++) {
        for (let x = 0; x < window.mazeWidth; x++) {
            let texture = textures[window.tileData[y * window.mazeWidth + x]];
            if (!texture) continue;

            drawTile(gl, x, y, texture);
        }
    }
}

// Draw tile considering camera offset and zoom
function drawTile(gl, x, y, texture) {
    let tileSize = window.tileSize * window.zoomLevel;
    let offsetX = (x * tileSize - window.cameraX) / gl.canvas.width * 2 - 1;
    let offsetY = 1 - (y * tileSize - window.cameraY) / gl.canvas.height * 2;

    gl.bindTexture(gl.TEXTURE_2D, texture);

    const vertices = new Float32Array([
        offsetX, offsetY, 0.0, 0.0,
        offsetX + tileSize / gl.canvas.width * 2, offsetY, 1.0, 0.0,
        offsetX, offsetY - tileSize / gl.canvas.height * 2, 0.0, 1.0,
        offsetX + tileSize / gl.canvas.width * 2, offsetY - tileSize / gl.canvas.height * 2, 1.0, 1.0
    ]);

    const vertexBuffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
    gl.bufferData(gl.ARRAY_BUFFER, vertices, gl.STATIC_DRAW);

    const positionAttrib = gl.getAttribLocation(window.shaderProgram, "aPosition");
    const texCoordAttrib = gl.getAttribLocation(window.shaderProgram, "aTexCoord");

    gl.vertexAttribPointer(positionAttrib, 2, gl.FLOAT, false, 16, 0);
    gl.vertexAttribPointer(texCoordAttrib, 2, gl.FLOAT, false, 16, 8);
    gl.enableVertexAttribArray(positionAttrib);
    gl.enableVertexAttribArray(texCoordAttrib);

    gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);
}

// ✅ Fix: Camera movement and zooming now works without `tileData` issues
function setupInputListeners() {
    document.addEventListener("keydown", (event) => {
        let moveSpeed = 20 / window.zoomLevel;
        if (event.key === "ArrowUp") window.cameraY -= moveSpeed;
        if (event.key === "ArrowDown") window.cameraY += moveSpeed;
        if (event.key === "ArrowLeft") window.cameraX -= moveSpeed;
        if (event.key === "ArrowRight") window.cameraX += moveSpeed;

        renderMaze(window.gl, window.tileTextures);
    });

    document.addEventListener("wheel", (event) => {
        let zoomFactor = event.deltaY > 0 ? 0.9 : 1.1;
        window.zoomLevel = Math.max(0.5, Math.min(2, window.zoomLevel * zoomFactor));
        renderMaze(window.gl, window.tileTextures);
    });
}

console.log("✅ WebGL Script Loaded Successfully!");


window.initMinimap = function (tileData, width, height) {
    const canvas = document.getElementById("minimapCanvas");
    if (!canvas) {
        console.error("Minimap canvas not found!");
        return;
    }

    const ctx = canvas.getContext("2d");
    if (!ctx) {
        console.error("2D context not supported for minimap!");
        return;
    }

    canvas.width = width * 4;  // Minimap scale factor
    canvas.height = height * 4;

    ctx.fillStyle = "black";
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    for (let y = 0; y < height; y++) {
        for (let x = 0; x < width; x++) {
            let tile = tileData[y * width + x];
            ctx.fillStyle = tile.includes("wall") ? "gray" : "white";
            ctx.fillRect(x * 4, y * 4, 4, 4);
        }
    }

    console.log("Minimap Rendered Successfully");
};
