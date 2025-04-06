using MazeGameBlazor.GameEngine.Models;

namespace MazeGameBlazor.GameEngine.Services;

public static class TileProcessor
{
    public static void Process(Maze maze)
    {
        // get walkable tiles before processing (at this point there is only -1 and 0 in the grid)
        IdentifyWalkableTiles(maze);

        // Process the tiles
        ProcessWalls(maze);
        ProcessCorners(maze);
        ProcessInnerCorners(maze);
        ProcessWallTypes(maze);
        ProcessFloorTiles(maze);
    }

    private static void ProcessWalls(Maze maze)
    {
        for (var y = 0; y < maze.Height; y++)
        for (var x = 0; x < maze.Width; x++)
            if (maze.Grid[x, y] == -1) // Check only empty spaces (-1)
            {
                var hasAdjacentFloor =
                    y > 0 && maze.Grid[x, y - 1] == 0 || // Top
                    y < maze.Height - 1 && maze.Grid[x, y + 1] == 0 || // Bottom
                    x > 0 && maze.Grid[x - 1, y] == 0 || // Left
                    x < maze.Width - 1 && maze.Grid[x + 1, y] == 0; // Right

                if (hasAdjacentFloor) maze.Grid[x, y] = 1; // Mark it as a wall
            }
    }

    private static void ProcessCorners(Maze maze)
    {
        Dictionary<(int x, int y), TileType> cornerUpdates = new();

        for (var y = 1; y < maze.Height - 1; y++) // Avoid out-of-bounds
        for (var x = 1; x < maze.Width - 1; x++)
            if (maze.Grid[x, y] == -1) // Only process empty spaces (-1)
            {
                // Cardinal walls
                var top = maze.Grid[x, y - 1] == 1;
                var bottom = maze.Grid[x, y + 1] == 1;
                var left = maze.Grid[x - 1, y] == 1;
                var right = maze.Grid[x + 1, y] == 1;

                // Diagonal floors (valid open spaces)
                var topLeftDiag = maze.Grid[x - 1, y - 1] == 0;
                var topRightDiag = maze.Grid[x + 1, y - 1] == 0;
                var bottomLeftDiag = maze.Grid[x - 1, y + 1] == 0;
                var bottomRightDiag = maze.Grid[x + 1, y + 1] == 0;

                // **Strict Corner Detection: At least 2 walls & 1 diagonal floor**
                var wallCount = (top ? 1 : 0) + (bottom ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);
                var floorCount = (topLeftDiag ? 1 : 0) + (topRightDiag ? 1 : 0) + (bottomLeftDiag ? 1 : 0) +
                                 (bottomRightDiag ? 1 : 0);

                // Ensure at least 2 walls and exactly 1 floor in diagonal
                if (wallCount >= 2 && floorCount == 1)
                {
                    if (bottomRightDiag)
                        cornerUpdates[(x, y)] = TileType.WallCornerTopLeft;
                    else if (bottomLeftDiag)
                        cornerUpdates[(x, y)] = TileType.WallCornerTopRight;
                    else if (topRightDiag)
                        cornerUpdates[(x, y)] = TileType.WallCornerBottomLeft;
                    else if (topLeftDiag)
                        cornerUpdates[(x, y)] = TileType.WallCornerBottomRight;
                }
            }

        // Apply corner updates after processing
        foreach (var (pos, type) in cornerUpdates) maze.Grid[pos.x, pos.y] = (int)type;
    }

    private static void ProcessInnerCorners(Maze maze)
    {
        for (var y = 1; y < maze.Height - 1; y++) // Avoid out-of-bounds
        for (var x = 1; x < maze.Width - 1; x++)
            if (maze.Grid[x, y] == 1) // Check only walls
            {
                var top = maze.Grid[x, y - 1] == 0; // Floor above
                var bottom = maze.Grid[x, y + 1] == 0; // Floor below
                var left = maze.Grid[x - 1, y] == 0; // Floor left
                var right = maze.Grid[x + 1, y] == 0; // Floor right

                // **Check for two perpendicular floor tiles**
                if (top && left) maze.Grid[x, y] = (int)TileType.WallInnerCornerBottomRight;
                else if (top && right) maze.Grid[x, y] = (int)TileType.WallInnerCornerBottomLeft;
                else if (bottom && left) maze.Grid[x, y] = (int)TileType.WallTop;
                else if (bottom && right) maze.Grid[x, y] = (int)TileType.WallTop;
            }
    }

