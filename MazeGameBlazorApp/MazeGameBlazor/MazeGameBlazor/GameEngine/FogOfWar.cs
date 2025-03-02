namespace MazeGameBlazor.GameEngine;

public class FogOfWar
{
    private bool[,] _visibilityMap;

    public void UpdateFog(int playerX, int playerY, int visionRadius)
    {
        for (var y = -visionRadius; y <= visionRadius; y++)
        for (var x = -visionRadius; x <= visionRadius; x++)
        {
            var checkX = playerX + x;
            var checkY = playerY + y;
            //if (IsValidTile(checkX, checkY))
            //{
            //    visibilityMap[checkY, checkX] = true;
            //}
        }
    }
}