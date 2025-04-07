using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using MazeGameBlazor.Shared.GameEngine.Models;
using MazeGameBlazor.Shared.GameEngine.Services;

namespace MazeGameBlazor.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Configure HttpClient for API calls (e.g., to the server project)
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

            // Register game services (must be browser-safe!)
            builder.Services.AddScoped<GameState>();
            builder.Services.AddScoped<MazeInputManager>();
            builder.Services.AddScoped<MazeGameService>();
            builder.Services.AddScoped<MazeGameManager>();

            await builder.Build().RunAsync();
        }
    }
}