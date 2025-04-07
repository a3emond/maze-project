using MazeGameBlazor.Shared.GameEngine.Models;

namespace MazeGameBlazor.Shared.GameEngine.Utils.GeneratingAlgorithms;

public interface IMazeAlgorithm
{
    void Generate(Maze maze);
}