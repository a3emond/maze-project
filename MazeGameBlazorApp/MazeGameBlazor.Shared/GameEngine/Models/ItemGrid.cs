using MazeGameBlazor.Shared.GameEngine.Utils;

namespace MazeGameBlazor.Shared.GameEngine.Models
{
    public class ItemGrid
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private List<Item> Items { get; set; }

        public ItemGrid(int logicalWidth, int logicalHeight)
        {
            Width = logicalWidth * 5;
            Height = logicalHeight * 5;
            Items = new List<Item>();
        }

        public void AddItem(ItemName name, int x, int y, string sprite,
                            bool walkable, bool interactable, bool collectible,
                            ItemEffect effect)
        {
            Items.Add(new Item(name, x, y, sprite, walkable, interactable, collectible, effect));
        }

        public void RemoveItem(int x, int y)
        {
            Items.RemoveAll(item => item.X == x && item.Y == y);
        }

        public Item? GetItemAt(int x, int y)
        {
            return Items.FirstOrDefault(item => item.X == x && item.Y == y);
        }

        public List<Item> GetAllItems()
        {
            return Items;
        }

        public bool IsItemCollectible(int x, int y) => GetItemAt(x, y)?.Collectible ?? false;

        public bool IsItemInteractable(int x, int y) => GetItemAt(x, y)?.Interactable ?? false;

        public bool IsItemWalkable(int x, int y) => GetItemAt(x, y)?.Walkable ?? false;

        public ItemEffect GetItemEffect(int x, int y) => GetItemAt(x, y)?.Effect ?? ItemEffect.None;

        public void GenerateItems(Maze maze)
        {
            var itemsToGenerate = new List<(ItemName name, bool walkable, bool interactable, bool collectible, ItemEffect effect, int count)>
            {
                (ItemName.Key, false, false, true, ItemEffect.Unlock, 1),
                (ItemName.Potion, false, true, true, ItemEffect.Heal, 10),
                (ItemName.Lantern, false, false, true, ItemEffect.LightRadiusIncrease, 3),
                (ItemName.Compass, false, false, true, ItemEffect.ShowDirection, 1),
                (ItemName.TeleportCircle, true, false, false, ItemEffect.Teleport, 20),
                (ItemName.Trap, true, false, false, ItemEffect.Damage, 200)
            };

            SpawnItemsFromList(maze, itemsToGenerate);
        }

        public void SpawnItemsFromList(Maze maze, List<(ItemName name, bool walkable, bool interactable, bool collectible, ItemEffect effect, int count)> itemList)
        {
            PlaceStartAndGoal(maze);
            var shuffledWalkableTiles = MazeUtils.ShuffleWalkableTiles(maze);
            int tileIndex = 0;

            foreach (var item in itemList)
            {
                for (int i = 0; i < item.count; i++)
                {
                    if (tileIndex >= shuffledWalkableTiles.Count)
                    {
                        return;
                    }

                    var position = shuffledWalkableTiles[tileIndex];
                    tileIndex++;

                    var sprite = Item.GetSprite(item.name);
                    AddItem(item.name, position.x, position.y, sprite,
                        item.walkable, item.interactable, item.collectible, item.effect);
                }
            }
        }

        public void PlaceStartAndGoal(Maze maze)
        {
            var start = MazeUtils.FindStartPosition(maze);
            var goal = MazeUtils.FindGoalPosition(maze, start);
            var startSprite = Item.GetSprite(ItemName.Start);
            var goalSprite = Item.GetSprite(ItemName.Goal);
            AddItem(ItemName.Start, start.Item1, start.Item2, startSprite, false, false, false, ItemEffect.None);
            AddItem(ItemName.Goal, goal.Item1, goal.Item2, goalSprite, false, false, false, ItemEffect.None);
        }

        public Item? PickupItem(int x, int y)
        {
            var item = GetItemAt(x, y);

            if (item == null)
                return null;

            if (item.Collectible)
            {
                Items.Remove(item);
                Console.WriteLine($"Picked up: {item.Name}");
                return item;
            }

            return item;
        }
    }
}
