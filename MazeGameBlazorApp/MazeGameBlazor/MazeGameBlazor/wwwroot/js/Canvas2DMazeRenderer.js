// -------------------------------------------------------------------------------
// Core Configuration
// -------------------------------------------------------------------------------
window.cameraX = 0;
window.cameraY = 0;

window.playerLightRadius = 3; // default light radius

window.MazeConfig = {
    tileSize: 16,
    zoomLevel: 3,
    moveSpeed: 16
};

window.setPlayerLightRadius = function (radius) {
    console.log("Canvas2D setPlayerLightRadius:", radius);
    window.playerLightRadius = radius;
    renderViewport2D();
};

// -------------------------------------------------------------------------------
// Canvas2D Initialization
// -------------------------------------------------------------------------------
window.initCanvas2D = function (tileDataInput, width, height) {
    console.log("Initializing Canvas2D...");

    const canvas = document.getElementById("mazeCanvas");
    if (!canvas) {
        console.error("Canvas element not found!");
        return;
    }

    const ctx = canvas.getContext("2d");
    const bufferCanvas = document.createElement("canvas");
    const bufferCtx = bufferCanvas.getContext("2d");

    const tileSize = window.MazeConfig.tileSize;
    const zoomLevel = window.MazeConfig.zoomLevel;

    bufferCanvas.width = width * tileSize * zoomLevel;
    bufferCanvas.height = height * tileSize * zoomLevel;

    canvas.width = 800;
    canvas.height = 600;

    window.Canvas2DState = {
        canvas,
        ctx,
        bufferCanvas,
        bufferCtx,
        tileData: tileDataInput,
        mazeWidth: width,
        mazeHeight: height,
        tileTextures: {},
        itemImageCache: {},
        player: null,
        playerSpriteCache: {},
        minimapScale: 4
    };

    window.MazeSharedState = {
        mazeWidth: width,
        mazeHeight: height,
        tileData: tileDataInput
    };

    loadTextures(tileDataInput).then(textures => {
        window.Canvas2DState.tileTextures = textures;
        renderFullMaze2D();
        renderViewport2D();
    });

    window.MinimapRenderer.init(tileDataInput, width, height);
};

function renderFullMaze2D() {
    const state = window.Canvas2DState;
    const ctx = state.bufferCtx;
    ctx.clearRect(0, 0, state.bufferCanvas.width, state.bufferCanvas.height);

    const tileSize = window.MazeConfig.tileSize * window.MazeConfig.zoomLevel;

    for (let y = 0; y < state.mazeHeight; y++) {
        for (let x = 0; x < state.mazeWidth; x++) {
            const tileIndex = y * state.mazeWidth + x;
            const tileTextureKey = state.tileData[tileIndex];
            const texture = state.tileTextures[tileTextureKey];
            if (!texture) continue;

            const drawX = x * tileSize;
            const drawY = y * tileSize;
            ctx.drawImage(texture, drawX, drawY, tileSize, tileSize);
        }
    }

    renderItems2D(ctx, tileSize);
}

function renderViewport2D() {
    const state = window.Canvas2DState;
    const ctx = state.ctx;

    ctx.clearRect(0, 0, state.canvas.width, state.canvas.height);

    const sourceX = window.cameraX;
    const sourceY = window.cameraY;

    ctx.drawImage(
        state.bufferCanvas,
        sourceX, sourceY,
        state.canvas.width, state.canvas.height,
        0, 0,
        state.canvas.width, state.canvas.height
    );

    renderPlayer2D();
}

function loadTextures(tileData) {
    return new Promise(resolve => {
        const tileTextures = {};
        let loadedCount = 0;

        const uniqueTextures = [...new Set(tileData.filter(url => url))];

        uniqueTextures.forEach(url => {
            const image = new Image();
            image.onload = () => {
                tileTextures[url] = image;
                loadedCount++;
                if (loadedCount === uniqueTextures.length) resolve(tileTextures);
            };
            image.onerror = () => console.error(`Failed to load texture: ${url}`);
            image.src = url;
        });
    });
}

