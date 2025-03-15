namespace MazeGameBlazor.GameEngine.GeneratingAlgorithms;

public class MazeAlgorithmDrunkardsWalk : IMazeAlgorithm
{
    /// <summary>
    /// Generates a maze using the Drunkard's Walk algorithm.
    /// </summary>
    /// <param name="maze">The maze to generate.</param>
    public void Generate(Maze maze)
    {
        var rand = new Random();
        var startX = maze.Width / 2; // center of the maze
        var startY = maze.Height / 2; // center of the maze
        var steps = maze.Width * maze.Height * 10; // number of steps to take (3x the area of the maze)

        var x = startX;
        var y = startY;
        maze.Grid[x, y] = (int)TileType.FloorCenter;

        // Track visited cells
        var visited = new bool[maze.Width, maze.Height];
        visited[x, y] = true;

        // Perform random walk to carve out the maze
        for (var i = 0; i < steps; i++)
        {
            var dir = rand.Next(4);
            var newX = x;
            var newY = y;

            switch (dir)
            {
                case 0:
                    if (y > 1) newY--;
                    break; // Up
                case 1:
                    if (y < maze.Height - 2) newY++;
                    break; // Down
                case 2:
                    if (x > 1) newX--;
                    break; // Left
                case 3:
                    if (x < maze.Width - 2) newX++;
                    break; // Right
            }

            // Only move to the new position if it has not been visited
            if (!visited[newX, newY])
            {
                // Carve a path between the current position and the new position
                MazeUtils.CarvePath(maze, x, y, newX, newY);

                x = newX;
                y = newY;
                visited[x, y] = true;
            }
        }

        // Ensure all regions are connected
        ConnectDisconnectedCaves(maze);
    }

    /// <summary>
    /// Connects disconnected regions of the maze to ensure all areas are reachable.
    /// </summary>
    /// <param name="maze">The maze to connect.</param>
    private void ConnectDisconnectedCaves(Maze maze)
    {
        Dictionary<(int, int), int> regions = new();
        var regionId = 0;

        // Step 1: Identify all separate regions using flood fill
        for (var y = 1; y < maze.Height - 1; y++)
            for (var x = 1; x < maze.Width - 1; x++)
                if (maze.Grid[x, y] == (int)TileType.FloorCenter && !regions.ContainsKey((x, y)))
                {
                    MazeUtils.FloodFill(maze, x, y, regionId, regions);
                    regionId++;
                }

        if (regionId <= 1) return; // Already fully connected

        // Step 2: Find the center of each region
        List<(int x, int y, int region)> regionCenters = new();
        for (var r = 0; r < regionId; r++)
        {
            var regionTiles = regions.Where(kvp => kvp.Value == r).Select(kvp => kvp.Key).ToList();
            var center = regionTiles[regionTiles.Count / 2]; // Approximate center of the region
            regionCenters.Add((center.Item1, center.Item2, r));
        }

        // Step 3: Connect regions using tunnels
        for (var i = 0; i < regionCenters.Count - 1; i++)
        {
            var start = regionCenters[i];
            var end = regionCenters[i + 1];

            MazeUtils.CarvePath(maze, start.x, start.y, end.x, end.y);
        }
    }
}
