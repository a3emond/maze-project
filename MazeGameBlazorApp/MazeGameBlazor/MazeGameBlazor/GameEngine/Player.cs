namespace MazeGameBlazor.GameEngine;

public class Player
{
    private const int AnimationSpeed = 10; // Adjust for animation timing
    private int _animationFrame;
    private int _frameCounter;

    public Dictionary<string, string[]> Animations = new()
    {
        { "up", new[] { "/assets/sprites/player/character_walk/up_1.png", "/assets/sprites/player/character_walk/up_2.png",
                        "/assets/sprites/player/character_walk/up_3.png", "/assets/sprites/player/character_walk/up_4.png" } },

        { "down", new[] { "/assets/sprites/player/character_walk/down_1.png", "/assets/sprites/player/character_walk/down_2.png",
                          "/assets/sprites/player/character_walk/down_3.png", "/assets/sprites/player/character_walk/down_4.png" } },

        { "left", new[] { "/assets/sprites/player/character_walk/left_1.png", "/assets/sprites/player/character_walk/left_2.png",
                          "/assets/sprites/player/character_walk/left_3.png", "/assets/sprites/player/character_walk/left_4.png" } },

        { "right", new[] { "/assets/sprites/player/character_walk/right_1.png", "/assets/sprites/player/character_walk/right_2.png",
                           "/assets/sprites/player/character_walk/right_3.png", "/assets/sprites/player/character_walk/right_4.png" } }
    };

    public Player(int startX, int startY)
    {
        X = startX;
        Y = startY;
    }

    public int X { get; private set; }
    public int Y { get; private set; }
    public string? Direction { get; private set; } = "down";

    public string GetCurrentSprite()
    {
        if (Direction != null) return Animations[Direction][_animationFrame];
        return Animations["down"][_animationFrame]; // Default to down
    }

    public void Move(string? direction, Maze maze)
    {
        int newX = X, newY = Y;
        Direction = direction;

        switch (direction?.ToLower())
        {
            case "up": newY -= 1; break;
            case "down": newY += 1; break;
            case "left": newX -= 1; break;
            case "right": newX += 1; break;
        }

        if (maze.IsWalkable(newX, newY))
        {
            X = newX;
            Y = newY;
            Animate();

            // 🔹 Try picking up an item after moving
            TryPickupItem(maze);
        }
    }

    private void Animate()
    {
        _frameCounter++;
        if (_frameCounter >= AnimationSpeed)
        {
            _animationFrame = (_animationFrame + 1) % 4;
            _frameCounter = 0;
        }
    }

    private void TryPickupItem(Maze maze)
    {
        var item = maze.ItemGrid.PickupItem(X, Y); // ✅ Get item at player's position

        if (item != null)
        {
            ApplyItemEffect(item); // 🔹 Apply effect

            if (item.Walkable)
            {
                Console.WriteLine($"🔄 {item.Name} remains on the grid (walkable item).");
            }
        }
    }

    public void ApplyItemEffect(Item item)
    {
        switch (item.Effect)
        {
            case ItemEffect.Heal:
                Heal(10);
                Console.WriteLine("❤️ Player healed!");
                break;

            case ItemEffect.Unlock:
                Console.WriteLine("🔑 Unlocking a door...");
                break;

            case ItemEffect.Teleport:
                Console.WriteLine("⚡ Player stepped on a teleport circle!");
                TeleportToRandomLocation();
                break;

            case ItemEffect.LightRadiusIncrease:
                Console.WriteLine("💡 Increased light radius!");
                IncreaseVisionRadius();
                break;

            case ItemEffect.ShowDirection:
                Console.WriteLine("🧭 Compass effect triggered!");
                break;

            case ItemEffect.Damage:
                Console.WriteLine("💀 Player hit a trap!");
                TakeDamage(5);
                break;

            default:
                Console.WriteLine($"⚠️ No effect for item: {item.Name}");
                break;
        }
    }

    // TODO: Implement these methods with HUD integration
    // Placeholder methods for future integration
    private void Heal(int amount)
    {
        Console.WriteLine($"❤️ Restored {amount} HP.");
    }

    private void TakeDamage(int amount)
    {
        Console.WriteLine($"💀 Lost {amount} HP.");
    }

    private void TeleportToRandomLocation()
    {
        Console.WriteLine("⚡ Teleported to a new location!");
    }

    private void IncreaseVisionRadius()
    {
        Console.WriteLine("💡 Light radius increased!");
    }
}
