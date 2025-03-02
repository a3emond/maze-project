namespace MazeGameBlazor.GameEngine;

public class Maze
{
    public Maze(int width, int height, int cellSize)
    {
        Width = width * cellSize;
        Height = height * cellSize;
        Grid = new int[Width, Height];
        WalkableTiles = new HashSet<(int x, int y)>();
    }

    public int[,] Grid { get; }
    public int Width { get; }
    public int Height { get; }
    public HashSet<(int x, int y)> WalkableTiles { get; }

    public (int x, int y) StartPosition { get; private set; }
    public (int x, int y) GoalPosition { get; private set; }

    public void SetStartAndGoal((int, int) start, (int, int) goal)
    {
        StartPosition = start;
        GoalPosition = goal;
        Grid[start.Item1, start.Item2] = (int)TileType.Start;
        Grid[goal.Item1, goal.Item2] = (int)TileType.Goal;
    }


    public void InitializeStartAndGoal()
    {
        var start = MazeUtils.FindStartPosition(this);
        var goal = MazeUtils.FindGoalPosition(this, start);

        SetStartAndGoal(start, goal);
    }


    public bool IsWalkable(int x, int y)
    {
        return WalkableTiles.Contains((x, y));
    }

    // TODO: Implement display method for debugging (grid and walkable tiles)
}