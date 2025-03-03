namespace MazeGameBlazor.GameEngine;

public class Maze
{
    public Maze(int width, int height, int cellSize)
    {
        Width = width * cellSize;
        Height = height * cellSize;
        Grid = new int[Width, Height];
        WalkableTiles = new HashSet<(int x, int y)>();
        ItemGrid = new ItemGrid(width, height);
    }

    public int[,] Grid { get; }
    public int Width { get; }
    public int Height { get; }
    public HashSet<(int x, int y)> WalkableTiles { get; }

    public ItemGrid ItemGrid { get; }

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

    // Helper to check if an item is at a given position
    public bool HasItemAt(int x, int y)
    {
        return ItemGrid.GetItemAt(x, y) != null;
    }

    // Helper to spawn an item in a valid location
    public void SpawnItem(ItemName name, string sprite,bool walkable, bool interactable, bool collectible, ItemEffect effect)
    {
        var position = MazeUtils.GetRandomWalkableTile(this);

        if (position.HasValue)
        {
            ItemGrid.AddItem(name, position.Value.x, position.Value.y, sprite, walkable ,interactable, collectible, effect);
        }
    }

    public bool IsWalkable(int x, int y)
    {
        return WalkableTiles.Contains((x, y));
    }

}