window.updateItemData2D = function (items) {
    const state = window.Canvas2DState;
    state.itemData = items;
    renderFullMaze2D();
    renderViewport2D();
};

function renderItems2D(ctx, tileSize) {
    const state = window.Canvas2DState;
    if (!state.itemData) return;

    state.itemData.forEach(item => {
        if (!state.itemImageCache[item.sprite]) {
            const img = new Image();
            img.src = item.sprite;
            img.onload = () => {
                state.itemImageCache[item.sprite] = img;
                drawItem2D(ctx, img, item, tileSize);
            };
        } else {
            const img = state.itemImageCache[item.sprite];
            drawItem2D(ctx, img, item, tileSize);
        }
    });
}

function drawItem2D(ctx, img, item, tileSize) {
    ctx.drawImage(img, item.x * tileSize, item.y * tileSize, tileSize, tileSize);
}

window.spawnPlayer2D = function (gridX, gridY, sprite) {
    const state = window.Canvas2DState;

    state.player = { x: gridX, y: gridY, sprite };

    const canvas = state.canvas;
    window.cameraX = (gridX * window.MazeConfig.tileSize * window.MazeConfig.zoomLevel) - (canvas.width / 2);
    window.cameraY = (gridY * window.MazeConfig.tileSize * window.MazeConfig.zoomLevel) - (canvas.height / 2);

    enforceCameraBounds(canvas);
    renderViewport2D();
    window.MinimapRenderer.updatePlayerPosition(gridX, gridY);
};

window.updatePlayerPosition2D = function (gridX, gridY, sprite) {
    const state = window.Canvas2DState;
    const canvas = state.canvas;

    state.player.x = gridX;
    state.player.y = gridY;
    state.player.sprite = sprite;

    window.cameraX = (gridX * window.MazeConfig.tileSize * window.MazeConfig.zoomLevel) - (canvas.width / 2);
    window.cameraY = (gridY * window.MazeConfig.tileSize * window.MazeConfig.zoomLevel) - (canvas.height / 2);

    enforceCameraBounds(canvas);
    renderViewport2D();
    window.MinimapRenderer.updatePlayerPosition(gridX, gridY);
};

window.renderPlayer2D = function () {
    const state = window.Canvas2DState;
    const player = state.player;
    const tileSize = window.MazeConfig.tileSize * window.MazeConfig.zoomLevel;
    const ctx = state.ctx;

    const drawX = (player.x * tileSize) - window.cameraX;
    const drawY = (player.y * tileSize) - window.cameraY;

    // Draw fog gradient first
    const radius = window.playerLightRadius * tileSize;
    const gradient = ctx.createRadialGradient(
        drawX + tileSize / 2, drawY + tileSize / 2, tileSize / 4,
        drawX + tileSize / 2, drawY + tileSize / 2, radius
    );

    gradient.addColorStop(0, "rgba(255,255,255,0.05)");
    gradient.addColorStop(1, "rgba(0,0,0,0.95)");

    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, state.canvas.width, state.canvas.height);

    const drawPlayer = (img) => {
        ctx.drawImage(img, drawX, drawY, tileSize, tileSize);
    };

    if (!state.playerSpriteCache[player.sprite]) {
        const img = new Image();
        img.src = player.sprite;
        img.onload = () => {
            state.playerSpriteCache[player.sprite] = img;
            drawPlayer(img);
        };
    } else {
        drawPlayer(state.playerSpriteCache[player.sprite]);
    }
};

function enforceCameraBounds(canvas) {
    const mazePixelWidth = window.Canvas2DState.mazeWidth * window.MazeConfig.tileSize * window.MazeConfig.zoomLevel;
    const mazePixelHeight = window.Canvas2DState.mazeHeight * window.MazeConfig.tileSize * window.MazeConfig.zoomLevel;

    const maxCameraX = Math.max(0, mazePixelWidth - canvas.width);
    const maxCameraY = Math.max(0, mazePixelHeight - canvas.height);

    window.cameraX = Math.max(0, Math.min(window.cameraX, maxCameraX));
    window.cameraY = Math.max(0, Math.min(window.cameraY, maxCameraY));
}

