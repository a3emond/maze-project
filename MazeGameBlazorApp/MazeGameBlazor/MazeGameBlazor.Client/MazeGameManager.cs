using MazeGameBlazor.Shared.GameEngine.Models;
using Microsoft.JSInterop;

namespace MazeGameBlazor.Client
{
    public class MazeGameManager
    {
        private readonly MazeGameService _gameService;
        private readonly MazeInputManager _inputManager;
        private readonly IJSRuntime _js;
        private readonly GameState _state;

        private bool _loopRunning = false;

        public MazeGameManager(
            MazeGameService gameService,
            MazeInputManager inputManager,
            IJSRuntime js,
            GameState state)
        {
            _gameService = gameService;
            _inputManager = inputManager;
            _js = js;
            _state = state;
        }

        public async Task InitializeAsync()
        {
            _state.MazeInitialized = false;

            _state.Maze = _gameService.GenerateMaze(_state.SelectedAlgorithm);
            _state.RendererType = await MazeInterop.DetectRendererAsync(_js);

            var spriteGrid = _gameService.GenerateSpriteGrid(_state.Maze);
            var flattened = spriteGrid.Cast<string>().ToArray();

            await MazeInterop.InitRendererAsync(_js, _state.RendererType, flattened, _state.Maze.Width, _state.Maze.Height);
         
            var items = _state.Maze.ItemGrid.GetAllItems()
                .Select(i => new { i.X, i.Y, i.Sprite }).ToList();

            await MazeInterop.UpdateItemsAsync(_js, _state.RendererType, items);

            _state.MazeInitialized = true;
            if (NotifyUi is not null)
                await NotifyUi.Invoke();
        }

        public async Task StartGameAsync()
        {
            _state.GameStarted = true;

            var (x, y) = _state.Maze?.StartPosition ?? (0, 0);
            _state.Player = new Player(x, y, _state.Maze);

            await _js.InvokeVoidAsync("clearOverlay");
            await MazeInterop.SpawnPlayerAsync(_js, _state.RendererType, x, y, _state.Player.GetCurrentSprite());
        }


        public Func<Task>? NotifyUi { get; set; } // to notify the UI when the maze is initialized

        

        public Task OnAlgorithmChangeAsync() => InitializeAsync();

        public void RestartGame()
        {
            _state.GameStarted = false;
            _state.GameRunning = false;
            _ = InitializeAsync();
        }

        public async Task FocusGameScreenAsync() // helper method to focus the game screen on start
        {
            await _js.InvokeVoidAsync("focusGameScreen"); // No corresponding MazeInterop method for this
        }

        // handle keypress/release to void jerky movement ( as long as the key is pressed, the player moves)
        public async Task HandleKeyPressAsync(string key)
        {
            var direction = key switch
            {
                "w" => "up",
                "s" => "down",
                "a" => "left",
                "d" => "right",
                _ => null
            };

            if (direction != null)
            {
                _inputManager.Press(direction);
                await TickLoopAsync();
            }
        }

        public void HandleKeyRelease(string key)
        {
            var direction = key switch
            {
                "w" => "up",
                "s" => "down",
                "a" => "left",
                "d" => "right",
                _ => null
            };

            if (direction != null)
                _inputManager.Release(direction);
        }

        public async Task TickLoopAsync() // this is the main game loop
        {
            if (_loopRunning || !_state.GameStarted || _state.Player == null) return;

            _loopRunning = true;
            while (_inputManager.IsMoving)
            {
                var dir = _inputManager.GetLastDirection();
                _state.Player.Move(dir, _state.Maze);

                await MazeInterop.UpdatePlayerAsync(_js, _state.RendererType, _state.Player.X, _state.Player.Y, _state.Player.GetCurrentSprite());

                _state.Player.TryPickupItem(_state.Maze);

                await Task.Delay(30);
            }
            _loopRunning = false;
        }
    }
}
