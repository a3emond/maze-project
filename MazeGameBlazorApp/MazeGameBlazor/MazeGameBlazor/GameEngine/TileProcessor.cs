using System.Runtime.CompilerServices;

namespace MazeGameBlazor.GameEngine
{
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
            for (int y = 0; y < maze.Height; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    if (maze.Grid[x, y] == -1) // Check only empty spaces (-1)
                    {
                        bool hasAdjacentFloor =
                            (y > 0 && maze.Grid[x, y - 1] == 0) ||     // Top
                            (y < maze.Height - 1 && maze.Grid[x, y + 1] == 0) || // Bottom
                            (x > 0 && maze.Grid[x - 1, y] == 0) ||     // Left
                            (x < maze.Width - 1 && maze.Grid[x + 1, y] == 0); // Right

                        if (hasAdjacentFloor)
                        {
                            maze.Grid[x, y] = 1; // Mark it as a wall
                        }
                    }
                }
            }
        }

        private static void ProcessCorners(Maze maze)
        {
            Dictionary<(int x, int y), TileType> cornerUpdates = new();

            for (int y = 1; y < maze.Height - 1; y++) // Avoid out-of-bounds
            {
                for (int x = 1; x < maze.Width - 1; x++)
                {
                    if (maze.Grid[x, y] == -1) // Only process empty spaces (-1)
                    {
                        // Cardinal walls
                        bool top = (maze.Grid[x, y - 1] == 1);
                        bool bottom = (maze.Grid[x, y + 1] == 1);
                        bool left = (maze.Grid[x - 1, y] == 1);
                        bool right = (maze.Grid[x + 1, y] == 1);

                        // Diagonal floors (valid open spaces)
                        bool topLeftDiag = (maze.Grid[x - 1, y - 1] == 0);
                        bool topRightDiag = (maze.Grid[x + 1, y - 1] == 0);
                        bool bottomLeftDiag = (maze.Grid[x - 1, y + 1] == 0);
                        bool bottomRightDiag = (maze.Grid[x + 1, y + 1] == 0);

                        // **Strict Corner Detection: At least 2 walls & 1 diagonal floor**
                        int wallCount = (top ? 1 : 0) + (bottom ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);
                        int floorCount = (topLeftDiag ? 1 : 0) + (topRightDiag ? 1 : 0) + (bottomLeftDiag ? 1 : 0) + (bottomRightDiag ? 1 : 0);

                        // Ensure at least 2 walls and exactly 1 floor in diagonal
                        if (wallCount >= 2 && floorCount == 1)
                        {
                            if (bottomRightDiag)
                                cornerUpdates[(x, y)] = TileType.Wall_Corner_TopLeft;
                            else if (bottomLeftDiag)
                                cornerUpdates[(x, y)] = TileType.Wall_Corner_TopRight;
                            else if (topRightDiag)
                                cornerUpdates[(x, y)] = TileType.Wall_Corner_BottomLeft;
                            else if (topLeftDiag)
                                cornerUpdates[(x, y)] = TileType.Wall_Corner_BottomRight;
                        }
                    }
                }
            }

            // Apply corner updates after processing
            foreach (var (pos, type) in cornerUpdates)
            {
                maze.Grid[pos.x, pos.y] = (int)type;
            }
        }

        private static void ProcessInnerCorners(Maze maze)
        {
            for (int y = 1; y < maze.Height - 1; y++) // Avoid out-of-bounds
            {
                for (int x = 1; x < maze.Width - 1; x++)
                {
                    if (maze.Grid[x, y] == 1) // Check only walls
                    {
                        bool top = (maze.Grid[x, y - 1] == 0);     // Floor above
                        bool bottom = (maze.Grid[x, y + 1] == 0);  // Floor below
                        bool left = (maze.Grid[x - 1, y] == 0);    // Floor left
                        bool right = (maze.Grid[x + 1, y] == 0);   // Floor right

                        // **Check for two perpendicular floor tiles**
                        if (top && left) maze.Grid[x, y] = (int)TileType.Wall_InnerCorner_BottomRight;
                        else if (top && right) maze.Grid[x, y] = (int)TileType.Wall_InnerCorner_BottomLeft;
                        else if (bottom && left) maze.Grid[x, y] = (int)TileType.Wall_Top;
                        else if (bottom && right) maze.Grid[x, y] = (int)TileType.Wall_Top;
                    }
                }
            }
        }

        private static void ProcessWallTypes(Maze maze)
        {
            for (int y = 1; y < maze.Height - 1; y++) // Avoid out-of-bounds
            {
                for (int x = 1; x < maze.Width - 1; x++)
                {
                    if (maze.Grid[x, y] == 1) // Only process walls (1)
                    {
                        bool hasFloorAbove = (maze.Grid[x, y - 1] == 0);   // Floor above
                        bool hasFloorBelow = (maze.Grid[x, y + 1] == 0);   // Floor below
                        bool hasFloorLeft = (maze.Grid[x - 1, y] == 0);    // Floor left
                        bool hasFloorRight = (maze.Grid[x + 1, y] == 0);   // Floor right

                        // **Determine Wall Type Based on Adjacent Floor**
                        if (hasFloorBelow)
                            maze.Grid[x, y] = (int)TileType.Wall_Top;    // Wall with floor **below** is a TOP wall
                        else if (hasFloorAbove)
                            maze.Grid[x, y] = (int)TileType.Wall_Bottom; // Wall with floor **above** is a BOTTOM wall
                        else if (hasFloorRight)
                            maze.Grid[x, y] = (int)TileType.Wall_Left;   // Wall with floor **to the right** is a LEFT wall
                        else if (hasFloorLeft)
                            maze.Grid[x, y] = (int)TileType.Wall_Right;  // Wall with floor **to the left** is a RIGHT wall
                    }
                }
            }
        }

        private static void ProcessFloorTiles(Maze maze)
        {
            Dictionary<TileType, List<(int x, int y)>> floorCategories = new();

            // Step 1: Scan the grid and classify floor tiles
            for (int y = 1; y < maze.Height - 1; y++) // Avoid out-of-bounds
            {
                for (int x = 1; x < maze.Width - 1; x++)
                {
                    if (maze.Grid[x, y] == (int)TileType.Floor_Center) // Only process floor tiles
                    {
                        bool top = (maze.Grid[x, y - 1] == (int)TileType.Floor_Center);
                        bool bottom = (maze.Grid[x, y + 1] == (int)TileType.Floor_Center);
                        bool left = (maze.Grid[x - 1, y] == (int)TileType.Floor_Center);
                        bool right = (maze.Grid[x + 1, y] == (int)TileType.Floor_Center);

                        TileType tileType = TileType.Floor_Center;

                        // **Determine Floor Type**
                        if (!top && bottom && left && right) tileType = TileType.Floor_Top;
                        else if (top && !bottom && left && right) tileType = TileType.Floor_Bottom;
                        else if (top && bottom && !left && right) tileType = TileType.Floor_Left;
                        else if (top && bottom && left && !right) tileType = TileType.Floor_Right;

                        // **Corner Detection**
                        else if (!top && !left) tileType = TileType.Floor_Corner_TopLeft;
                        else if (!top && !right) tileType = TileType.Floor_Corner_TopRight;
                        else if (!bottom && !left) tileType = TileType.Floor_Corner_BottomLeft;
                        else if (!bottom && !right) tileType = TileType.Floor_Corner_BottomRight;

                        // Add to dictionary
                        if (!floorCategories.ContainsKey(tileType))
                            floorCategories[tileType] = new List<(int x, int y)>();

                        floorCategories[tileType].Add((x, y));
                    }
                }
            }

            // Step 2: Process each category of floors & update the grid
            foreach (var category in floorCategories)
            {
                foreach (var (x, y) in category.Value)
                {
                    maze.Grid[x, y] = (int)category.Key; // Assign correct tile type
                }
            }
        }
        private static void IdentifyWalkableTiles(Maze maze)
        {
            maze.WalkableTiles.Clear();
            for (int y = 0; y < maze.Height; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    if (maze.Grid[x, y] == (int)TileType.Floor_Center)
                    {
                        maze.WalkableTiles.Add((x, y));
                    }
                }
            }
        }

        public static string GetTileSprite(Maze maze, int x, int y)
        {
            TileType tile = (TileType)maze.Grid[x, y];
            return tile.GetTileSprite(); // Call the extension method
        }


    }

}
