using MazeGameBlazor.Shared.GameEngine.Models;

namespace MazeGameBlazor.Shared.GameEngine.Utils;

public static class MazeUtils
{
    private static readonly Random Rand = new();

    /// <summary>
    ///     Returns a randomized list of cardinal directions (Up, Down, Left, Right).
    ///     Useful for randomized DFS or Prim's algorithm.
    /// </summary>
    public static List<(int, int)> RandomizedDirections()
    {
        List<(int, int)> directions = new() { (0, -1), (0, 1), (-1, 0), (1, 0) };

        // Fisher-Yates Shuffle for efficient in-place randomization
        for (var i = directions.Count - 1; i > 0; i--)
        {
            var j = Rand.Next(i + 1);
            (directions[i], directions[j]) = (directions[j], directions[i]); // Swap
        }

        return directions;
    }

    /// <summary>
    ///     Checks if a given coordinate is within maze bounds.
    /// </summary>
    public static bool IsWithinBounds(Maze maze, int x, int y)
    {
        return x > 0 && y > 0 && x < maze.Width - 1 && y < maze.Height - 1;
    }

    /// <summary>
    ///     Gets all valid neighbors of a given cell in the maze.
    /// </summary>
    public static List<(int x, int y)> GetNeighbors(Maze maze, int x, int y)
    {
        List<(int x, int y)> neighbors = new();
        if (x > 1) neighbors.Add((x - 1, y)); // Left
        if (x < maze.Width - 2) neighbors.Add((x + 1, y)); // Right
        if (y > 1) neighbors.Add((x, y - 1)); // Top
        if (y < maze.Height - 2) neighbors.Add((x, y + 1)); // Bottom
        return neighbors;
    }

    /// <summary>
    ///     Carves a path between two cells, ensuring proper passage width.
    /// </summary>
    public static void CarvePath(Maze maze, int cx, int cy, int nx, int ny)
    {
        var x1 = cx;
        var y1 = cy;
        var x2 = nx;
        var y2 = ny;

        // Ensure starting and ending points are clear
        for (var i = -1; i <= 1; i++) // Creates a 3x3 walkable area
        for (var j = -1; j <= 1; j++)
        {
            var clearX1 = x1 + i;
            var clearY1 = y1 + j;
            var clearX2 = x2 + i;
            var clearY2 = y2 + j;

            if (IsWithinBounds(maze, clearX1, clearY1))
                maze.Grid[clearX1, clearY1] = (int)TileType.FloorCenter;

            if (IsWithinBounds(maze, clearX2, clearY2))
                maze.Grid[clearX2, clearY2] = (int)TileType.FloorCenter;
        }

        // Carve middle path
        var midX = (x1 + x2) / 2;
        var midY = (y1 + y2) / 2;

        for (var i = -1; i <= 1; i++) // Ensures a 3-tile wide passage
        for (var j = -1; j <= 1; j++)
        {
            var newX = midX + i;
            var newY = midY + j;

            if (IsWithinBounds(maze, newX, newY))
                maze.Grid[newX, newY] = (int)TileType.FloorCenter;
        }
    }


    /// <summary>
    ///     Finds a suitable start position randomly from the walkable tiles.
    /// </summary>
    public static (int, int) FindStartPosition(Maze maze)
    {
        if (maze.WalkableTiles.Count == 0)
            throw new InvalidOperationException("No walkable tiles found in the maze.");

        var rand = new Random();
        return maze.WalkableTiles.ElementAt(rand.Next(maze.WalkableTiles.Count));
    }

    /// <summary>
    ///     Finds the farthest walkable tile from the start position.
    ///     Uses Manhattan distance (not pathfinding).
    /// </summary>
    public static (int, int) FindGoalPosition(Maze maze, (int, int) start)
    {
        var farthest = start;
        var maxDistance = 0;

        foreach (var (x, y) in maze.WalkableTiles)
        {
            var distance = Math.Abs(x - start.Item1) + Math.Abs(y - start.Item2);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthest = (x, y);
            }
        }

        return farthest;
    }

    /// <summary>
    ///     Performs a flood fill to label all connected regions.
    ///     Used to detect disconnected areas in mazes like Drunkard’s Walk.
    /// </summary>
    public static void FloodFill(Maze maze, int x, int y, int regionId, Dictionary<(int, int), int> regions)
    {
        Queue<(int, int)> queue = new();
        queue.Enqueue((x, y));

        while (queue.Count > 0)
        {
            var (cx, cy) = queue.Dequeue();
            if (regions.ContainsKey((cx, cy))) continue;

            regions[(cx, cy)] = regionId;

            foreach (var (nx, ny) in GetNeighbors(maze, cx, cy))
                if (maze.Grid[nx, ny] == (int)TileType.FloorCenter && !regions.ContainsKey((nx, ny)))
                    queue.Enqueue((nx, ny));
        }
    }

    // Items layer utils
    public static (int x, int y)? GetRandomWalkableTile(Maze maze)
    {
        var validTiles = maze.WalkableTiles
            .Where(t => !maze.HasItemAt(t.x, t.y) && t != maze.StartPosition && t != maze.GoalPosition)
        .ToList();

        return validTiles.Count > 0 ? validTiles[Rand.Next(validTiles.Count)] : null;
    }

    private static readonly Random Random = new();

    public static List<(int x, int y)> ShuffleWalkableTiles(Maze maze)
    {
        return maze.WalkableTiles.OrderBy(_ => Random.Next()).ToList();
    }
}