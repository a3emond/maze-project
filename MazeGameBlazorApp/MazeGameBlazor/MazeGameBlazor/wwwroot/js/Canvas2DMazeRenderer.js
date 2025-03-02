
// Global variables
window.canvas = null;
window.ctx = null;
window.tileSize = 16; // Base tile size
window.zoomLevel = 3; // Zoom factor (adjust for desired view)
window.cameraX = 0; // Camera position (top-left)
window.cameraY = 0;
window.moveSpeed = 16; // How fast the camera moves

window.tileData = [];
window.tileTextures = {};
window.mazeWidth = 0;
window.mazeHeight = 0;

// 🎯 Initialize Canvas2D Renderer
window.initCanvas2D = function(tileDataInput, width, height) {
    console.log("🖌 Initializing Canvas2D...");

    window.canvas = document.getElementById("mazeCanvas");
    if (!window.canvas) {
        console.error("❌ Canvas element not found!");
        return;
    }
    window.ctx = window.canvas.getContext("2d");

    // Store maze data
    window.tileData = tileDataInput;
    window.mazeWidth = width;
    window.mazeHeight = height;

    // Set fixed canvas size for game window (DO NOT SCALE CANVAS)
    window.canvas.width = 800; // Fixed game window width
    window.canvas.height = 600; // Fixed game window height

    console.log(`🖼 Maze Size: ${width} x ${height}, Canvas: ${canvas.width} x ${canvas.height}`);

    loadTextures(tileDataInput).then(textures => {
        window.tileTextures = textures;
        renderMaze();
    }).catch(error => console.error("❌ Texture loading failed:", error));

    initMinimap(tileDataInput, width, height);
};

// 🖼 Load Textures
function loadTextures(tileData) {
    return new Promise(resolve => {
        const tileTextures = {};
        let loadedCount = 0;

        const uniqueTextures = [...new Set(tileData.filter(url => url))];
        console.log("🎨 Loading Textures:", uniqueTextures);

        uniqueTextures.forEach(url => {
            const image = new Image();
            image.onload = () => {
                tileTextures[url] = image;
                loadedCount++;

                if (loadedCount === uniqueTextures.length) resolve(tileTextures);
            };
            image.onerror = () => console.error(`❌ Failed to Load Texture: ${url}`);
            image.src = url;
        });
    });
}

//// 🎮 Handle Camera Movement (WASD)
//function setupInputListeners() {
//    document.addEventListener("keydown", (event) => {
//        let moved = false;

//        if (event.key === "w" || event.key === "W") {
//            window.cameraY -= window.moveSpeed;
//            moved = true;
//        }
//        if (event.key === "s" || event.key === "S") {
//            window.cameraY += window.moveSpeed;
//            moved = true;
//        }
//        if (event.key === "a" || event.key === "A") {
//            window.cameraX -= window.moveSpeed;
//            moved = true;
//        }
//        if (event.key === "d" || event.key === "D") {
//            window.cameraX += window.moveSpeed;
//            moved = true;
//        }

//        if (moved) {
//            enforceCameraBounds(); // ✅ Ensure camera stays within maze limits
//            renderMaze(); // ✅ Render updated view
//        }
//    });
//}

// 🛠 Fix Camera Boundaries
function enforceCameraBounds() {
    const mazePixelWidth = window.mazeWidth * window.tileSize * window.zoomLevel;
    const mazePixelHeight = window.mazeHeight * window.tileSize * window.zoomLevel;

    const maxCameraX = Math.max(0, mazePixelWidth - window.canvas.width);
    const maxCameraY = Math.max(0, mazePixelHeight - window.canvas.height);

    window.cameraX = Math.max(0, Math.min(window.cameraX, maxCameraX));
    window.cameraY = Math.max(0, Math.min(window.cameraY, maxCameraY));

    console.log(`📷 Camera Bounds Enforced: X(${window.cameraX}/${maxCameraX}), Y(${window.cameraY}/${maxCameraY})`);
}


