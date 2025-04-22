using MazeGameBlazor.Shared.GameEngine.Utils.GeneratingAlgorithms;

namespace MazeGameBlazor.Shared.GameEngine.Models;

public class GameState
{
    public Maze? Maze { get; set; }
    public Player? Player { get; set; }
    public FogOfWar FogOfWar { get; set; } = new();

    public bool GameStarted { get; set; }
    public bool GameRunning { get; set; }
    public MazeAlgorithmType SelectedAlgorithm { get; set; } = MazeAlgorithmType.RecursiveBacktracking;
    public MazeRendererType RendererType { get; set; } = MazeRendererType.Canvas2D;
    public bool MazeInitialized { get; set; } = false;

    public int MaxHearts { get; set; } = 5;
    public double CurrentHearts { get; set; } = 3.0;
    public string[] InventorySlots { get; set; } = new string[6];
    public TimeSpan Timer { get; set; } = TimeSpan.Zero;
    public string StatusEffect { get; set; } = "Normal";
    public bool GameOver { get; set; } = false;

    public bool GoalUnlocked { get; set; } = false;


}