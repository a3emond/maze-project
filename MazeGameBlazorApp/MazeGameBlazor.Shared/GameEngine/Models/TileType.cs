namespace MazeGameBlazor.Shared.GameEngine.Models;

public enum TileType
{
    EmptyBlack = -1,
    Start = -2,
    Goal = -3,

    FloorCenter = 0,
    FloorTop = 1,
    FloorBottom = 2,
    FloorLeft = 3,
    FloorRight = 4,
    FloorCornerTopLeft = 5,
    FloorCornerTopRight = 6,
    FloorCornerBottomLeft = 7,
    FloorCornerBottomRight = 8,

    WallTop = 9,
    WallBottom = 10,
    WallLeft = 11,
    WallRight = 12,
    WallCornerTopLeft = 13,
    WallCornerTopRight = 14,
    WallCornerBottomLeft = 15,
    WallCornerBottomRight = 16,
    WallInnerCornerTopLeft = 17,
    WallInnerCornerTopRight = 18,
    WallInnerCornerBottomLeft = 19,
    WallInnerCornerBottomRight = 20,

    PlainWall = 21, // Generic wall tile
    PlainFloor = 22 // Generic floor tile
}