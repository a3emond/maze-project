namespace MazeGameBlazor.GameEngine.Algorithms;

public class MazeAlgorithmPrims : IMazeAlgorithm
{
    public void Generate(Maze maze)
    {
        var rand = new Random();
        List<(int, int)> walls = new();

        var startX = rand.Next(1, maze.Width - 1);
        var startY = rand.Next(1, maze.Height - 1);
        maze.Grid[startX, startY] = (int)TileType.FloorCenter;
        walls.AddRange(MazeUtils.GetNeighbors(maze, startX, startY));

        while (walls.Count > 0)
        {
            var (wx, wy) = walls[rand.Next(walls.Count)];
            walls.Remove((wx, wy));

            var adjacentFloors = MazeUtils.GetNeighbors(maze, wx, wy)
                .Where(n => maze.Grid[n.Item1, n.Item2] == (int)TileType.FloorCenter)
                .ToList();

            if (adjacentFloors.Count == 1) // Carve only if touching 1 floor tile
            {
                maze.Grid[wx, wy] = (int)TileType.FloorCenter;
                walls.AddRange(MazeUtils.GetNeighbors(maze, wx, wy)
                    .Where(n => maze.Grid[n.Item1, n.Item2] == (int)TileType.EmptyBlack));
            }
        }
    }
}