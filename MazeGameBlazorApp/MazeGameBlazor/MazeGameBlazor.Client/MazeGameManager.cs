using MazeGameBlazor.Shared.GameEngine.Audio;
using MazeGameBlazor.Shared.GameEngine.Models;
using Microsoft.JSInterop;

namespace MazeGameBlazor.Client
{
    public class MazeGameManager
    {
        // ================================================================
        // Dependencies & State
        // ================================================================
        private readonly MazeGameService _gameService;
        private readonly MazeInputManager _inputManager;
        private readonly IJSRuntime _js;
        private readonly GameState _state;
        private bool _loopRunning = false;
        private CancellationTokenSource? _timerLoopCts;

        public Func<Task>? NotifyUi { get; set; }

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

        // ================================================================
        // Maze Initialization
        // ================================================================
        public async Task InitializeAsync()
        {
            _state.MazeInitialized = false;
            _state.Maze = _gameService.GenerateMaze(_state.SelectedAlgorithm);
            _state.RendererType = await MazeInterop.DetectRendererAsync(_js);

            var spriteGrid = _gameService.GenerateSpriteGrid(_state.Maze);
            var flattened = spriteGrid.Cast<string>().ToArray();

            var (x, y) = _state.Maze?.StartPosition ?? (0, 0);
            _state.Player = new Player(x, y, _state.Maze, _state);

            await _js.InvokeVoidAsync("eval", $@"
                window.initialPlayerX = {x};
                window.initialPlayerY = {y};
                window.initialPlayerSprite = '{_state.Player.GetCurrentSprite()}';
            ");

            await MazeInterop.InitRendererAsync(_js, _state.RendererType, flattened, _state.Maze.Width, _state.Maze.Height);

            var items = _state.Maze.ItemGrid.GetAllItems()
                .Select(i => new { i.X, i.Y, i.Sprite }).ToList();

            await MazeInterop.UpdateItemsAsync(_js, _state.RendererType, items);

            _state.MazeInitialized = true;

            if (NotifyUi is not null)
                await NotifyUi.Invoke();
        }

        // ================================================================
        // Game Lifecycle Control
        // ================================================================
        public async Task StartGameAsync()
        {
            _state.GameStarted = true;
            _state.GameStartTime = DateTime.Now;
            _state.Timer = _state.TimeLimit;

            _timerLoopCts = new();
            _ = TimerLoopAsync(_timerLoopCts.Token);

            var (x, y) = _state.Maze?.StartPosition ?? (0, 0);
            _state.Player = new Player(x, y, _state.Maze, _state);

            await _js.InvokeVoidAsync("clearOverlay");
            await MazeInterop.SpawnPlayerAsync(_js, _state.RendererType, x, y, _state.Player.GetCurrentSprite());

            await MazeInterop.PlayBackgroundMusicAsync(_js);
        }

        public async Task EndGameAsync()
        {
            _timerLoopCts?.Cancel();
            _state.GameOver = true;
            _state.GameRunning = false;

            var message = _state.CurrentHearts <= 0 ? "You Died!" : "You Escaped!";
            await _js.InvokeVoidAsync("showOverlay", message, true);

            if (NotifyUi is not null)
                await NotifyUi.Invoke();
        }

        public void RestartGame()
        {
            _timerLoopCts?.Cancel();
            _state.GameStarted = false;
            _state.GameRunning = false;
            _ = InitializeAsync();
        }

        public Task OnAlgorithmChangeAsync() => InitializeAsync();

        // ================================================================
        // Game Focus & Input Handling
        // ================================================================
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

        // ================================================================
        // 🎮 Main Game Loop
        // ================================================================
        public async Task TickLoopAsync()
        {
            if (_loopRunning || !_state.GameStarted || _state.Player == null)
                return;

            _loopRunning = true;

            while (_inputManager.IsMoving)
            {
                var dir = _inputManager.GetLastDirection();
                _state.Player.Move(dir, _state.Maze);
                _state.Player.TryPickupItem(_state.Maze);

                if (_state.Timer <= TimeSpan.Zero && !_state.GameOver)
                {
                    _state.StatusEffect = "Time's up!";
                    _state.CurrentHearts = 0;
                    await EndGameAsync();
                    await MazeInterop.PlaySoundEffectAsync(_js, AudioTracks.GameOver);
                    return;
                }

                if (_state.CurrentHearts <= 0 && !_state.GameOver)
                {
                    await EndGameAsync();
                    await MazeInterop.PlaySoundEffectAsync(_js, AudioTracks.GameOver);
                    break;
                }

                if (_state.PlayHealSound)
                {
                    await MazeInterop.PlaySoundEffectAsync(_js, AudioTracks.PotionPickup);
                    _state.PlayHealSound = false;
                }

                if (_state.PlayPowerUpSound)
                {
                    await MazeInterop.PlaySoundEffectAsync(_js, AudioTracks.PowerUp);
                    _state.PlayPowerUpSound = false;
                }

                if (_state.PlayTeleportSound)
                {
                    await MazeInterop.PlaySoundEffectAsync(_js, AudioTracks.Teleport);
                    _state.PlayTeleportSound = false;
                }

                if (_state.PlayTrapDamageSound)
                {
                    await MazeInterop.PlaySoundEffectAsync(_js, AudioTracks.TrapDamage);
                    _state.PlayTrapDamageSound = false;
                }

                await MazeInterop.SetLightRadiusAsync(_js, _state.Player.LightRadius);

                if (_state.StatusEffect == "Guided")
                {
                    var goal = _state.Maze.GoalPosition;
                    await MazeInterop.SetMinimapGoalAsync(_js, goal.x, goal.y);
                    _state.StatusEffect = "Normal";
                }

                var atGoal = _state.Maze.GoalPosition == (_state.Player.X, _state.Player.Y);

                if (atGoal && _state.GoalUnlocked)
                {
                    await EndGameAsync();
                    await MazeInterop.PlaySoundEffectAsync(_js, AudioTracks.WinGame);
                    return;
                }

                if (atGoal)
                {
                    _state.StatusEffect = "Goal Locked, find the key...";
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

        private async Task TimerLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && _state.GameStarted && !_state.GameOver)
            {
                var elapsed = DateTime.Now - _state.GameStartTime;
                _state.Timer = _state.TimeLimit - elapsed;

                if (_state.Timer < TimeSpan.Zero)
                    _state.Timer = TimeSpan.Zero;

                if (NotifyUi is not null)
                    await NotifyUi.Invoke();

                await Task.Delay(1000, ct);
            }
        }
    }
}