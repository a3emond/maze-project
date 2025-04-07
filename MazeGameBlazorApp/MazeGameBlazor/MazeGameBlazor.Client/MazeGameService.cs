using MazeGameBlazor.Shared.GameEngine.Models;
using MazeGameBlazor.Shared.GameEngine.Services;
using MazeGameBlazor.Shared.GameEngine.Utils.GeneratingAlgorithms;

namespace MazeGameBlazor.Client
{
    public class MazeGameService
    {
        private readonly MazeGenerator _generator = new();

        public Maze GenerateMaze(MazeAlgorithmType algorithm)
        {
            var maze = _generator.GenerateMaze(algorithm);
            maze.ItemGrid.GenerateItems(maze);
            return maze;
        }

        public string[,] GenerateSpriteGrid(Maze maze)
        {
            var grid = new string[maze.Width, maze.Height];
            for (int y = 0; y < maze.Height; y++)
            for (int x = 0; x < maze.Width; x++)
                grid[y, x] = TileProcessor.GetTileSprite(maze, x, y);
            return grid;
        }
    }

}
