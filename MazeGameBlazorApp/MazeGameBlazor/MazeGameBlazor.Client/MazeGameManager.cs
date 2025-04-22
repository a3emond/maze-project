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
            _state.Player = new Player(x, y, _state.Maze, _state);

            await _js.InvokeVoidAsync("clearOverlay");
            await MazeInterop.SpawnPlayerAsync(_js, _state.RendererType, x, y, _state.Player.GetCurrentSprite());
        }

        public Func<Task>? NotifyUi { get; set; }

        public Task OnAlgorithmChangeAsync() => InitializeAsync();

        public void RestartGame()
        {
            _state.GameStarted = false;
            _state.GameRunning = false;
            _ = InitializeAsync();
        }

        public async Task FocusGameScreenAsync()
        {
            await _js.InvokeVoidAsync("focusGameScreen");
        }

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
        public async Task EndGameAsync()
        {
            _state.GameOver = true;
            _state.GameRunning = false;
            await _js.InvokeVoidAsync("showOverlay", "Game Over", true);
            if (NotifyUi is not null)
                await NotifyUi.Invoke();
        }


        public async Task TickLoopAsync()
        {
            if (_loopRunning || !_state.GameStarted || _state.Player == null) return;

            _loopRunning = true;
            while (_inputManager.IsMoving)
            {
                var dir = _inputManager.GetLastDirection();
                _state.Player.Move(dir, _state.Maze);

                _state.Player.TryPickupItem(_state.Maze);
                if (_state.CurrentHearts <= 0 && !_state.GameOver) // check for game over (0 life)
                {
                    await EndGameAsync();
                    break;
                }



                await MazeInterop.UpdatePlayerAsync(_js, _state.RendererType, _state.Player.X, _state.Player.Y, _state.Player.GetCurrentSprite());
                await MazeInterop.UpdateItemsAsync(_js, _state.RendererType,
                    _state.Maze.ItemGrid.GetAllItems().Select(i => new { i.X, i.Y, i.Sprite }).ToList());

                if (NotifyUi is not null)
                    await NotifyUi.Invoke();

                await Task.Delay(30);
            }
            _loopRunning = false;
        }
    }
}
