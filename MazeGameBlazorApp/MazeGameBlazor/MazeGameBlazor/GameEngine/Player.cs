namespace MazeGameBlazor.GameEngine
{
    public class Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Direction { get; private set; } = "down";
        private int _animationFrame = 0;
        private int _frameCounter = 0;
        private const int _animationSpeed = 10; // Adjust for animation timing

        public Dictionary<string, string[]> Animations = new()
        {
            { "up", new[] { "/assets/sprites/player/character_walk/up_1.png", "/assets/sprites/player/character_walk/up_2.png", "/assets/sprites/player/character_walk/up_3.png", "/assets/sprites/player/character_walk/up_4.png" } },
            { "down", new[] { "/assets/sprites/player/character_walk/down_1.png", "/assets/sprites/player/character_walk/down_2.png", "/assets/sprites/player/character_walk/down_3.png", "/assets/sprites/player/character_walk/down_4.png" } },
            { "left", new[] { "/assets/sprites/player/character_walk/left_1.png", "/assets/sprites/player/character_walk/left_2.png", "/assets/sprites/player/character_walk/left_3.png", "/assets/sprites/player/character_walk/left_4.png" } },
            { "right", new[] { "/assets/sprites/player/character_walk/right_1.png", "/assets/sprites/player/character_walk/right_2.png", "/assets/sprites/player/character_walk/right_3.png", "/assets/sprites/player/character_walk/right_4.png" } }
        };

        public Player(int startX, int startY)
        {
            X = startX;
            Y = startY;
        }

        public string GetCurrentSprite()
        {
            return Animations[Direction][_animationFrame];
        }

        public void Move(string direction, HashSet<(int, int)> walkableTiles)
        {
            int newX = X, newY = Y;
            Direction = direction;

            switch (direction.ToLower())
            {
                case "up": newY -= 1; break;
                case "down": newY += 1; break;
                case "left": newX -= 1; break;
                case "right": newX += 1; break;
            }

            if (walkableTiles.Contains((newX, newY)))
            {
                X = newX;
                Y = newY;
                Animate();
            }
        }

        private void Animate()
        {
            _frameCounter++;
            if (_frameCounter >= _animationSpeed)
            {
                _animationFrame = (_animationFrame + 1) % 4;
                _frameCounter = 0;
            }
        }
    }
}
