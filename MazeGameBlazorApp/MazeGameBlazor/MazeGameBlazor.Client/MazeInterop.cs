using MazeGameBlazor.Shared.GameEngine.Models;
using Microsoft.JSInterop;

namespace MazeGameBlazor.Client
{
    public static class MazeInterop
    {
        // ================================
        // 🎮 Maze Renderer Interop
        // ================================
        public static async Task<MazeRendererType> DetectRendererAsync(IJSRuntime js)
        {
            var renderer = await js.InvokeAsync<string>("eval", "window.MazeRenderer?.rendererType || 'canvas2d'");
            return Enum.TryParse(renderer, true, out MazeRendererType result) ? result : MazeRendererType.Canvas2D;
        }

        public static async Task InitRendererAsync(IJSRuntime js, MazeRendererType rendererType, string[] tileData, int width, int height)
        {
            await js.InvokeVoidAsync("MazeRenderer.init", tileData, width, height);
        }

        public static async Task SpawnPlayerAsync(IJSRuntime js, MazeRendererType rendererType, int x, int y, string sprite)
        {
            await js.InvokeVoidAsync("MazeRenderer.spawnPlayer", x, y, sprite);
        }

        public static async Task UpdatePlayerAsync(IJSRuntime js, MazeRendererType rendererType, int x, int y, string sprite)
        {
            await js.InvokeVoidAsync("MazeRenderer.updatePlayer", x, y, sprite);
        }

        public static async Task UpdateItemsAsync(IJSRuntime js, MazeRendererType rendererType, object items)
        {
            await js.InvokeVoidAsync("MazeRenderer.updateItems", items);
        }

        public static async Task SetMinimapGoalAsync(IJSRuntime js, int x, int y)
        {
            await js.InvokeVoidAsync("MazeRenderer.setGoal", x, y);
        }

        public static async Task SetLightRadiusAsync(IJSRuntime js, int radius)
        {
            await js.InvokeVoidAsync("setPlayerLightRadius", radius);
        }

        // ================================
        // 🔊 Audio Interop
        // ================================
        public static async Task InitAudioAsync(IJSRuntime js)
        {
            await js.InvokeVoidAsync("AudioManager.init");
        }

        public static async Task PlayBackgroundMusicAsync(IJSRuntime js, string? track = null)
        {
            if (!string.IsNullOrEmpty(track))
                await js.InvokeVoidAsync("AudioManager.playBackgroundMusic", track);
            else
                await js.InvokeVoidAsync("AudioManager.playBackgroundMusic");
        }

        public static async Task PauseBackgroundMusicAsync(IJSRuntime js)
        {
            await js.InvokeVoidAsync("AudioManager.pauseBackgroundMusic");
        }

        public static async Task ResumeBackgroundMusicAsync(IJSRuntime js)
        {
            await js.InvokeVoidAsync("AudioManager.resumeBackgroundMusic");
        }

        public static async Task StopBackgroundMusicAsync(IJSRuntime js)
        {
            await js.InvokeVoidAsync("AudioManager.stopBackgroundMusic");
        }

        public static async Task NextBackgroundTrackAsync(IJSRuntime js)
        {
            await js.InvokeVoidAsync("AudioManager.nextTrack");
        }

        public static async Task SetBackgroundVolumeAsync(IJSRuntime js, double volume)
        {
            await js.InvokeVoidAsync("AudioManager.setVolume", volume);
        }

        public static async Task PlaySoundEffectAsync(IJSRuntime js, string filePath)
        {
            await js.InvokeVoidAsync("AudioManager.playEffect", filePath);
        }
    }
}