// 🔍 Render only visible tiles
function renderMaze() {
    const ctx = window.ctx;
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    const tileSize = window.tileSize * window.zoomLevel;

    const startX = Math.floor(window.cameraX / tileSize);
    const startY = Math.floor(window.cameraY / tileSize);
    const endX = Math.min(window.mazeWidth, Math.ceil((window.cameraX + window.canvas.width) / tileSize));
    const endY = Math.min(window.mazeHeight, Math.ceil((window.cameraY + window.canvas.height) / tileSize));

    console.log(`🎥 Rendering Viewport: X(${startX} - ${endX}), Y(${startY} - ${endY})`);

    for (let y = startY; y < endY; y++) {
        for (let x = startX; x < endX; x++) {
            const tileIndex = y * window.mazeWidth + x;
            const tileTextureKey = window.tileData[tileIndex];
            const texture = window.tileTextures[tileTextureKey];

            if (!texture) continue;

            // Calculate on-screen position
            const drawX = (x * tileSize) - window.cameraX;
            const drawY = (y * tileSize) - window.cameraY;

            ctx.drawImage(texture, drawX, drawY, tileSize, tileSize);
        }
    }
}

// ✅ Log success
console.log("✅ Canvas2D with Camera Loaded!");


// ✅ Log success
console.log("✅ Canvas2D with Minimap Loaded!");


window.spawnPlayer = function(gridX, gridY, sprite) {
    window.player = {
        x: gridX, // Logical grid position
        y: gridY,
        sprite: sprite
    };

    // Ensure the camera is positioned to center on the player at the start
    window.cameraX = (window.player.x * window.tileSize * window.zoomLevel) - (window.canvas.width / 2);
    window.cameraY = (window.player.y * window.tileSize * window.zoomLevel) - (window.canvas.height / 2);

    // Make sure the camera doesn't go out of bounds
    enforceCameraBounds();

    // Render everything
    renderMaze();
    renderPlayer();
};

window.renderPlayer = function() {
    const canvas = document.getElementById("mazeCanvas");
    const ctx = canvas.getContext("2d");
    const img = new Image();
    img.src = window.player.sprite;

    img.onload = () => {
        // Convert grid position to screen position relative to camera
        const tileSize = window.tileSize * window.zoomLevel; // Match zoom scaling
        const playerSize = tileSize; // Player size should be the same as tiles (adjust if needed)

        // Correct viewport positioning
        const viewportX = (window.canvas.width / 2) - (playerSize / 2); // Center player in the viewport
        const viewportY = (window.canvas.height / 2) - (playerSize / 2);

        // Adjust position to keep the player centered while moving
        const drawX = (window.player.x * tileSize) - window.cameraX + (tileSize / 2) - (playerSize / 2);
        const drawY = (window.player.y * tileSize) - window.cameraY + (tileSize / 2) - (playerSize / 2);

        // Ensure the maze is rendered before drawing the player
        renderMaze();
        ctx.drawImage(img, drawX, drawY, playerSize, playerSize);
    };
};

window.updatePlayerPosition = function(gridX, gridY, sprite) {
    window.player.x = gridX;
    window.player.y = gridY;
    window.player.sprite = sprite;

    // Center the camera on the player
    window.cameraX = (gridX * window.tileSize * window.zoomLevel) - (window.canvas.width / 2);
    window.cameraY = (gridY * window.tileSize * window.zoomLevel) - (window.canvas.height / 2);

    enforceCameraBounds(); // Keep camera inside maze
    renderMaze();
    renderPlayer();
};

window.focusGameScreen = function() {
    const gameScreen = document.querySelector(".game-screen");
    if (gameScreen) {
        gameScreen.focus();
    }
};


window.registerKeyListeners = (dotNetInstance) => {
    const activeKeys = new Set();

    document.addEventListener("keydown",
        (event) => {
            const key = event.key.toLowerCase();
            if (!activeKeys.has(key)) {
                activeKeys.add(key);
                dotNetInstance.invokeMethodAsync("HandleKeyPress", key);
            }
        });

    document.addEventListener("keyup",
        (event) => {
            const key = event.key.toLowerCase();
            activeKeys.delete(key);
            dotNetInstance.invokeMethodAsync("HandleKeyRelease", key);
        });
};


window.clearOverlay = function() {
    document.getElementById("gameOverlay").style.display = "none";
};

// -------------------------------------------------------------------------------
// ------------------------------Minimap Rendering--------------------------------
// -------------------------------------------------------------------------------

