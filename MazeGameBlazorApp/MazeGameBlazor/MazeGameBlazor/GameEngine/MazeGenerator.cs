using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MazeGameBlazor.GameEngine
{
    public class MazeGenerator
    {
        private const int CellSize = 5; // 3-floor corridor + 1-wall padding per side
        private const int Width = 60; // Logical maze grid (cells)
        private const int Height = 60;
        private const int RoomMaxSize = 6;
        private const int RoomMinSize = 3;
        private const int MaxRooms = 3;
        private Random rand = new Random();

        public int[,] mazeGrid = new int[Width * CellSize, Height * CellSize];
        private (int, int) startPosition;
        private (int, int) goalPosition;

        public enum MazeAlgorithm
        {
            RecursiveBacktracking,
            Prims
        }

        public void GenerateMaze(MazeAlgorithm algorithm)
        {
            // Step 1: Fill grid with empty black tiles
            for (int y = 0; y < Height * CellSize; y++)
            {
                for (int x = 0; x < Width * CellSize; x++)
                {
                    mazeGrid[x, y] = (int)TileType.Empty_Black;
                }
            }

            // Step 2: Generate the maze structure
            switch (algorithm)
            {
                case MazeAlgorithm.RecursiveBacktracking:
                    GenerateMazeDFS(1, 1, new bool[Width, Height]);
                    break;
                case MazeAlgorithm.Prims:
                    GenerateMazePrims();
                    break;
            }

            // Step 3: Add rooms
            //PlaceRooms();

            // Step 4: Set Start and Goal
            SetStartAndGoal();

            // Step 5: Process tiles for appropriate wall placement
            ProcessTileTypes();


            // Step 6: Print the maze to the console
            //PrintMaze();
        }

        private void GenerateMazeDFS(int cx, int cy, bool[,] visited)
        {
            visited[cx, cy] = true;

            foreach (var (dx, dy) in RandomizedDirections())
            {
                int nx = cx + dx;
                int ny = cy + dy;

                if (IsWithinBounds(nx, ny) && !visited[nx, ny])
                {
                    CarvePath(cx, cy, nx, ny);
                    GenerateMazeDFS(nx, ny, visited);
                }
            }
        }

        private void GenerateMazePrims()
        {
            // TODO: Implement Prim's Algorithm for wide maze generation
        }

        private void CarvePath(int cx, int cy, int nx, int ny)
        {
            int x1 = cx * CellSize;
            int y1 = cy * CellSize;
            int x2 = nx * CellSize;
            int y2 = ny * CellSize;

            for (int i = 1; i <= 3; i++)
            {
                for (int j = 1; j <= 3; j++)
                {
                    mazeGrid[x1 + i, y1 + j] = (int)TileType.Floor_Center;
                    mazeGrid[x2 + i, y2 + j] = (int)TileType.Floor_Center;
                }
            }

            int midX = (x1 + x2) / 2;
            int midY = (y1 + y2) / 2;
            for (int i = 1; i <= 3; i++)
            {
                for (int j = 1; j <= 3; j++)
                {
                    mazeGrid[midX + i, midY + j] = (int)TileType.Floor_Center;
                }
            }
        }

        private void PlaceRooms()
        {
            for (int i = 0; i < MaxRooms; i++)
            {
                int roomWidth = rand.Next(RoomMinSize, RoomMaxSize) * CellSize;
                int roomHeight = rand.Next(RoomMinSize, RoomMaxSize) * CellSize;
                int x = rand.Next(2, Width * CellSize - roomWidth - 2);
                int y = rand.Next(2, Height * CellSize - roomHeight - 2);

                for (int rx = 0; rx < roomWidth; rx++)
                {
                    for (int ry = 0; ry < roomHeight; ry++)
                    {
                        mazeGrid[x + rx, y + ry] = (int)TileType.Floor_Center;
                    }
                }
            }
        }

        private void SetStartAndGoal() // TODO: make it so the placement is randomized
        {
            startPosition = (1 * CellSize + 2, 1 * CellSize + 2);
            mazeGrid[startPosition.Item1, startPosition.Item2] = (int)TileType.Start;

            // Find the farthest floor tile for the goal
            goalPosition = FindFarthestPoint(startPosition);
            mazeGrid[goalPosition.Item1-1, goalPosition.Item2-1] = (int)TileType.Goal;
        }

        private (int, int) FindFarthestPoint((int, int) start)
        {

            // TODO: calculate the walk distance not the fly distance
            (int, int) farthest = start;
            int maxDistance = 0;
            for (int y = 0; y < Height * CellSize; y++)
            {
                for (int x = 0; x < Width * CellSize; x++)
                {
                    if (mazeGrid[x, y] == (int)TileType.Floor_Center)
                    {
                        int distance = Math.Abs(x - start.Item1) + Math.Abs(y - start.Item2);
                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            farthest = (x, y);
                        }
                    }
                }
            }
            return farthest;
        }
        // ------------------------------------------------------------------------------
        // tile processing---------------------------------------------------------------
        // ------------------------------------------------------------------------------
        private void ProcessWalls()
        {

            // TODO: while at it, make a list of all floor tiles surrounded by other floor tiles for item placement later

            for (int y = 0; y < Height * CellSize; y++)
            {
                for (int x = 0; x < Width * CellSize; x++)
                {
                    if (mazeGrid[x, y] == -1) // Check only empty spaces (-1)
                    {
                        bool hasAdjacentFloor =
                            (y > 0 && mazeGrid[x, y - 1] == 0) ||     // Top
                            (y < Height * CellSize - 1 && mazeGrid[x, y + 1] == 0) || // Bottom
                            (x > 0 && mazeGrid[x - 1, y] == 0) ||     // Left
                            (x < Width * CellSize - 1 && mazeGrid[x + 1, y] == 0); // Right

                        if (hasAdjacentFloor)
                        {
                            mazeGrid[x, y] = 1; // Mark it as a wall
                        }
                    }
                }
            }

        }
        private void ProcessCorners()
        {
            for (int y = 1; y < Height * CellSize - 1; y++) // Avoid out-of-bounds
            {
                for (int x = 1; x < Width * CellSize - 1; x++)
                {
                    if (mazeGrid[x, y] == -1) // Only process empty spaces (-1)
                    {
                        bool top = (mazeGrid[x, y - 1] == 1);
                        bool bottom = (mazeGrid[x, y + 1] == 1);
                        bool left = (mazeGrid[x - 1, y] == 1);
                        bool right = (mazeGrid[x + 1, y] == 1);

                        // **Check for two perpendicular walls (doesn't matter how many total)**
                        if ((top && left)) mazeGrid[x, y] = (int)TileType.Wall_Corner_BottomRight; // ╯
                        else if ((top && right)) mazeGrid[x, y] = (int)TileType.Wall_Corner_BottomLeft;  // ╰
                        else if ((bottom && left)) mazeGrid[x, y] = (int)TileType.Wall_Corner_TopRight;  // ╮
                        else if ((bottom && right)) mazeGrid[x, y] = (int)TileType.Wall_Corner_TopLeft;  // ╭
                    }
                }
            }
        }

        private void ProcessInnerCorners()
        {
            for (int y = 1; y < Height * CellSize - 1; y++) // Avoid out-of-bounds
            {
                for (int x = 1; x < Width * CellSize - 1; x++)
                {
                    if (mazeGrid[x, y] == 1) // Check only walls
                    {
                        bool top = (mazeGrid[x, y - 1] == 0);     // Floor above
                        bool bottom = (mazeGrid[x, y + 1] == 0);  // Floor below
                        bool left = (mazeGrid[x - 1, y] == 0);    // Floor left
                        bool right = (mazeGrid[x + 1, y] == 0);   // Floor right

                        // **Check for two perpendicular floor tiles**
                        if (top && left) mazeGrid[x, y] = (int)TileType.Wall_InnerCorner_TopLeft;  // ┌
                        else if (top && right) mazeGrid[x, y] = (int)TileType.Wall_InnerCorner_TopRight; // ┐
                        else if (bottom && left) mazeGrid[x, y] = (int)TileType.Wall_InnerCorner_BottomLeft; // └
                        else if (bottom && right) mazeGrid[x, y] = (int)TileType.Wall_InnerCorner_BottomRight; // ┘
                    }
                }
            }
        }

        private void ProcessWallTypes()
        {
            for (int y = 1; y < Height * CellSize - 1; y++) // Avoid out-of-bounds
            {
                for (int x = 1; x < Width * CellSize - 1; x++)
                {
                    if (mazeGrid[x, y] == 1) // Only process walls (1)
                    {
                        bool hasFloorAbove = (mazeGrid[x, y - 1] == 0);   // Floor above
                        bool hasFloorBelow = (mazeGrid[x, y + 1] == 0);   // Floor below
                        bool hasFloorLeft = (mazeGrid[x - 1, y] == 0);    // Floor left
                        bool hasFloorRight = (mazeGrid[x + 1, y] == 0);   // Floor right

                        // **Determine Wall Type Based on Adjacent Floor**
                        if (hasFloorBelow)
                            mazeGrid[x, y] = (int)TileType.Wall_Top;    // Wall with floor **below** is a TOP wall
                        else if (hasFloorAbove)
                            mazeGrid[x, y] = (int)TileType.Wall_Bottom; // Wall with floor **above** is a BOTTOM wall
                        else if (hasFloorRight)
                            mazeGrid[x, y] = (int)TileType.Wall_Left;   // Wall with floor **to the right** is a LEFT wall
                        else if (hasFloorLeft)
                            mazeGrid[x, y] = (int)TileType.Wall_Right;  // Wall with floor **to the left** is a RIGHT wall
                    }
                }
            }
        }


        private void ProcessFloorTiles()
        {
            Dictionary<TileType, List<(int x, int y)>> floorCategories = new();

            // Step 1: Scan the grid and classify floor tiles
            for (int y = 1; y < Height * CellSize - 1; y++) // Avoid out-of-bounds
            {
                for (int x = 1; x < Width * CellSize - 1; x++)
                {
                    if (mazeGrid[x, y] == (int)TileType.Floor_Center) // Only process floor tiles
                    {
                        bool top = (mazeGrid[x, y - 1] == (int)TileType.Floor_Center);
                        bool bottom = (mazeGrid[x, y + 1] == (int)TileType.Floor_Center);
                        bool left = (mazeGrid[x - 1, y] == (int)TileType.Floor_Center);
                        bool right = (mazeGrid[x + 1, y] == (int)TileType.Floor_Center);

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
                    mazeGrid[x, y] = (int)category.Key; // Assign correct tile type
                }
            }
        }








        private void ProcessTileTypes()
        {
            ProcessWalls();
            ProcessCorners();
            ProcessInnerCorners();
            ProcessWallTypes();
            ProcessFloorTiles();
        }
        // ------------------------------------------------------------------------------
        // END            ---------------------------------------------------------------
        // ------------------------------------------------------------------------------

        private List<(int, int)> RandomizedDirections()
        {
            List<(int, int)> directions = new() { (0, -1), (0, 1), (-1, 0), (1, 0) };
            return directions.OrderBy(_ => rand.Next()).ToList();
        }

        private bool IsWithinBounds(int x, int y) => x > 0 && y > 0 && x < Width - 1 && y < Height - 1;

        public void PrintMaze()
        {
            for (int y = 0; y < Height * CellSize; y++)
            {
                for (int x = 0; x < Width * CellSize; x++)
                {
                    int tileValue = mazeGrid[x, y];  // Get the raw numeric value
                    Console.Write(tileValue.ToString().PadLeft(2, ' ')); // Align numbers for readability
                }
                Console.WriteLine(); // Move to the next line after each row
            }
        }







        public string GetTileSprite(int x, int y)
        {
            // TODO: Add randomization for floor tiles variants and applicable walls (top, bottom, left, right)
            // use random to handle file name number for different floor and wall tiles

            switch ((TileType)mazeGrid[x, y])
            {
                case TileType.Empty_Black: return "/assets/textures/plain_empty_black.png";
                case TileType.Start: return "/assets/textures/plain_empty_black.png"; // Temporarily use empty black
                case TileType.Goal: return "/assets/textures/plain_empty_black.png"; // Temporarily use empty black

                case TileType.Floor_Center: return "/assets/textures/floor_center_1.png";
                case TileType.Floor_Top: return "/assets/textures/floor_top_1.png";
                case TileType.Floor_Bottom: return "/assets/textures/floor_bottom_1.png";
                case TileType.Floor_Left: return "/assets/textures/floor_left_1.png";
                case TileType.Floor_Right: return "/assets/textures/floor_right_1.png";
                case TileType.Floor_Corner_TopLeft: return "/assets/textures/floor_corner_top_left.png";
                case TileType.Floor_Corner_TopRight: return "/assets/textures/floor_corner_top_right.png";
                case TileType.Floor_Corner_BottomLeft: return "/assets/textures/floor_corner_bottom_left.png";
                case TileType.Floor_Corner_BottomRight: return "/assets/textures/floor_corner_bottom_right.png";

                case TileType.Wall_Top: return "/assets/textures/wall_top_1.png";
                case TileType.Wall_Bottom: return "/assets/textures/wall_bottom_1.png";
                case TileType.Wall_Left: return "/assets/textures/wall_left_1.png";
                case TileType.Wall_Right: return "/assets/textures/wall_right_1.png";
                case TileType.Wall_Corner_TopLeft: return "/assets/textures/wall_corner_top_left.png";
                case TileType.Wall_Corner_TopRight: return "/assets/textures/wall_corner_top_right.png";
                case TileType.Wall_Corner_BottomLeft: return "/assets/textures/wall_corner_bottom_left.png";
                case TileType.Wall_Corner_BottomRight: return "/assets/textures/wall_corner_bottom_right.png";
                case TileType.Wall_InnerCorner_TopLeft: return "/assets/textures/wall_inner_corner_top_left_1.png";
                case TileType.Wall_InnerCorner_TopRight: return "/assets/textures/wall_inner_corner_top_right_1.png";
                case TileType.Wall_InnerCorner_BottomLeft:
                    return "/assets/textures/wall_inner_corner_bottom_left_1.png";
                case TileType.Wall_InnerCorner_BottomRight:
                    return "/assets/textures/wall_inner_corner_bottom_right_1.png";

                case TileType.Plain_Wall: return "/assets/textures/plain_wall_beige.png";
                case TileType.Plain_Floor: return "/assets/textures/plain_floor_purple.png";

                default: return "/assets/textures/plain_empty_black.png"; // Default to empty black
            }
        }

        /* Reference: TileType.cs
           Empty_Black = -1,
           Start = -2,
           Goal = -3,
           
           Floor_Center = 0,
           Floor_Top = 1,
           Floor_Bottom = 2,
           Floor_Left = 3,
           Floor_Right = 4,
           Floor_Corner_TopLeft = 5,
           Floor_Corner_TopRight = 6,
           Floor_Corner_BottomLeft = 7,
           Floor_Corner_BottomRight = 8,
           
           Wall_Top = 9,
           Wall_Bottom = 10,
           Wall_Left = 11,
           Wall_Right = 12,
           Wall_Corner_TopLeft = 13,
           Wall_Corner_TopRight = 14,
           Wall_Corner_BottomLeft = 15,
           Wall_Corner_BottomRight = 16,
           Wall_InnerCorner_TopLeft = 17,
           Wall_InnerCorner_TopRight = 18,
           Wall_InnerCorner_BottomLeft = 19,
           Wall_InnerCorner_BottomRight = 20,
           
           Plain_Wall = 21, // colored tile
           Plain_Floor = 22 // colored tile
         */
    }

}
