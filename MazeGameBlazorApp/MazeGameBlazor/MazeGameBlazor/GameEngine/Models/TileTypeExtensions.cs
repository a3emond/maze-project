namespace MazeGameBlazor.GameEngine.Models;

public static class TileTypeExtensions
{
    private static readonly Random Rand = new();

    public static string GetTileSprite(this TileType tileType)
    {
        return tileType switch
        {
            TileType.EmptyBlack => "/assets/textures/plain_empty_black.png",
            TileType.Start => $"/assets/textures/floor_center_{Rand.Next(1, 15)}.png",
            TileType.Goal => $"/assets/textures/floor_center_{Rand.Next(1, 15)}.png",

            // Floor Center - 14 Variants
            TileType.FloorCenter => $"/assets/textures/floor_center_{Rand.Next(1, 15)}.png",
            TileType.FloorTop => $"/assets/textures/floor_top_{Rand.Next(1, 3)}.png",
            TileType.FloorBottom => $"/assets/textures/floor_bottom_{Rand.Next(1, 3)}.png",
            TileType.FloorLeft => "/assets/textures/floor_left_1.png",
            TileType.FloorRight => "/assets/textures/floor_right_1.png",
            TileType.FloorCornerTopLeft => "/assets/textures/floor_corner_top_left.png",
            TileType.FloorCornerTopRight => "/assets/textures/floor_corner_top_right.png",
            TileType.FloorCornerBottomLeft => "/assets/textures/floor_corner_bottom_left.png",
            TileType.FloorCornerBottomRight => "/assets/textures/floor_corner_bottom_right.png",

            // Wall Variants
            TileType.WallTop => $"/assets/textures/wall_top_{Rand.Next(1, 5)}.png",
            TileType.WallBottom => $"/assets/textures/wall_bottom_{Rand.Next(1, 7)}.png",
            TileType.WallLeft => $"/assets/textures/wall_left_{Rand.Next(1, 4)}.png",
            TileType.WallRight => $"/assets/textures/wall_right_{Rand.Next(1, 4)}.png",

            // Corner Wall Variants
            TileType.WallCornerTopLeft => "/assets/textures/wall_corner_top_left.png",
            TileType.WallCornerTopRight => "/assets/textures/wall_corner_top_right.png",
            TileType.WallCornerBottomLeft => "/assets/textures/wall_corner_bottom_left.png",
            TileType.WallCornerBottomRight => "/assets/textures/wall_corner_bottom_right.png",

            // Inner Corners
            TileType.WallInnerCornerTopLeft => "/assets/textures/wall_inner_corner_top_left_1.png",
            TileType.WallInnerCornerTopRight => "/assets/textures/wall_inner_corner_top_right_1.png",
            TileType.WallInnerCornerBottomLeft => "/assets/textures/wall_inner_corner_bottom_left_1.png",
            TileType.WallInnerCornerBottomRight => "/assets/textures/wall_inner_corner_bottom_right_1.png",

            // Plain Tiles
            TileType.PlainWall => "/assets/textures/plain_wall_beige.png",
            TileType.PlainFloor => "/assets/textures/plain_floor_purple.png",

            _ => "/assets/textures/plain_empty_black.png" // Default Case
        };
    }
}