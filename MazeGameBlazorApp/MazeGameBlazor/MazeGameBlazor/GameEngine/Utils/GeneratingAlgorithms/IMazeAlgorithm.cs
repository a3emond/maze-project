using MazeGameBlazor.GameEngine.Models;

namespace MazeGameBlazor.GameEngine.GeneratingAlgorithms;

public interface IMazeAlgorithm
{
    void Generate(Maze maze);
}