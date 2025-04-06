using MazeGameBlazor.GameEngine.GeneratingAlgorithms;
using MazeGameBlazor.GameEngine.Models;

namespace MazeGameBlazor.GameEngine.Services
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
