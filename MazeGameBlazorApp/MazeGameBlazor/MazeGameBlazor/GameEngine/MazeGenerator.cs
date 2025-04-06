using MazeGameBlazor.GameEngine.GeneratingAlgorithms;
using System.Text;

namespace MazeGameBlazor.GameEngine;

public class MazeGenerator
{
    private const int CellSize = 5; // 3-floor corridor + 1-wall padding per side
    private const int Width = 40; // Logical maze grid (cells)
    private const int Height = 40;

    private readonly Maze _maze;
    private Random _rand = new();

    public MazeGenerator()
    {
        _maze = new Maze(Width, Height, CellSize);
    }

    public Maze GenerateMaze(MazeAlgorithmType algorithm)
    {
        // Step 1: Fill grid with empty black tiles
        for (var y = 0; y < _maze.Height; y++)
        for (var x = 0; x < _maze.Width; x++)
            _maze.Grid[x, y] = (int)TileType.EmptyBlack;

        // Step 2: Generate the maze structure using the selected algorithm
        switch (algorithm)
        {
            case MazeAlgorithmType.RecursiveBacktracking:
                new RecursiveBacktrackingMaze().Generate(_maze);
                //print mazegrid
                for (var y = 0; y < _maze.Height; y++)
                {
                    for (var x = 0; x < _maze.Width; x++)
                    {
                        int value = _maze.Grid[x, y];
                        Console.Write(value == -1 ? 1 : value);
                    }
                    Console.WriteLine(); // new line after each row
                }


                break;
            case MazeAlgorithmType.Prims:
                new MazeAlgorithmPrims().Generate(_maze);
                break;
            case MazeAlgorithmType.DrunkardsWalk:
                new MazeAlgorithmDrunkardsWalk().Generate(_maze);
                break;
            case MazeAlgorithmType.BspDungeon:
                new MazeAlgorithmBsp().Generate(_maze);
                break;
        }


        TileProcessor.Process(_maze);

        _maze.InitializeStartAndGoal();


        return _maze; // Return the generated maze
    }
}