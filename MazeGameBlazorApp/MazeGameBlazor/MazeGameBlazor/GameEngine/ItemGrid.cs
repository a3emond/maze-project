﻿namespace MazeGameBlazor.GameEngine
{
    public class ItemGrid
    {
        public int Width { get; private set; } // Processed width (logical width * 5)
        public int Height { get; private set; } // Processed height (logical height * 5)
        private List<Item> Items { get; set; } // Stores all items

        public ItemGrid(int logicalWidth, int logicalHeight)
        {
            Width = logicalWidth * 5;
            Height = logicalHeight * 5;
            Items = new List<Item>();
        }

        // Add an item 
        public void AddItem(ItemName name, int x, int y, string sprite,
                            bool walkable, bool interactable, bool collectible,
                            ItemEffect effect)
        {
            Items.Add(new Item(name, x, y, sprite, walkable, interactable, collectible, effect));
        }

        // Remove an item from the grid (e.g., when picked up)
        public void RemoveItem(int x, int y)
        {
            Items.RemoveAll(item => item.X == x && item.Y == y);
        }

        // Get an item at a specific position
        public Item? GetItemAt(int x, int y)
        {
            return Items.FirstOrDefault(item => item.X == x && item.Y == y);
        }

        // Get all items (for rendering in the UI)
        public List<Item> GetAllItems()
        {
            return Items;
        }

        // Check if an item is collectible at a position
        public bool IsItemCollectible(int x, int y) => GetItemAt(x, y)?.Collectible ?? false;

        // Check if an item is interactable at a position
        public bool IsItemInteractable(int x, int y) => GetItemAt(x, y)?.Interactable ?? false;

        // Check if an item is walkable at a position
        public bool IsItemWalkable(int x, int y) => GetItemAt(x, y)?.Walkable ?? false;

        // Get the effect of an item at a position
        public ItemEffect GetItemEffect(int x, int y) => GetItemAt(x, y)?.Effect ?? ItemEffect.None;

        // Generate predefined items and store them in the grid
        public void GenerateItems(Maze maze)
        {
            // Define all items with their properties
            var itemsToGenerate = new List<(ItemName name, bool walkable, bool interactable, bool collectible, ItemEffect effect, int count)>
            {
                (ItemName.Key, false, false, true, ItemEffect.Unlock, 1),
                (ItemName.Potion, false, true, true, ItemEffect.Heal, 10),
                (ItemName.Lantern, false, false, true, ItemEffect.LightRadiusIncrease, 3),
                (ItemName.Compass, false, false, true, ItemEffect.ShowDirection, 1),
                (ItemName.TeleportCircle, true, false, false, ItemEffect.Teleport, 20),
                (ItemName.Trap, true, false, false, ItemEffect.Damage, 200)
            };

            // Call SpawnItemsFromList to add items to the grid
            SpawnItemsFromList(maze, itemsToGenerate);
        }


        // Spawn multiple items from a predefined list
        /*
         * I improved it by shuffling the walkable tiles and then
         * iterating over the items list until all required items are placed.
         * Saved many iterations and made the code more efficient.
         */
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
                        // No more walkable tiles available
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

        // place start and goal items
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
                return null; // No item at this position

            if (item.Collectible) // Only remove collectible items
            {
                Items.Remove(item);
                Console.WriteLine($"Picked up: {item.Name}");
                return item; // Return the collected item
            }

            return item; // Non-collectible items remain in the grid
        }


    }
}
