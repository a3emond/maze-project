// Global WebGL variables
window.gl = null;
window.shaderProgram = null;
window.tileTextures = {};
window.tileSize = 16;
window.tileData = [];
window.mazeWidth = 0;
window.mazeHeight = 0;

// Initialize WebGL and Render the Maze
window.initWebGL = function(tileDataInput, width, height) {
    console.log("Initializing WebGL...");

    const canvas = document.getElementById("mazeCanvas");
    if (!canvas) {
        console.error("❌ Canvas element not found!");
        return;
    }

    window.gl = canvas.getContext("webgl");
    if (!window.gl) {
        console.error("❌ WebGL not supported!");
        return;
    }

    // Store maze data
    window.tileData = tileDataInput;
    window.mazeWidth = width;
    window.mazeHeight = height;

    // Adjust canvas size to fit the entire maze
    canvas.width = width * window.tileSize;
    canvas.height = height * window.tileSize;
    window.gl.viewport(0, 0, canvas.width, canvas.height);

    console.log(`🖼 Maze Dimensions: ${width} x ${height}, Canvas Size: ${canvas.width} x ${canvas.height}`);

    initShaders();
    loadTextures(tileDataInput).then(textures => {
        window.tileTextures = textures;
        renderMaze(); // Call render function once textures are ready
    }).catch(error => console.error("❌ Texture loading failed:", error));
};

// Initialize shaders
function initShaders() {
    console.log("🔹 Compiling shaders...");
    const gl = window.gl;

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

    const shaderProgram = gl.createProgram();
    gl.attachShader(shaderProgram, vertexShader);
    gl.attachShader(shaderProgram, fragmentShader);
    gl.linkProgram(shaderProgram);
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
        console.error("❌ Shader compilation error:", gl.getShaderInfoLog(shader));
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

        const uniqueTextures = [...new Set(tileData.filter(url => url))];

        console.log("🔹 Loading Textures:", uniqueTextures);

        uniqueTextures.forEach(url => {
            const texture = gl.createTexture();
            const image = new Image();
            image.onload = () => {
                gl.bindTexture(gl.TEXTURE_2D, texture);
                gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
                gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
                tileTextures[url] = texture;
                loadedCount++;

                if (loadedCount === uniqueTextures.length) resolve(tileTextures);
            };
            image.onerror = () => console.error(`❌ Failed to Load Texture: ${url}`);
            image.src = url;
        });
    });
}

// Render full maze
function renderMaze() {
    const gl = window.gl;
    gl.clear(gl.COLOR_BUFFER_BIT);

    console.log(`🖼 Rendering Full Maze (${window.mazeWidth} x ${window.mazeHeight})`);

    for (let y = 0; y < window.mazeHeight; y++) {
        for (let x = 0; x < window.mazeWidth; x++) {
            const tileIndex = y * window.mazeWidth + x;
            const tileTextureKey = window.tileData[tileIndex];
            const texture = window.tileTextures[tileTextureKey];

            if (!texture) {
                console.warn(`🚨 Missing Texture for Tile (${x}, ${y}), Key: ${tileTextureKey}`);
                continue;
            }
            drawTile(gl, x, y, texture);
        }
    }
}

// Draw a single tile at (x, y) with the given texture
function drawTile(gl, x, y, texture) {
    gl.bindTexture(gl.TEXTURE_2D, texture);

    const tileSize = window.tileSize;
    const x0 = (x * tileSize) / gl.canvas.width * 2 - 1; // Convert to NDC (-1 to 1)
    const y0 = 1 - (y * tileSize) / gl.canvas.height * 2; // Convert to NDC (-1 to 1)

    const x1 = x0 + (tileSize / gl.canvas.width) * 2;
    const y1 = y0 - (tileSize / gl.canvas.height) * 2;

    const vertices = new Float32Array([
        x0, y0, 0.0, 0.0, // Top-left
        x1, y0, 1.0, 0.0, // Top-right
        x0, y1, 0.0, 1.0, // Bottom-left
        x1, y1, 1.0, 1.0 // Bottom-right
    ]);

    const buffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
    gl.bufferData(gl.ARRAY_BUFFER, vertices, gl.STATIC_DRAW);

    const positionAttrib = gl.getAttribLocation(window.shaderProgram, "aPosition");
    const texCoordAttrib = gl.getAttribLocation(window.shaderProgram, "aTexCoord");

    gl.vertexAttribPointer(positionAttrib, 2, gl.FLOAT, false, 16, 0);
    gl.vertexAttribPointer(texCoordAttrib, 2, gl.FLOAT, false, 16, 8);
    gl.enableVertexAttribArray(positionAttrib);
    gl.enableVertexAttribArray(texCoordAttrib);

    gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);

}


console.log("✅ Simplified WebGL Script Loaded!");


window.initMinimap = function(tileData, width, height) {
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

    canvas.width = width * 4; // Minimap scale factor
    canvas.height = height * 4;

    ctx.fillStyle = "black";
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    for (let y = 0; y < height; y++) {
        for (let x = 0; x < width; x++) {
            const tile = tileData[y * width + x];
            ctx.fillStyle = tile.includes("wall") ? "gray" : "white";
            ctx.fillRect(x * 4, y * 4, 4, 4);
        }
    }

    console.log("Minimap Rendered Successfully");
};