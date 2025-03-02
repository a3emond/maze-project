using MazeGameBlazor.GameEngine.Algorithms;


namespace MazeGameBlazor.GameEngine
{
    public class MazeGenerator
    {
        private const int CellSize = 5; // 3-floor corridor + 1-wall padding per side
        private const int Width = 40; // Logical maze grid (cells)
        private const int Height = 40;
        private Random rand = new Random();

        private Maze maze;

        public MazeGenerator()
        {
            maze = new Maze(Width, Height, CellSize);
        }

        public Maze GenerateMaze(MazeAlgorithmEnum algorithm)
        {
            // Step 1: Fill grid with empty black tiles
            for (int y = 0; y < maze.Height; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    maze.Grid[x, y] = (int)TileType.Empty_Black;
                }
            }

            // Step 2: Generate the maze structure using the selected algorithm
            switch (algorithm)
            {
                case MazeAlgorithmEnum.RecursiveBacktracking:
                    new RecursiveBacktrackingMaze().Generate(maze);
                    break;
                case MazeAlgorithmEnum.Prims:
                    new MazeAlgorithmPrims().Generate(maze);
                    break;
                case MazeAlgorithmEnum.DrunkardsWalk:
                    new MazeAlgorithmDrunkardsWalk().Generate(maze);
                    break;
                case MazeAlgorithmEnum.BspDungeon:
                    new MazeAlgorithmBsp().Generate(maze);
                    break;
            }


            TileProcessor.Process(maze);

            maze.InitializeStartAndGoal();


            return maze; // Return the generated maze
        }
    }
}
