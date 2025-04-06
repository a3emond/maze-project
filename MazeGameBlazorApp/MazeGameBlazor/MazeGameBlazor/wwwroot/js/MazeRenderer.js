window.MazeRenderer = {
    rendererType: "canvas2d", // Default fallback (can be overridden at init)

    init: function (tileData, width, height) {
        if (supportsWebGL()) {
            this.rendererType = "webgl";
            window.initWebGL(tileData, width, height);
        } else {
            this.rendererType = "canvas2d";
            window.initCanvas2D(tileData, width, height);
        }
        console.log("MazeRenderer using:", window.MazeRenderer.rendererType);

    },

    render: function () {
        if (this.rendererType === "webgl") {
            window.renderWebGL();
        } else {
            window.renderViewport();
        }
    },

    spawnPlayer: function (x, y, sprite) {
        if (this.rendererType === "webgl") {
            window.spawnPlayerWebGL(x, y, sprite);
        } else {
            window.spawnPlayer(x, y, sprite);
        }
    },

    updatePlayer: function (x, y, sprite) {
        if (this.rendererType === "webgl") {
            window.updatePlayerPositionWebGL(x, y, sprite);
        } else {
            window.updatePlayerPosition(x, y, sprite);
        }
    },

    updateItems: function (items) {
        if (this.rendererType === "webgl") {
            window.updateItemDataWebGL(items);
        } else {
            window.updateItemData(items);
        }
    }
};