using MazeGameBlazor.Shared.GameEngine.Utils;

namespace MazeGameBlazor.Shared.GameEngine.Models;

public class Player
{
    private const int AnimationSpeed = 10;
    private int _animationFrame;
    private int _frameCounter;
    private Maze _maze;
    private GameState _state;
    public int LightRadius { get; private set; } = 3;




    public Dictionary<string, string[]> Animations = new()
    {
        { "up", [
            "/assets/sprites/player/character_walk/up_1.png", "/assets/sprites/player/character_walk/up_2.png",
            "/assets/sprites/player/character_walk/up_3.png", "/assets/sprites/player/character_walk/up_4.png"
        ]},
        { "down", [
            "/assets/sprites/player/character_walk/down_1.png", "/assets/sprites/player/character_walk/down_2.png",
            "/assets/sprites/player/character_walk/down_3.png", "/assets/sprites/player/character_walk/down_4.png"
        ]},
        { "left", [
            "/assets/sprites/player/character_walk/left_1.png", "/assets/sprites/player/character_walk/left_2.png",
            "/assets/sprites/player/character_walk/left_3.png", "/assets/sprites/player/character_walk/left_4.png"
        ]},
        { "right", [
            "/assets/sprites/player/character_walk/right_1.png", "/assets/sprites/player/character_walk/right_2.png",
            "/assets/sprites/player/character_walk/right_3.png", "/assets/sprites/player/character_walk/right_4.png"
        ]}
    };

    public Player(int startX, int startY, Maze maze, GameState state)
    {
        X = startX;
        Y = startY;
        _maze = maze;
        _state = state;
        Direction = "down";
    }

    public int X { get; private set; }
    public int Y { get; private set; }
    public string? Direction { get; private set; }

    public string GetCurrentSprite()
    {
        if (Direction == null || !Animations.TryGetValue(Direction, out var animation))
            return Animations["down"][0];

        return animation[_animationFrame];
    }

    public void Move(string? direction, Maze maze)
    {
        if (direction == null) return;

        Direction = direction;

        switch (direction)
        {
            case "up":
                if (maze.IsWalkable(X, Y - 1)) Y--;
                break;
            case "down":
                if (maze.IsWalkable(X, Y + 1)) Y++;
                break;
            case "left":
                if (maze.IsWalkable(X - 1, Y)) X--;
                break;
            case "right":
                if (maze.IsWalkable(X + 1, Y)) X++;
                break;
        }

        Animate();
    }

    private void Animate()
    {
        _frameCounter++;
        if (_frameCounter >= AnimationSpeed)
        {
            _frameCounter = 0;
            _animationFrame = (_animationFrame + 1) % Animations[Direction].Length;
        }
    }

    public void TryPickupItem(Maze maze)
    {
        var item = maze.ItemGrid.GetItemAt(X, Y);

        if (item != null)
        {
            ApplyItemEffect(item);

            if (item.Collectible && item.Effect != ItemEffect.Heal)
            {
                for (int i = 0; i < _state.InventorySlots.Length; i++)
                {
                    if (string.IsNullOrEmpty(_state.InventorySlots[i]))
                    {
                        _state.InventorySlots[i] = item.Sprite;
                        break;
                    }
                }

                maze.ItemGrid.RemoveItem(item.X, item.Y);
            }
            else if (item.Collectible && item.Effect == ItemEffect.Heal)
            {
                maze.ItemGrid.RemoveItem(item.X, item.Y);
            }
        }
    }


    private void ApplyItemEffect(Item item)
    {
        switch (item.Effect)
        {
            case ItemEffect.Heal:
                Heal(0.5);
                break;
            case ItemEffect.Unlock:
                _state.GoalUnlocked = true;
                _state.StatusEffect = "Goal Unlocked";
                break;

            case ItemEffect.Teleport:
                TeleportToRandomLocation();
                break;
            case ItemEffect.LightRadiusIncrease:
                IncreaseVisionRadius();
                break;
            case ItemEffect.ShowDirection:
                _state.StatusEffect = "Guided";
                break;
            case ItemEffect.Damage:
                TakeDamage(0.25);
                break;
            default:
                break;
        }
    }

    private void Heal(double amount)
    {
        _state.CurrentHearts = Math.Min(_state.CurrentHearts + amount, _state.MaxHearts);
    }

    private void TakeDamage(double amount)
    {
        _state.CurrentHearts = Math.Max(_state.CurrentHearts - amount, 0);
    }

    private void TeleportToRandomLocation()
    {
        var randomTile = MazeUtils.GetRandomWalkableTile(_maze);
        if (randomTile.HasValue)
        {
            X = randomTile.Value.x;
            Y = randomTile.Value.y;
        }
    }

    private void IncreaseVisionRadius()
    {
        LightRadius += 2;
        _state.StatusEffect = "Vision Radius Increased";
    }

}
