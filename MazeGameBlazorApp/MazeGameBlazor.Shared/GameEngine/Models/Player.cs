using MazeGameBlazor.Shared.GameEngine.Utils;

namespace MazeGameBlazor.Shared.GameEngine.Models;

public class Player
{
    private const int AnimationSpeed = 10; // Adjust for animation timing
    private int _animationFrame;
    private int _frameCounter;
    private Maze _maze;

    public Dictionary<string, string[]> Animations = new()
    {
        { "up", [
            "/assets/sprites/player/character_walk/up_1.png", "/assets/sprites/player/character_walk/up_2.png",
                        "/assets/sprites/player/character_walk/up_3.png", "/assets/sprites/player/character_walk/up_4.png"
            ]
        },

        { "down", [
            "/assets/sprites/player/character_walk/down_1.png", "/assets/sprites/player/character_walk/down_2.png",
                          "/assets/sprites/player/character_walk/down_3.png", "/assets/sprites/player/character_walk/down_4.png"
            ]
        },

        { "left", [
            "/assets/sprites/player/character_walk/left_1.png", "/assets/sprites/player/character_walk/left_2.png",
                          "/assets/sprites/player/character_walk/left_3.png", "/assets/sprites/player/character_walk/left_4.png"
            ]
        },

        { "right", [
            "/assets/sprites/player/character_walk/right_1.png", "/assets/sprites/player/character_walk/right_2.png",
                           "/assets/sprites/player/character_walk/right_3.png", "/assets/sprites/player/character_walk/right_4.png"
            ]
        }
    };

    public Player(int startX, int startY, Maze maze)
    {
        X = startX;
        Y = startY;
        _maze = maze;
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

        // Update position based on direction
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

    private void Animate()  //TODO: Adjust for animation speed
    {
        _frameCounter++;
        if (_frameCounter >= AnimationSpeed)
        {
            _frameCounter = 0;
            _animationFrame = (_animationFrame + 1) % Animations[Direction].Length;
        }
    }

    public void TryPickupItem(Maze maze)  // TODO: integrate and check for item removal
    {
        var item = maze.ItemGrid.PickupItem(X, Y);

        if (item != null)
        {
            ApplyItemEffect(item);
        }
    }

    private void ApplyItemEffect(Item item)
    {
        switch (item.Effect)
        {
            case ItemEffect.Heal:
                Heal(10);
                break;
            case ItemEffect.Unlock:
                // Unlock logic
                break;
            case ItemEffect.Teleport:
                TeleportToRandomLocation();
                break;
            case ItemEffect.LightRadiusIncrease:
                IncreaseVisionRadius();
                break;
            case ItemEffect.ShowDirection:
                // Show direction logic
                break;
            case ItemEffect.Damage:
                TakeDamage(5);
                break;
            default:
                break;
        }
    }

    private void Heal(int amount)
    {
        // Heal logic
        Console.WriteLine("heal");
    }

    private void TakeDamage(int amount)
    {
        // Take damage logic
        Console.WriteLine("damage");
    }

    private void TeleportToRandomLocation()
    {
        // Teleport logic
        Console.WriteLine("teleport");

        // update player position with a random walkable tile
        var randomTile = MazeUtils.GetRandomWalkableTile(_maze);
        if (randomTile.HasValue)
        {
            X = randomTile.Value.x;
            Y = randomTile.Value.y;
        }
    }

    private void IncreaseVisionRadius()
    {
        // Increase vision radius logic
        Console.WriteLine("increase vision radius");
    }
}