// 🗺️ Render Minimap
function initMinimap(tileData, width, height) {
    const minimapCanvas = document.getElementById("minimapCanvas");
    if (!minimapCanvas) {
        console.error("❌ Minimap canvas not found!");
        return;
    }

    const ctx = minimapCanvas.getContext("2d");
    if (!ctx) {
        console.error("❌ 2D context not supported for minimap!");
        return;
    }

    const scale = 4; // Minimap scale factor (4px per tile)
    minimapCanvas.width = width * scale;
    minimapCanvas.height = height * scale;

    // Disable smoothing for sharp pixel-based rendering
    ctx.imageSmoothingEnabled = false;

    console.log(`🗺 Rendering Minimap (${width} x ${height})`);

    // Predefine colors for walls, floors, and unexplored areas
    const wallColor = "#222"; // Dark gray for walls
    const floorColor = "#DDD"; // Light gray for walkable paths
    const fogColor = "#111"; // Blackish-gray for unexplored areas

    // Initialize fog of war (all tiles are "unexplored" at the beginning)
    if (!window.fogOfWar) {
        window.fogOfWar = Array.from({ length: height }, () => Array(width).fill(true));
    }

    // Draw minimap tiles
    for (let y = 0; y < height; y++) {
        for (let x = 0; x < width; x++) {
            const tileIndex = y * width + x;
            const tileType = tileData[tileIndex];

            if (!tileType) continue;

            // Decide tile color based on type and fog visibility
            if (!window.fogOfWar[y][x]) {
                ctx.fillStyle = fogColor; // Hide unexplored areas
            } else {
                ctx.fillStyle = tileType.includes("wall") ? wallColor : floorColor;
            }

            ctx.fillRect(x * scale, y * scale, scale, scale);
        }
    }

    console.log("✅ Minimap Rendered with High Clarity & Fog of War");
};


function revealMinimapArea(playerX, playerY, radius = 2) {
    const width = window.mazeWidth;
    const height = window.mazeHeight;

    for (let dy = -radius; dy <= radius; dy++) {
        for (let dx = -radius; dx <= radius; dx++) {
            const nx = playerX + dx;
            const ny = playerY + dy;

            if (nx >= 0 && ny >= 0 && nx < width && ny < height) {
                window.fogOfWar[ny][nx] = true;
            }
        }
    }

    renderMinimap(); // Update minimap with revealed areas
}

window.renderMinimap = function() {
    const minimapCanvas = document.getElementById("minimapCanvas");
    if (!minimapCanvas) {
        console.error("❌ Minimap canvas not found!");
        return;
    }

    const ctx = minimapCanvas.getContext("2d");
    const scale = 4; // Minimap scale

    ctx.clearRect(0, 0, minimapCanvas.width, minimapCanvas.height); // Clear previous minimap

    // Predefine colors for walls, floors, and unexplored areas
    const wallColor = "#222"; // Dark gray for walls
    const floorColor = "#DDD"; // Light gray for walkable paths
    const fogColor = "#111"; // Blackish-gray for unexplored areas

    for (let y = 0; y < window.mazeHeight; y++) {
        for (let x = 0; x < window.mazeWidth; x++) {
            const tileIndex = y * window.mazeWidth + x;
            const tileType = window.tileData[tileIndex];

            if (!tileType) continue;

            // Decide tile color based on type and fog visibility
            if (!window.fogOfWar[y][x]) {
                ctx.fillStyle = fogColor; // Hide unexplored areas
            } else {
                ctx.fillStyle = tileType.includes("wall") ? wallColor : floorColor;
            }

            ctx.fillRect(x * scale, y * scale, scale, scale);
        }
    }

    console.log("✅ Minimap Redrawn");
};


let playerBlinkState = 1.0; // Start fully visible
let blinkDirection = -0.05; // Fade in & out smoothly

window.updatePlayerOnMinimap = function(playerX, playerY) {
    const minimapCanvas = document.getElementById("minimapCanvas");
    if (!minimapCanvas) {
        console.error("❌ Minimap canvas not found!");
        return;
    }

    const ctx = minimapCanvas.getContext("2d");
    if (!ctx) {
        console.error("❌ 2D context not supported for minimap!");
        return;
    }

    const scale = 4; // Minimap scale factor (4px per tile)

    console.log(`📍 Drawing player at minimap position: (${playerX}, ${playerY})`);

    // Render the minimap **before** adding the player dot
    renderMinimap();

    setTimeout(() => {
            if (playerX < 0 || playerX >= window.mazeWidth || playerY < 0 || playerY >= window.mazeHeight) {
                console.warn(`⚠️ Player position (${playerX}, ${playerY}) is out of minimap bounds.`);
                return;
            }

            // Draw red dot for player
            ctx.fillStyle = "red";
            ctx.fillRect(playerX * scale, playerY * scale, scale, scale);
            console.log(`✅ Player dot drawn at (${playerX * scale}, ${playerY * scale}) on minimap.`);
        },
        50); // Short delay to prevent overwriting
};