namespace MazeGameBlazor.GameEngine.Services
{
    public class MazeInputManager
    {
        private readonly HashSet<string> _movementKeys = new();

        public void Press(string? direction)
        {
            if (!string.IsNullOrWhiteSpace(direction))
                _movementKeys.Add(direction);
        }

        public void Release(string? direction)
        {
            if (!string.IsNullOrWhiteSpace(direction))
                _movementKeys.Remove(direction);
        }

        public bool IsMoving => _movementKeys.Count > 0;

        public string? GetLastDirection() => _movementKeys.LastOrDefault();
    }

}
