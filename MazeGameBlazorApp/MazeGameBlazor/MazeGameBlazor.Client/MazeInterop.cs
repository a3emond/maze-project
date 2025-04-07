using Microsoft.JSInterop;

// THIS IS NOT USED... need to migrate game.razor to web assembly before

namespace MazeGameBlazor.Client
{
    public enum MazeRendererType
    {
        Canvas2D,
        WebGL
    }

    public static class MazeInterop
    {
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
    }


}
