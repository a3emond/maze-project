// log to console
console.log("MazeRenderer.js loaded.");
function supportsWebGL() {
    try {
        const canvas = document.createElement("canvas");
        const supported = !!(window.WebGLRenderingContext &&
            (canvas.getContext("webgl") || canvas.getContext("experimental-webgl")));
        console.log("supportsWebGL:", supported);
        return supported;
    } catch (e) {
        console.log("supportsWebGL threw an error:", e);
        return false;
    }
}

window.MazeRenderer = {
    rendererType: "canvas2d", // Default (will be overridden)

    init: function (tileData, width, height) {
        console.log("MazeRenderer.init called.");

        if (supportsWebGL()) {
            console.log("WebGL is supported. Initializing WebGL renderer...");
            this.rendererType = "webgl";
            window.MazeRendererType = "webgl";
            window.initWebGL(tileData, width, height);
        } else {
            console.log("WebGL not supported. Falling back to Canvas2D.");
            this.rendererType = "canvas2d";
            window.MazeRendererType = "canvas2d";
            window.initCanvas2D(tileData, width, height);
        }

        console.log("MazeRenderer using:", this.rendererType);
        window.MazeRendererReady = true;
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

// Ensure global flags are defined before any C# interop
window.MazeRendererReady = false;
window.MazeRendererType = "canvas2d"; // Will be overwritten by init()
