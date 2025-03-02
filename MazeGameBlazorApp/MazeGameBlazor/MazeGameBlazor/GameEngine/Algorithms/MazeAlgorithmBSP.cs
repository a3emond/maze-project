using System.Drawing;

namespace MazeGameBlazor.GameEngine.Algorithms
{
    public class MazeAlgorithmBsp : IMazeAlgorithm
    {
        public void Generate(Maze maze)
        {
            List<Rectangle> rooms = new();
            BSPDivide(maze, new Rectangle(1, 1, maze.Width - 2, maze.Height - 2), rooms);

            foreach (var room in rooms)
            {
                CarveRoom(maze, room);
            }

            ConnectBSPRooms(maze, rooms);
        }

        private void BSPDivide(Maze maze, Rectangle area, List<Rectangle> rooms)
        {
            Random rand = new Random();

            if (area.Width < 6 || area.Height < 6) // Ensure minimum room size
            {
                rooms.Add(area);
                return;
            }

            bool divideHorizontally = (area.Width > area.Height);
            int split = divideHorizontally
                ? rand.Next(area.Width / 3, 2 * area.Width / 3)
                : rand.Next(area.Height / 3, 2 * area.Height / 3);

            if (divideHorizontally)
            {
                BSPDivide(maze, new Rectangle(area.Left, area.Top, split, area.Height), rooms);
                BSPDivide(maze, new Rectangle(area.Left + split, area.Top, area.Width - split, area.Height), rooms);
            }
            else
            {
                BSPDivide(maze, new Rectangle(area.Left, area.Top, area.Width, split), rooms);
                BSPDivide(maze, new Rectangle(area.Left, area.Top + split, area.Width, area.Height - split), rooms);
            }
        }

        private void CarveRoom(Maze maze, Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right - 1; x++)
                for (int y = room.Top + 1; y < room.Bottom - 1; y++)
                    maze.Grid[x, y] = (int)TileType.Floor_Center;
        }

        private void ConnectBSPRooms(Maze maze, List<Rectangle> rooms)
        {
            for (int i = 1; i < rooms.Count; i++)
            {
                Rectangle r1 = rooms[i - 1];
                Rectangle r2 = rooms[i];
                int midX = (r1.Left + r2.Left) / 2;
                int midY = (r1.Top + r2.Top) / 2;

                for (int x = Math.Min(r1.Left, r2.Left); x <= Math.Max(r1.Right, r2.Right); x++)
                    maze.Grid[x, midY] = (int)TileType.Floor_Center;

                for (int y = Math.Min(r1.Top, r2.Top); y <= Math.Max(r1.Bottom, r2.Bottom); y++)
                    maze.Grid[midX, y] = (int)TileType.Floor_Center;
            }
        }
    }
}
