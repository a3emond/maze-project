using System;

namespace MazeGameBlazor.GameEngine
{
    public static class TileTypeExtensions
    {
        private static readonly Random rand = new Random();

        public static string GetTileSprite(this TileType tileType)
        {
            return tileType switch
            {
                TileType.Empty_Black => "/assets/textures/plain_empty_black.png",
                TileType.Start => "/assets/textures/start.png",
                TileType.Goal => "/assets/textures/goal.png",

                // Floor Center - 14 Variants
                TileType.Floor_Center => $"/assets/textures/floor_center_{rand.Next(1, 15)}.png",
                TileType.Floor_Top => $"/assets/textures/floor_top_{rand.Next(1, 3)}.png",
                TileType.Floor_Bottom => $"/assets/textures/floor_bottom_{rand.Next(1, 3)}.png",
                TileType.Floor_Left => "/assets/textures/floor_left_1.png",
                TileType.Floor_Right => "/assets/textures/floor_right_1.png",
                TileType.Floor_Corner_TopLeft => "/assets/textures/floor_corner_top_left.png",
                TileType.Floor_Corner_TopRight => "/assets/textures/floor_corner_top_right.png",
                TileType.Floor_Corner_BottomLeft => "/assets/textures/floor_corner_bottom_left.png",
                TileType.Floor_Corner_BottomRight => "/assets/textures/floor_corner_bottom_right.png",

                // Wall Variants
                TileType.Wall_Top => $"/assets/textures/wall_top_{rand.Next(1, 5)}.png",
                TileType.Wall_Bottom => $"/assets/textures/wall_bottom_{rand.Next(1, 7)}.png",
                TileType.Wall_Left => $"/assets/textures/wall_left_{rand.Next(1, 4)}.png",
                TileType.Wall_Right => $"/assets/textures/wall_right_{rand.Next(1, 4)}.png",

                // Corner Wall Variants
                TileType.Wall_Corner_TopLeft => "/assets/textures/wall_corner_top_left.png",
                TileType.Wall_Corner_TopRight => "/assets/textures/wall_corner_top_right.png",
                TileType.Wall_Corner_BottomLeft => "/assets/textures/wall_corner_bottom_left.png",
                TileType.Wall_Corner_BottomRight => "/assets/textures/wall_corner_bottom_right.png",

                // Inner Corners
                TileType.Wall_InnerCorner_TopLeft => "/assets/textures/wall_inner_corner_top_left_1.png",
                TileType.Wall_InnerCorner_TopRight => "/assets/textures/wall_inner_corner_top_right_1.png",
                TileType.Wall_InnerCorner_BottomLeft => "/assets/textures/wall_inner_corner_bottom_left_1.png",
                TileType.Wall_InnerCorner_BottomRight => "/assets/textures/wall_inner_corner_bottom_right_1.png",

                // Plain Tiles
                TileType.Plain_Wall => "/assets/textures/plain_wall_beige.png",
                TileType.Plain_Floor => "/assets/textures/plain_floor_purple.png",

                _ => "/assets/textures/plain_empty_black.png", // Default Case
            };
        }
    }
}

