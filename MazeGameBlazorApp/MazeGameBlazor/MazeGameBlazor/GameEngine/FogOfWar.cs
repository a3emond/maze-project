namespace MazeGameBlazor.GameEngine
{
    public class FogOfWar
    {
        private bool[,] visibilityMap;

        public void UpdateFog(int playerX, int playerY, int visionRadius)
        {
            for (int y = -visionRadius; y <= visionRadius; y++)
            {
                for (int x = -visionRadius; x <= visionRadius; x++)
                {
                    int checkX = playerX + x;
                    int checkY = playerY + y;
                    //if (IsValidTile(checkX, checkY))
                    //{
                    //    visibilityMap[checkY, checkX] = true;
                    //}
                }
            }
        }
    }

}
