namespace MazeGameBlazor.Shared.GameEngine.Models;

public class FogOfWar
{
    public List<LightSource> Sources { get; } = new();

    public void AddLightSource(int x, int y, int radius)
    {
        Sources.Add(new LightSource(x, y, radius));
    }

    public void Clear() => Sources.Clear();
}
