namespace MazeGameBlazor.GameEngine.Algorithms
{
    public class RecursiveBacktrackingMaze : IMazeAlgorithm
    {
        private Random rand = new Random();

        public void Generate(Maze maze)
        {
            Console.WriteLine("Recursive Backtracking Started");
            bool[,] visited = new bool[maze.Width, maze.Height];

            // Start at a random odd position inside the logical grid
            int logicalWidth = maze.Width / 5;  // Convert back to logical size
            int logicalHeight = maze.Height / 5;

            int startX = 1 + rand.Next((logicalWidth - 2) / 2) * 2;
            int startY = 1 + rand.Next((logicalHeight - 2) / 2) * 2;

            GenerateDFS(maze, startX, startY, visited);

        }

        private void GenerateDFS(Maze maze, int cx, int cy, bool[,] visited)
        {
            visited[cx, cy] = true;

            int realCx = cx * 5;
            int realCy = cy * 5;

            maze.Grid[realCx, realCy] = (int)TileType.Floor_Center; // Correctly map logical to real coordinates

            foreach (var (dx, dy) in MazeUtils.RandomizedDirections())
            {
                int nx = cx + dx * 2;  // Move two logical cells in the direction
                int ny = cy + dy * 2;
                int realNx = nx * 5;
                int realNy = ny * 5;

                if (MazeUtils.IsWithinBounds(maze, realNx, realNy) && !visited[nx, ny])
                {
                    // Remove walls between logical cells, maintaining 3-tile-wide paths
                    MazeUtils.CarvePath(maze, realCx, realCy, realCx + dx * 5, realCy + dy * 5);
                    MazeUtils.CarvePath(maze, realCx + dx * 5, realCy + dy * 5, realNx, realNy);

                    GenerateDFS(maze, nx, ny, visited);
                }
            }
        }


    }
}