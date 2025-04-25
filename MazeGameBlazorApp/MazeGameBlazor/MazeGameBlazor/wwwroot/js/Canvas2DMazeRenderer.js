// -------------------------------------------------------------------------------
// Core Configuration
// -------------------------------------------------------------------------------
window.cameraX = 0;
window.cameraY = 0;

window.playerLightRadius = 3; // default light radius
let canvasNeedsRedraw = true;

window.MazeConfig = {
    tileSize: 16,
    zoomLevel: 3,
    moveSpeed: 16
};

window.setPlayerLightRadius = function (radius) {
    window.playerLightRadius = radius;
    requestRedraw2D();
};

// -------------------------------------------------------------------------------
// Canvas2D Initialization
// -------------------------------------------------------------------------------
window.initCanvas2D = function (tileDataInput, width, height) {
    console.log("Initializing Canvas2D...");

    const canvas = document.getElementById("mazeCanvas");
    if (!canvas) return console.error("Canvas element not found!");

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
        minimapScale: 4,
        itemData: []
    };

    window.MazeSharedState = {
        mazeWidth: width,
        mazeHeight: height,
        tileData: tileDataInput
    };

    loadTextures(tileDataInput).then(textures => {
        window.Canvas2DState.tileTextures = textures;
        renderFullMaze2D();
        requestRedraw2D();
    });

    window.MinimapRenderer.init(tileDataInput, width, height);
    requestAnimationFrame(renderIfNeeded2D);
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
            ctx.drawImage(texture, x * tileSize, y * tileSize, tileSize, tileSize);
        }
    }

    renderItems2D(ctx, tileSize);
}

function renderViewport2D() {
    const state = window.Canvas2DState;
    const ctx = state.ctx;
    if (!state.player) return;

    ctx.clearRect(0, 0, state.canvas.width, state.canvas.height);

    ctx.drawImage(
        state.bufferCanvas,
        window.cameraX, window.cameraY,
        state.canvas.width, state.canvas.height,
        0, 0,
        state.canvas.width, state.canvas.height
    );

    renderPlayer2D();
}

function renderIfNeeded2D() {
    if (canvasNeedsRedraw) {
        renderViewport2D();
        canvasNeedsRedraw = false;
    }
    requestAnimationFrame(renderIfNeeded2D);
}

function requestRedraw2D() {
    canvasNeedsRedraw = true;
}

function loadTextures(tileData) {
    return new Promise(resolve => {
        const tileTextures = {};
        const uniqueTextures = [...new Set(tileData.filter(url => url))];
        let loaded = 0;

        if (uniqueTextures.length === 0) return resolve(tileTextures);

        uniqueTextures.forEach(url => {
            const img = new Image();
            img.onload = () => {
                tileTextures[url] = img;
                if (++loaded === uniqueTextures.length) resolve(tileTextures);
            };
            img.onerror = () => {
                console.error(`Failed to load texture: ${url}`);
                if (++loaded === uniqueTextures.length) resolve(tileTextures);
            };
            img.src = url;
        });
    });
}

// -------------------------------------------------------------------------------
// Item Rendering
// -------------------------------------------------------------------------------
window.updateItemData2D = function (items) {
    const state = window.Canvas2DState;
    const tileSize = window.MazeConfig.tileSize * window.MazeConfig.zoomLevel;
    const ctx = state.bufferCtx;

    const previousItems = state.itemData || [];

    // Clear only the tiles that previously had items
    for (const item of previousItems) {
        const drawX = item.x * tileSize;
        const drawY = item.y * tileSize;

        const tileIndex = item.y * state.mazeWidth + item.x;
        const textureKey = state.tileData[tileIndex];
        const texture = state.tileTextures[textureKey];

        if (texture) {
            ctx.clearRect(drawX, drawY, tileSize, tileSize); // remove old item
            ctx.drawImage(texture, drawX, drawY, tileSize, tileSize); // redraw tile
        }
    }

    // Save new item list and render only those
    state.itemData = items;

    for (const item of items) {
        if (!state.itemImageCache[item.sprite]) {
            const img = new Image();
            img.src = item.sprite;
            img.onload = () => {
                state.itemImageCache[item.sprite] = img;
                ctx.drawImage(img, item.x * tileSize, item.y * tileSize, tileSize, tileSize);
                requestRedraw2D();
            };
        } else {
            const img = state.itemImageCache[item.sprite];
            ctx.drawImage(img, item.x * tileSize, item.y * tileSize, tileSize, tileSize);
        }
    }

    requestRedraw2D();
};


