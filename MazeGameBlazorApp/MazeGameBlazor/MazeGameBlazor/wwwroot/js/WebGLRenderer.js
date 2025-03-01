// Global WebGL context and shader program
window.gl = null;
window.shaderProgram = null;
window.tileTextures = {};

// Initialize WebGL
window.initWebGL = function (tileData, width, height) {
    console.log("Initializing WebGL...");
    const canvas = document.getElementById("mazeCanvas");

    if (!canvas) {
        console.error("❌ Error: Canvas element not found!");
        return;
    }

    // Get WebGL context
    window.gl = canvas.getContext("webgl");
    if (!window.gl) {
        console.error("❌ Error: WebGL is not supported!");
        return;
    }

    const gl = window.gl;

    // Set canvas size based on maze dimensions
    canvas.width = width * 16;
    canvas.height = height * 16;
    gl.viewport(0, 0, gl.drawingBufferWidth, gl.drawingBufferHeight);

    // Clear canvas
    gl.clearColor(0.0, 0.0, 0.0, 1.0);
    gl.clear(gl.COLOR_BUFFER_BIT);

    console.log("✅ WebGL Initialized Successfully!");

    // ✅ Initialize shaders
    initShaders(gl);

    // ✅ Load textures
    loadTextures(tileData).then(textures => {
        console.log("✅ Textures Loaded:", textures);
        renderMazeWithTextures(gl, textures, tileData, width, height);
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

    // Create shader program
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
                console.log(`✅ Texture loaded: ${url}`);
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
function renderMazeWithTextures(gl, textures, tileData, width, height) {
    console.log("🔹 Rendering maze...");
    for (let y = 0; y < height; y++) {
        for (let x = 0; x < width; x++) {
            let texture = textures[tileData[y * width + x]];
            if (!texture) {
                console.warn(`⚠️ Missing texture at (${x}, ${y}):`, tileData[y * width + x]);
                continue;
            }
            drawTile(gl, x * 16, y * 16, texture);
        }
    }
    console.log("✅ Maze Rendered Successfully!");
}

// Draw individual tile
function drawTile(gl, x, y, texture) {
    if (!gl || !texture) {
        console.warn(`⚠️ Skipping tile draw at (${x}, ${y}) - Missing texture or WebGL context.`);
        return;
    }

    // Bind the texture
    gl.bindTexture(gl.TEXTURE_2D, texture);

    // Set up quad vertices
    const vertices = new Float32Array([
        -1 + (x / gl.canvas.width) * 2, 1 - (y / gl.canvas.height) * 2, 0.0, 0.0,
        -1 + ((x + 16) / gl.canvas.width) * 2, 1 - (y / gl.canvas.height) * 2, 1.0, 0.0,
        -1 + (x / gl.canvas.width) * 2, 1 - ((y + 16) / gl.canvas.height) * 2, 0.0, 1.0,
        -1 + ((x + 16) / gl.canvas.width) * 2, 1 - ((y + 16) / gl.canvas.height) * 2, 1.0, 1.0
    ]);

    // Create buffer for vertices
    const vertexBuffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
    gl.bufferData(gl.ARRAY_BUFFER, vertices, gl.STATIC_DRAW);

    // Get shader attribute locations
    const positionAttrib = gl.getAttribLocation(window.shaderProgram, "aPosition");
    const texCoordAttrib = gl.getAttribLocation(window.shaderProgram, "aTexCoord");

    // Enable attributes
    gl.vertexAttribPointer(positionAttrib, 2, gl.FLOAT, false, 16, 0);
    gl.vertexAttribPointer(texCoordAttrib, 2, gl.FLOAT, false, 16, 8);
    gl.enableVertexAttribArray(positionAttrib);
    gl.enableVertexAttribArray(texCoordAttrib);

    // Draw tile
    gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);
}


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
