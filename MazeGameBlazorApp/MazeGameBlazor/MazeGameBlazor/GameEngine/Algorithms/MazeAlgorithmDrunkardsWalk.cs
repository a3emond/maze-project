using System.Collections.Generic;
using System.Linq;

namespace MazeGameBlazor.GameEngine.Algorithms
{
    public class MazeAlgorithmDrunkardsWalk : IMazeAlgorithm
    {
        public void Generate(Maze maze)
        {
            Random rand = new Random();
            int startX = maze.Width / 2;
            int startY = maze.Height / 2;
            int steps = (maze.Width * maze.Height) * 3;

            int x = startX;
            int y = startY;
            maze.Grid[x, y] = (int)TileType.Floor_Center;

            for (int i = 0; i < steps; i++)
            {
                int dir = rand.Next(4);
                switch (dir)
                {
                    case 0: if (y > 1) y--; break; // Up
                    case 1: if (y < maze.Height - 2) y++; break; // Down
                    case 2: if (x > 1) x--; break; // Left
                    case 3: if (x < maze.Width - 2) x++; break; // Right
                }

                maze.Grid[x, y] = (int)TileType.Floor_Center;
            }

            // Ensure all regions are connected
            ConnectDisconnectedCaves(maze);
        }

        private void ConnectDisconnectedCaves(Maze maze)
        {
            Dictionary<(int, int), int> regions = new();
            int regionId = 0;

            // Step 1: Identify all separate regions using flood fill
            for (int y = 1; y < maze.Height - 1; y++)
            {
                for (int x = 1; x < maze.Width - 1; x++)
                {
                    if (maze.Grid[x, y] == (int)TileType.Floor_Center && !regions.ContainsKey((x, y)))
                    {
                        MazeUtils.FloodFill(maze, x, y, regionId, regions);
                        regionId++;
                    }
                }
            }

            if (regionId <= 1) return; // Already fully connected

            // Step 2: Find the center of each region
            List<(int x, int y, int region)> regionCenters = new();
            for (int r = 0; r < regionId; r++)
            {
                var regionTiles = regions.Where(kvp => kvp.Value == r).Select(kvp => kvp.Key).ToList();
                var center = regionTiles[regionTiles.Count / 2]; // Approximate center of the region
                regionCenters.Add((center.Item1, center.Item2, r));
            }

            // Step 3: Connect regions using tunnels
            for (int i = 0; i < regionCenters.Count - 1; i++)
            {
                var start = regionCenters[i];
                var end = regionCenters[i + 1];

                MazeUtils.CarvePath(maze, start.x, start.y, end.x, end.y);
            }
        }

    }
}
