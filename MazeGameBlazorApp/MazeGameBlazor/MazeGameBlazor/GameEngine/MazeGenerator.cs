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
        private const int Width = 40; // Logical maze grid (cells)
        private const int Height = 40;
        private const int RoomMaxSize = 4;
        private const int RoomMinSize = 3;
        private const int MaxRooms = 10;
        private Random rand = new Random();
        private HashSet<(int x, int y)> walkableTiles = new();

        public int[,] mazeGrid = new int[Width * CellSize, Height * CellSize];
        private (int, int) startPosition;
        private (int, int) goalPosition;

        public enum MazeAlgorithm
        {
            RecursiveBacktracking,
            Prims,
            DrunkardsWalk,
            BSPDungeon
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

            // Step 2: Generate the maze structure using the selected algorithm
            switch (algorithm)
            {
                case MazeAlgorithm.RecursiveBacktracking:
                    GenerateMazeDFS(1, 1, new bool[Width, Height]);
                    break;
                case MazeAlgorithm.Prims:
                    GenerateMazePrims();
                    break;
                case MazeAlgorithm.DrunkardsWalk:
                    GenerateDrunkardsWalk();
                    break;
                case MazeAlgorithm.BSPDungeon:
                    GenerateBSPDungeon();
                    break;
            }

            // Step 3: Ensure connectivity (for Cellular Automata & Drunkard’s Walk)
            if (algorithm == MazeAlgorithm.DrunkardsWalk)
            {
                ConnectDisconnectedCaves();
            }

            // Step 4: Set Start and Goal
            SetStartAndGoal();

            // Step 5: Process tiles for correct wall placement
            ProcessTileTypes();

            // Step 6: Print the maze to the console
            PrintMaze();
        }

        // GetStartPosition method
        public (int, int) GetStartPosition()
        {
            Console.WriteLine($"start position : ({startPosition.Item1},{startPosition.Item2})");
            return startPosition;
        }

        // ------------------------------------------------------------------------------
        // Collect walkable tiles for item placement and player movements ---------------
        // ------------------------------------------------------------------------------

        // **Extract Walkable Tiles from Grid**
        private void IdentifyWalkableTiles()
        {
            walkableTiles.Clear();

            for (int y = 0; y < Height * CellSize; y++)
            {
                for (int x = 0; x < Width * CellSize; x++)
                {
                    if (mazeGrid[x, y] == 0)
                    {
                        walkableTiles.Add((x, y));
                    }
                }
            }
        }

        // **Get Walkable Tiles for External Use**
        public HashSet<(int x, int y)> GetWalkableTiles()
        {
            return walkableTiles;
        }

        // **Check if a Given Tile is Walkable**
        public bool IsWalkable(int x, int y)
        {
            return walkableTiles.Contains((x, y));
        }
    



        // ------------------------------------------------------------------------------
        // Maze Generation Algorithms ---------------------------------------------------
        // ------------------------------------------------------------------------------
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
            List<(int, int)> walls = new();

            int startX = rand.Next(1, Width - 1);
            int startY = rand.Next(1, Height - 1);
            mazeGrid[startX, startY] = (int)TileType.Floor_Center;
            walls.AddRange(GetNeighbors(startX, startY));

            while (walls.Count > 0)
            {
                var (wx, wy) = walls[rand.Next(walls.Count)];
                walls.Remove((wx, wy));

                var adjacentFloors = GetNeighbors(wx, wy).Where(n => mazeGrid[n.Item1, n.Item2] == (int)TileType.Floor_Center).ToList();

                if (adjacentFloors.Count == 1) // Carve only if touching 1 floor tile
                {
                    mazeGrid[wx, wy] = (int)TileType.Floor_Center;
                    walls.AddRange(GetNeighbors(wx, wy).Where(n => mazeGrid[n.Item1, n.Item2] == (int)TileType.Empty_Black));
                }
            }
        }
        private List<(int, int)> GetNeighbors(int x, int y)
        {
            List<(int, int)> neighbors = new List<(int, int)>();

            if (x > 1) neighbors.Add((x - 1, y)); // Left
            if (x < Width - 2) neighbors.Add((x + 1, y)); // Right
            if (y > 1) neighbors.Add((x, y - 1)); // Top
            if (y < Height - 2) neighbors.Add((x, y + 1)); // Bottom

            return neighbors;
        }




        private void GenerateDrunkardsWalk()
        {
            int startX = Width / 2 * CellSize;
            int startY = Height / 2 * CellSize;
            int steps = (Width * Height) * 3; // More steps for better coverage

            int x = startX;
            int y = startY;
            mazeGrid[x, y] = (int)TileType.Floor_Center;

            for (int i = 0; i < steps; i++)
            {
                int dir = rand.Next(4);
                switch (dir)
                {
                    case 0: if (y > 1) y--; break; // Up
                    case 1: if (y < Height * CellSize - 2) y++; break; // Down
                    case 2: if (x > 1) x--; break; // Left
                    case 3: if (x < Width * CellSize - 2) x++; break; // Right
                }

                mazeGrid[x, y] = (int)TileType.Floor_Center; // Carve out path
            }

            
        }


        private void GenerateBSPDungeon()
        {
            List<Rectangle> rooms = new();
            BSPDivide(new Rectangle(1, 1, Width - 2, Height - 2), rooms);

            foreach (var room in rooms)
            {
                CarveRoom(room);
            }

            ConnectBSPRooms(rooms);
        }

        private void BSPDivide(Rectangle area, List<Rectangle> rooms)
        {
            if (area.Width < RoomMinSize * 2 || area.Height < RoomMinSize * 2)
            {
                rooms.Add(area);
                return;
            }

            bool divideHorizontally = (area.Width > area.Height);
            int split = divideHorizontally ?
                rand.Next(area.Width / 3, 2 * area.Width / 3) :
                rand.Next(area.Height / 3, 2 * area.Height / 3);

            if (divideHorizontally)
            {
                BSPDivide(new Rectangle(area.Left, area.Top, split, area.Height), rooms);
                BSPDivide(new Rectangle(area.Left + split, area.Top, area.Width - split, area.Height), rooms);
            }
            else
            {
                BSPDivide(new Rectangle(area.Left, area.Top, area.Width, split), rooms);
                BSPDivide(new Rectangle(area.Left, area.Top + split, area.Width, area.Height - split), rooms);
            }
        }

        private void CarveRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right - 1; x++)
            for (int y = room.Top + 1; y < room.Bottom - 1; y++)
                mazeGrid[x, y] = (int)TileType.Floor_Center;
        }

        private void ConnectBSPRooms(List<Rectangle> rooms)
        {
            for (int i = 1; i < rooms.Count; i++)
            {
                Rectangle r1 = rooms[i - 1];
                Rectangle r2 = rooms[i];
                int midX = (r1.Left + r2.Left) / 2;
                int midY = (r1.Top + r2.Top) / 2;

                for (int x = Math.Min(r1.Left, r2.Left); x <= Math.Max(r1.Right, r2.Right); x++)
                    mazeGrid[x, midY] = (int)TileType.Floor_Center;

                for (int y = Math.Min(r1.Top, r2.Top); y <= Math.Max(r1.Bottom, r2.Bottom); y++)
                    mazeGrid[midX, y] = (int)TileType.Floor_Center;
            }
        }
        private void ConnectDisconnectedCaves()
        {
            // Find all separate cave regions
            Dictionary<(int, int), int> regions = new();
            int regionId = 0;

            for (int y = 1; y < Height * CellSize - 1; y++)
            {
                for (int x = 1; x < Width * CellSize - 1; x++)
                {
                    if (mazeGrid[x, y] == (int)TileType.Floor_Center && !regions.ContainsKey((x, y)))
                    {
                        FloodFill(x, y, regionId, regions);
                        regionId++;
                    }
                }
            }

            if (regionId <= 1) return; // Already fully connected

            // Find and connect nearest regions
            List<(int x, int y, int region)> regionCenters = new();
            for (int r = 0; r < regionId; r++)
            {
                var regionTiles = regions.Where(kvp => kvp.Value == r).Select(kvp => kvp.Key).ToList();
                var center = regionTiles[regionTiles.Count / 2]; // Approximate center of the region
                regionCenters.Add((center.Item1, center.Item2, r));
            }

            for (int i = 0; i < regionCenters.Count - 1; i++)
            {
                var start = regionCenters[i];
                var end = regionCenters[i + 1];

                CreateTunnel(start.x, start.y, end.x, end.y);
            }
        }

        private void FloodFill(int x, int y, int regionId, Dictionary<(int, int), int> regions)
        {
            Queue<(int, int)> queue = new();
            queue.Enqueue((x, y));

            while (queue.Count > 0)
            {
                var (cx, cy) = queue.Dequeue();
                if (regions.ContainsKey((cx, cy))) continue;

                regions[(cx, cy)] = regionId;

                foreach (var (nx, ny) in GetNeighbors(cx, cy))
                {
                    if (mazeGrid[nx, ny] == (int)TileType.Floor_Center && !regions.ContainsKey((nx, ny)))
                    {
                        queue.Enqueue((nx, ny));
                    }
                }
            }
        }

        private void CreateTunnel(int x1, int y1, int x2, int y2)
        {
            // Randomly decide if it should go horizontally first or vertically first
            bool horizontalFirst = rand.Next(2) == 0;

            if (horizontalFirst)
            {
                for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                    mazeGrid[x, y1] = (int)TileType.Floor_Center;
                for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                    mazeGrid[x2, y] = (int)TileType.Floor_Center;
            }
            else
            {
                for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                    mazeGrid[x1, y] = (int)TileType.Floor_Center;
                for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                    mazeGrid[x, y2] = (int)TileType.Floor_Center;
            }
        }

        // ------------------------------------------------------------------------------
        // Utility Functions ------------------------------------------------------------
        // ------------------------------------------------------------------------------


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
            Dictionary<(int x, int y), TileType> cornerUpdates = new();

            for (int y = 1; y < Height * CellSize - 1; y++) // Avoid out-of-bounds
            {
                for (int x = 1; x < Width * CellSize - 1; x++)
                {
                    if (mazeGrid[x, y] == -1) // Only process empty spaces (-1)
                    {
                        // Cardinal walls
                        bool top = (mazeGrid[x, y - 1] == 1);
                        bool bottom = (mazeGrid[x, y + 1] == 1);
                        bool left = (mazeGrid[x - 1, y] == 1);
                        bool right = (mazeGrid[x + 1, y] == 1);

                        // Diagonal floors (valid open spaces)
                        bool topLeftDiag = (mazeGrid[x - 1, y - 1] == 0);
                        bool topRightDiag = (mazeGrid[x + 1, y - 1] == 0);
                        bool bottomLeftDiag = (mazeGrid[x - 1, y + 1] == 0);
                        bool bottomRightDiag = (mazeGrid[x + 1, y + 1] == 0);

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
                mazeGrid[pos.x, pos.y] = (int)type;
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
                        if (top && left) mazeGrid[x, y] = (int)TileType.Wall_InnerCorner_BottomRight;
                        else if (top && right) mazeGrid[x, y] = (int)TileType.Wall_InnerCorner_BottomLeft; 
                        else if (bottom && left) mazeGrid[x, y] = (int)TileType.Wall_Top;
                        else if (bottom && right) mazeGrid[x, y] = (int)TileType.Wall_Top; 
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
            // get walkable tiles before processing (at this point there is only -1 and 0 in the grid)
            IdentifyWalkableTiles();
            // Process the tiles
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
            switch ((TileType)mazeGrid[x, y])
            {
                case TileType.Empty_Black: return "/assets/textures/plain_empty_black.png";
                case TileType.Start: return "/assets/textures/plain_empty_black.png"; // Temporarily use empty black
                case TileType.Goal: return "/assets/textures/plain_empty_black.png"; // Temporarily use empty black

                //floor center 14 variants
                case TileType.Floor_Center: return $"/assets/textures/floor_center_{rand.Next(1,15)}.png";
                //floor top 2 variant
                case TileType.Floor_Top: return $"/assets/textures/floor_top_{rand.Next(1,3)}.png";
                //floor bottom 2 variant
                case TileType.Floor_Bottom: return $"/assets/textures/floor_bottom_{rand.Next(1,3)}.png";
                case TileType.Floor_Left: return "/assets/textures/floor_left_1.png";
                case TileType.Floor_Right: return "/assets/textures/floor_right_1.png";
                case TileType.Floor_Corner_TopLeft: return "/assets/textures/floor_corner_top_left.png";
                case TileType.Floor_Corner_TopRight: return "/assets/textures/floor_corner_top_right.png";
                case TileType.Floor_Corner_BottomLeft: return "/assets/textures/floor_corner_bottom_left.png";
                case TileType.Floor_Corner_BottomRight: return "/assets/textures/floor_corner_bottom_right.png";

                //wall top 4 variant
                case TileType.Wall_Top: return $"/assets/textures/wall_top_{rand.Next(1,5)}.png";
                //wall bottom 6 variant
                case TileType.Wall_Bottom: return $"/assets/textures/wall_bottom_{rand.Next(1,7)}.png";
                //wall left 3 variant
                case TileType.Wall_Left: return $"/assets/textures/wall_left_{rand.Next(1, 4)}.png";
                //wall right 3 variant
                case TileType.Wall_Right: return $"/assets/textures/wall_right_{rand.Next(1, 4)}.png";
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
