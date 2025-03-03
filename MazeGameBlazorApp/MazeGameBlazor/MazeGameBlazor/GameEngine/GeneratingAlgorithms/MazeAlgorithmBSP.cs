using System.Drawing;

namespace MazeGameBlazor.GameEngine.GeneratingAlgorithms;

public class MazeAlgorithmBsp : IMazeAlgorithm
{
    public void Generate(Maze maze)
    {
        List<Rectangle> rooms = new();
        BspDivide(maze, new Rectangle(1, 1, maze.Width - 2, maze.Height - 2), rooms);

        foreach (var room in rooms) CarveRoom(maze, room);

        ConnectBspRooms(maze, rooms);
    }

    private void BspDivide(Maze maze, Rectangle area, List<Rectangle> rooms)
    {
        var rand = new Random();

        if (area.Width < 6 || area.Height < 6) // Ensure minimum room size
        {
            rooms.Add(area);
            return;
        }

        var divideHorizontally = area.Width > area.Height;
        var split = divideHorizontally
            ? rand.Next(area.Width / 3, 2 * area.Width / 3)
            : rand.Next(area.Height / 3, 2 * area.Height / 3);

        if (divideHorizontally)
        {
            BspDivide(maze, new Rectangle(area.Left, area.Top, split, area.Height), rooms);
            BspDivide(maze, new Rectangle(area.Left + split, area.Top, area.Width - split, area.Height), rooms);
        }
        else
        {
            BspDivide(maze, new Rectangle(area.Left, area.Top, area.Width, split), rooms);
            BspDivide(maze, new Rectangle(area.Left, area.Top + split, area.Width, area.Height - split), rooms);
        }
    }

    private void CarveRoom(Maze maze, Rectangle room)
    {
        for (var x = room.Left + 1; x < room.Right - 1; x++)
        for (var y = room.Top + 1; y < room.Bottom - 1; y++)
            maze.Grid[x, y] = (int)TileType.FloorCenter;
    }

    private void ConnectBspRooms(Maze maze, List<Rectangle> rooms)
    {
        for (var i = 1; i < rooms.Count; i++)
        {
            var r1 = rooms[i - 1];
            var r2 = rooms[i];
            var midX = (r1.Left + r2.Left) / 2;
            var midY = (r1.Top + r2.Top) / 2;

            for (var x = Math.Min(r1.Left, r2.Left); x <= Math.Max(r1.Right, r2.Right); x++)
                maze.Grid[x, midY] = (int)TileType.FloorCenter;

            for (var y = Math.Min(r1.Top, r2.Top); y <= Math.Max(r1.Bottom, r2.Bottom); y++)
                maze.Grid[midX, y] = (int)TileType.FloorCenter;
        }
    }
}