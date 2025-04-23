// -------------------------------------------------------------------------------
// MazeRenderer Wrapper
// -------------------------------------------------------------------------------
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
    rendererType: "canvas2d",

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

        try {
            MinimapRenderer.init(tileData, width, height);
        } catch (err) {
            console.warn("MinimapRenderer init failed:", err);
        }

        console.log("MazeRenderer using:", this.rendererType);
        window.MazeRendererReady = true;
    },

    render: function () {
        if (this.rendererType === "webgl") {
            window.renderWebGL();
        } else {
            renderViewport2D();
        }
    },

    spawnPlayer: function (x, y, sprite) {
        if (this.rendererType === "webgl") {
            window.spawnPlayerWebGL(x, y, sprite);
        } else {
            window.spawnPlayer2D(x, y, sprite);
        }

        if (typeof MinimapRenderer?.updatePlayerPosition === "function") {
            MinimapRenderer.updatePlayerPosition(x, y);
        }
    },

    updatePlayer: function (x, y, sprite) {
        if (this.rendererType === "webgl") {
            window.updatePlayerPositionWebGL(x, y, sprite);
        } else {
            window.updatePlayerPosition2D(x, y, sprite);
        }

        if (typeof MinimapRenderer?.updatePlayerPosition === "function") {
            MinimapRenderer.updatePlayerPosition(x, y);
        }
    },

    updateItems: function (items) {
        if (this.rendererType === "webgl") {
            window.updateItemDataWebGL(items);
        } else {
            window.updateItemData2D(items);
        }
    },

    setGoal: function (x, y) {
        if (typeof MinimapRenderer?.setGoal === "function") {
            MinimapRenderer.setGoal(x, y);
        }
    }
};

window.MazeRendererReady = false;
window.MazeRendererType = "canvas2d";



// -------------------------------------------------------------------------------
// Overlay Support
// -------------------------------------------------------------------------------
window.showOverlay = function (message = "Paused", showButton = false) {
    const overlay = document.getElementById("gameOverlay");
    const status = document.getElementById("gameStatus");

    if (overlay) overlay.style.display = "flex";

    if (status) {
        status.textContent = message;
        status.classList.remove("pulse");
    }

    const btn = overlay.querySelector("button");
    if (btn) btn.style.display = showButton ? "inline-block" : "none";
};

window.clearOverlay = function () {
    const overlay = document.getElementById("gameOverlay");
    if (overlay) overlay.style.display = "none";
};

window.focusGameScreen = function () {
    const gameScreen = document.querySelector(".game-screen");
    if (gameScreen) gameScreen.focus();
};
window.autoFocusGame = function () {
    const gameScreen = document.querySelector(".game-screen");

    const focusGame = () => {
        if (gameScreen) {
            gameScreen.focus();
            console.log("[MazeRenderer] Focus restored to .game-screen");
        }
    };

    // Restore focus when user switches back to the tab
    document.addEventListener("visibilitychange", () => {
        if (!document.hidden) {
            focusGame();
        }
    });

    // Restore focus on mouse/touch return
    window.addEventListener("focus", focusGame);

    // Optional: restore on pointer move if user didn't click
    document.addEventListener("pointermove", () => {
        if (document.activeElement !== gameScreen) {
            focusGame();
        }
    });
};


window.registerKeyListeners = (dotNetInstance) => {
    const activeKeys = new Set();
    const gameScreen = document.querySelector(".game-screen");

    const normalizeKey = (key) => key.toLowerCase();

    document.addEventListener("keydown", (event) => {
        const key = normalizeKey(event.key);
        if (!activeKeys.has(key)) {
            activeKeys.add(key);
            dotNetInstance.invokeMethodAsync("HandleKeyPress", key);
        }
    });

    document.addEventListener("keyup", (event) => {
        const key = normalizeKey(event.key);
        if (activeKeys.has(key)) {
            activeKeys.delete(key);
            dotNetInstance.invokeMethodAsync("HandleKeyRelease", key);
        }
    });

    // Clear keys on window blur (e.g. tab switch or click outside)
    window.addEventListener("blur", () => {
        for (const key of activeKeys) {
            dotNetInstance.invokeMethodAsync("HandleKeyRelease", key);
        }
        activeKeys.clear();
    });

    // Also clear on visibility change (minimized, tab switch, etc.)
    document.addEventListener("visibilitychange", () => {
        if (document.hidden) {
            for (const key of activeKeys) {
                dotNetInstance.invokeMethodAsync("HandleKeyRelease", key);
            }
            activeKeys.clear();
        }
    });

    // Restore focus on click/tap
    if (gameScreen) {
        gameScreen.addEventListener("click", () => {
            gameScreen.focus();
        });
    }

    console.log("MazeRenderer: Key listener registered.");
};
