// -------------------------------------------------------------------------------
// MinimapRenderer.js (Standalone Minimap Rendering Logic)
// -------------------------------------------------------------------------------

window.MiniMapState = {
    minimapCanvas: null,
    ctx: null,
    scale: 4,
    fogOfWar: [],
    goal: null,
    playerX: 0,
    playerY: 0
};

window.MinimapRenderer = {
    scale: 4,
    fogOfWar: [],
    goalPosition: null,

    init: function (tileData, width, height) {
        const minimapCanvas = document.getElementById("minimapCanvas");
        if (!minimapCanvas) return console.error("Minimap canvas not found!");

        const ctx = minimapCanvas.getContext("2d");
        if (!ctx) return console.error("2D context not supported for minimap!");

        this.scale = window.Canvas2DState?.minimapScale || 4;
        minimapCanvas.width = width * this.scale;
        minimapCanvas.height = height * this.scale;
        ctx.imageSmoothingEnabled = false;

        this.fogOfWar = Array.from({ length: height }, () => Array(width).fill(false));
        ctx.fillStyle = "#000";
        ctx.fillRect(0, 0, minimapCanvas.width, minimapCanvas.height);

        // Shared access for both renderers
        window.MazeSharedState = {
            mazeWidth: width,
            mazeHeight: height,
            tileData: tileData
        };
    },

    render: function () {
        const state = window.MazeSharedState;
        const minimapCanvas = document.getElementById("minimapCanvas");
        const ctx = minimapCanvas.getContext("2d");
        const scale = this.scale;

        ctx.clearRect(0, 0, minimapCanvas.width, minimapCanvas.height);

        for (let y = 0; y < state.mazeHeight; y++) {
            for (let x = 0; x < state.mazeWidth; x++) {
                const tileIndex = y * state.mazeWidth + x;
                const tileType = state.tileData[tileIndex];
                if (!tileType) continue;

                ctx.fillStyle = this.fogOfWar[y][x] ? (tileType.includes("wall") ? "#AAA" : "#FFF") : "#000";
                ctx.fillRect(x * scale, y * scale, scale, scale);
            }
        }

        if (this.goalPosition) {
            const { x, y } = this.goalPosition;
            ctx.fillStyle = "lime";
            const radius = scale * 2;
            ctx.beginPath();
            ctx.arc((x + 0.5) * scale, (y + 0.5) * scale, radius, 0, Math.PI * 2);
            ctx.fill();
        }
    },

    revealArea: function (playerX, playerY, radius = 3) {
        const state = window.MazeSharedState;
        const width = state.mazeWidth;
        const height = state.mazeHeight;

        for (let dy = -radius; dy <= radius; dy++) {
            for (let dx = -radius; dx <= radius; dx++) {
                const nx = playerX + dx;
                const ny = playerY + dy;
                if (nx >= 0 && ny >= 0 && nx < width && ny < height) {
                    this.fogOfWar[ny][nx] = true;
                }
            }
        }

        this.render();
    },

    updatePlayerPosition: function (x, y) {
        const minimapCanvas = document.getElementById("minimapCanvas");
        const ctx = minimapCanvas.getContext("2d");
        const scale = this.scale;

        this.revealArea(x, y);

        const centerX = (x + 0.5) * scale;
        const centerY = (y + 0.5) * scale;
        const radius = scale * 5.5;

        ctx.beginPath();
        ctx.arc(centerX, centerY, radius, 0, Math.PI * 2);
        ctx.fillStyle = "#f00";
        ctx.fill();
    },

    setGoal: function (x, y) {
        this.goalPosition = { x, y };
        this.render();
    }
};