    private static void ProcessWallTypes(Maze maze)
    {
        for (var y = 1; y < maze.Height - 1; y++) // Avoid out-of-bounds
        for (var x = 1; x < maze.Width - 1; x++)
            if (maze.Grid[x, y] == 1) // Only process walls (1)
            {
                var hasFloorAbove = maze.Grid[x, y - 1] == 0; // Floor above
                var hasFloorBelow = maze.Grid[x, y + 1] == 0; // Floor below
                var hasFloorLeft = maze.Grid[x - 1, y] == 0; // Floor left
                var hasFloorRight = maze.Grid[x + 1, y] == 0; // Floor right

                // **Determine Wall Type Based on Adjacent Floor**
                if (hasFloorBelow)
                    maze.Grid[x, y] = (int)TileType.WallTop; // Wall with floor **below** is a TOP wall
                else if (hasFloorAbove)
                    maze.Grid[x, y] = (int)TileType.WallBottom; // Wall with floor **above** is a BOTTOM wall
                else if (hasFloorRight)
                    maze.Grid[x, y] = (int)TileType.WallLeft; // Wall with floor **to the right** is a LEFT wall
                else if (hasFloorLeft)
                    maze.Grid[x, y] = (int)TileType.WallRight; // Wall with floor **to the left** is a RIGHT wall
            }
    }

    private static void ProcessFloorTiles(Maze maze)
    {
        Dictionary<TileType, List<(int x, int y)>> floorCategories = new();

        // Step 1: Scan the grid and classify floor tiles
        for (var y = 1; y < maze.Height - 1; y++) // Avoid out-of-bounds
        for (var x = 1; x < maze.Width - 1; x++)
            if (maze.Grid[x, y] == (int)TileType.FloorCenter) // Only process floor tiles
            {
                var top = maze.Grid[x, y - 1] == (int)TileType.FloorCenter;
                var bottom = maze.Grid[x, y + 1] == (int)TileType.FloorCenter;
                var left = maze.Grid[x - 1, y] == (int)TileType.FloorCenter;
                var right = maze.Grid[x + 1, y] == (int)TileType.FloorCenter;

                var tileType = TileType.FloorCenter;

                // **Determine Floor Type**
                if (!top && bottom && left && right) tileType = TileType.FloorTop;
                else if (top && !bottom && left && right) tileType = TileType.FloorBottom;
                else if (top && bottom && !left && right) tileType = TileType.FloorLeft;
                else if (top && bottom && left && !right) tileType = TileType.FloorRight;

                // **Corner Detection**
                else if (!top && !left) tileType = TileType.FloorCornerTopLeft;
                else if (!top && !right) tileType = TileType.FloorCornerTopRight;
                else if (!bottom && !left) tileType = TileType.FloorCornerBottomLeft;
                else if (!bottom && !right) tileType = TileType.FloorCornerBottomRight;

                // Add to dictionary
                if (!floorCategories.ContainsKey(tileType))
                    floorCategories[tileType] = new List<(int x, int y)>();

                floorCategories[tileType].Add((x, y));
            }

        // Step 2: Process each category of floors & update the grid
        foreach (var category in floorCategories)
        foreach (var (x, y) in category.Value)
            maze.Grid[x, y] = (int)category.Key; // Assign correct tile type
    }

    private static void IdentifyWalkableTiles(Maze maze)
    {
        maze.WalkableTiles.Clear();
        for (var y = 0; y < maze.Height; y++)
        for (var x = 0; x < maze.Width; x++)
            if (maze.Grid[x, y] == (int)TileType.FloorCenter)
                maze.WalkableTiles.Add((x, y));
    }

    public static string GetTileSprite(Maze maze, int x, int y)
    {
        var tile = (TileType)maze.Grid[x, y];
        return tile.GetTileSprite(); // Call the extension method
    }
}