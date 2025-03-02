namespace MazeGameBlazor.GameEngine.Algorithms
{
    public  class MazeAlgorithmPrims : IMazeAlgorithm
    {
        public void Generate(Maze maze)
        {
            Random rand = new Random();
            List<(int, int)> walls = new();

            int startX = rand.Next(1, maze.Width - 1);
            int startY = rand.Next(1, maze.Height - 1);
            maze.Grid[startX, startY] = (int)TileType.Floor_Center;
            walls.AddRange(MazeUtils.GetNeighbors(maze, startX, startY));

            while (walls.Count > 0)
            {
                var (wx, wy) = walls[rand.Next(walls.Count)];
                walls.Remove((wx, wy));

                var adjacentFloors = MazeUtils.GetNeighbors(maze, wx, wy)
                    .Where(n => maze.Grid[n.Item1, n.Item2] == (int)TileType.Floor_Center)
                    .ToList();

                if (adjacentFloors.Count == 1) // Carve only if touching 1 floor tile
                {
                    maze.Grid[wx, wy] = (int)TileType.Floor_Center;
                    walls.AddRange(MazeUtils.GetNeighbors(maze, wx, wy)
                        .Where(n => maze.Grid[n.Item1, n.Item2] == (int)TileType.Empty_Black));
                }
            }
        }

        
    }
}