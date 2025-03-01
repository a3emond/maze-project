using MazeGameBlazor.GameEngine;

namespace MazeGameBlazor.GameEngine
{
    public enum TileType
    {
        
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

        Plain_Wall = 21,  // Generic wall tile
        Plain_Floor = 22  // Generic floor tile
    }

}
