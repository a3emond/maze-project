namespace MazeGameBlazor.GameEngine
{
    public class Item
    {
        private static readonly Dictionary<ItemName, string> SpritePaths = new()
            {
                { ItemName.Key, "assets/sprites/items/keys/keys_1_1.png" },
                { ItemName.Potion, "assets/sprites/items/potions/potion_large_red.png" },
                { ItemName.Lantern, "assets/sprites/items/torch.png" },
                { ItemName.Compass, "assets/sprites/items/compas.png" },
                { ItemName.Door, "assets/sprites/items/door/door_1.png" },
                { ItemName.TeleportCircle, "assets/sprites/items/portal.png" },
                { ItemName.Trap, "assets/sprites/items/peaks/peaks_1.png" }
            };

        public ItemName Name { get; set; } // Item name (e.g., "Key", "Potion")
        public int X { get; set; } // Grid X position
        public int Y { get; set; } // Grid Y position
        public string Sprite { get; set; } // Path to sprite/image
        public bool Interactable { get; set; } // Can the player interact with it?
        public bool Collectible { get; set; } // Does it go into inventory?
        public bool Walkable { get; set; } // Can the player walk on it?
        public ItemEffect Effect { get; set; } // Effect of the item

        public Item(ItemName name, int x, int y, string sprite, bool walkable = false,
            bool interactable = true, bool collectible = false,
            ItemEffect effect = ItemEffect.None)
        {
            Name = name;
            X = x;
            Y = y;
            Sprite = sprite;
            Walkable = walkable;
            Interactable = interactable;
            Collectible = collectible;
            Effect = effect;
        }

        public static string GetSprite(ItemName name) // Get sprite path based on item name
        {
            return SpritePaths.GetValueOrDefault(name, "assets/sprites/items/unknown.png"); 
        }
    }



    public enum ItemEffect
    {
        None,        // No effect (default)
        Heal,        // Restore HP
        Damage,      // Inflict damage
        LightRadiusIncrease, // Increase light radius
        Unlock,      // Unlock a door or mechanism
        ShowDirection,// Compass effect to show direction
        Teleport    // Teleport to another location
    }

    public enum ItemName
    {
        Key,
        Potion,
        Lantern,
        Compass,
        Door,
        TeleportCircle, // walkable
        Trap, // walkable
    }



}