function renderItems2D(ctx, tileSize) {
    const state = window.Canvas2DState;
    for (const item of state.itemData || []) {
        const cached = state.itemImageCache[item.sprite];
        if (cached) {
            drawItem2D(ctx, cached, item, tileSize);
        } else {
            const img = new Image();
            img.onload = () => {
                state.itemImageCache[item.sprite] = img;
                drawItem2D(ctx, img, item, tileSize);
                requestRedraw2D();
            };
            img.src = item.sprite;
        }
    }
}

function drawItem2D(ctx, img, item, tileSize) {
    ctx.drawImage(img, item.x * tileSize, item.y * tileSize, tileSize, tileSize);
}

// -------------------------------------------------------------------------------
// Player Handling
// -------------------------------------------------------------------------------
window.spawnPlayer2D = function (gridX, gridY, sprite) {
    const state = window.Canvas2DState;
    state.player = { x: gridX, y: gridY, sprite };
    centerCameraOn(gridX, gridY);
    window.MinimapRenderer.updatePlayerPosition(gridX, gridY);
    requestRedraw2D();
};

window.updatePlayerPosition2D = function (gridX, gridY, sprite) {
    const state = window.Canvas2DState;
    state.player.x = gridX;
    state.player.y = gridY;
    state.player.sprite = sprite;
    centerCameraOn(gridX, gridY);
    window.MinimapRenderer.updatePlayerPosition(gridX, gridY);
    requestRedraw2D();
};

function centerCameraOn(x, y) {
    const canvas = Canvas2DState.canvas;
    const tileSize = window.MazeConfig.tileSize * window.MazeConfig.zoomLevel;
    window.cameraX = x * tileSize - canvas.width / 2;
    window.cameraY = y * tileSize - canvas.height / 2;
    enforceCameraBounds(canvas);
}

function enforceCameraBounds(canvas) {
    const tileSize = window.MazeConfig.tileSize * window.MazeConfig.zoomLevel;
    const maxX = Canvas2DState.mazeWidth * tileSize - canvas.width;
    const maxY = Canvas2DState.mazeHeight * tileSize - canvas.height;
    window.cameraX = Math.max(0, Math.min(window.cameraX, maxX));
    window.cameraY = Math.max(0, Math.min(window.cameraY, maxY));
}

window.renderPlayer2D = function () {
    const state = window.Canvas2DState;
    const player = state.player;
    if (!player || player.x == null || player.y == null) return;

    const tileSize = window.MazeConfig.tileSize * window.MazeConfig.zoomLevel;
    const ctx = state.ctx;
    const drawX = (player.x * tileSize) - window.cameraX;
    const drawY = (player.y * tileSize) - window.cameraY;

    // Fog
    const radius = window.playerLightRadius * tileSize;
    const gradient = ctx.createRadialGradient(
        drawX + tileSize / 2, drawY + tileSize / 2, tileSize * 0.2,
        drawX + tileSize / 2, drawY + tileSize / 2, radius
    );

    // More visible glow at the center, less aggressive fade
    gradient.addColorStop(0.0, "rgba(100,255,180,0.3)");    // cyan-green
    gradient.addColorStop(0.3, "rgba(120,80,255,0.4)");     // purplish
    gradient.addColorStop(0.6, "rgba(80,0,140,0.6)");       // deep violet
    gradient.addColorStop(1.0, "rgba(0,0,0,1)");         // full black

    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, state.canvas.width, state.canvas.height);


    // Player
    const cached = state.playerSpriteCache[player.sprite];
    if (cached) {
        ctx.drawImage(cached, drawX, drawY, tileSize, tileSize);
    } else {
        const img = new Image();
        img.onload = () => {
            state.playerSpriteCache[player.sprite] = img;
            requestRedraw2D();
        };
        img.src = player.sprite;
    }
};
