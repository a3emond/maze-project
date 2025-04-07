using MazeGameBlazor.Shared.GameEngine.Models;

namespace MazeGameBlazor.Shared.GameEngine.Utils.GeneratingAlgorithms;

public class RecursiveBacktrackingMaze : IMazeAlgorithm
{
    private readonly Random _rand = new();

    public void Generate(Maze maze)
    {
        Console.WriteLine("Recursive Backtracking Started");
        var visited = new bool[maze.Width, maze.Height];

        // Start at a random odd position inside the logical grid
        var logicalWidth = maze.Width / 5; // Convert back to logical size
        var logicalHeight = maze.Height / 5;

        var startX = 1 + _rand.Next((logicalWidth - 2) / 2) * 2;
        var startY = 1 + _rand.Next((logicalHeight - 2) / 2) * 2;

        GenerateDfs(maze, startX, startY, visited);
    }

    private void GenerateDfs(Maze maze, int cx, int cy, bool[,] visited)
    {
        visited[cx, cy] = true;

        var realCx = cx * 5;
        var realCy = cy * 5;

        maze.Grid[realCx, realCy] = (int)TileType.FloorCenter; // Correctly map logical to real coordinates

        foreach (var (dx, dy) in MazeUtils.RandomizedDirections())
        {
            var nx = cx + dx * 2; // Move two logical cells in the direction
            var ny = cy + dy * 2;
            var realNx = nx * 5;
            var realNy = ny * 5;

            if (MazeUtils.IsWithinBounds(maze, realNx, realNy) && !visited[nx, ny])
            {
                // Remove walls between logical cells, maintaining 3-tile-wide paths
                MazeUtils.CarvePath(maze, realCx, realCy, realCx + dx * 5, realCy + dy * 5);
                MazeUtils.CarvePath(maze, realCx + dx * 5, realCy + dy * 5, realNx, realNy);

                GenerateDfs(maze, nx, ny, visited);
            }
        }
    }
}