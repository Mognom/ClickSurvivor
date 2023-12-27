using System.Collections.Generic;
using UnityEngine;
using static Item;

public class Inventory {
    private readonly int width;
    private readonly int height;

    private InventorySlot[,] inventoryItems;

    public Inventory(int width, int height) {
        this.width = width;
        this.height = height;
        inventoryItems = new InventorySlot[height, width];

        // Initialize inventoryItems with InventorySlot
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                inventoryItems[y, x] = new InventorySlot(null);
            }
        }
    }

    // Update the list of modifiers that an Item has in a 3x3 square centered in x,y
    private void UpdateModifiersFrom(int x, int y, Item newItem, Item oldItem) {

        InventorySlot currentSlot = inventoryItems[y, x];
        if (currentSlot == null) {
            return;
        }

        // Remove the oldItem modifiers
        if (oldItem != null) {
            // Remove the oldItem modifiers
            foreach (Vector2Int tiles in oldItem.InventoryStatModifier.AffectedTiles.GetSurroundingTilesList()) {
                if (x + tiles.x < 0 || x + tiles.x >= width || y + tiles.y < 0 || y + tiles.y >= height) {
                    continue;
                }
                // Update the affected tile effect list
                inventoryItems[y + tiles.y, x + tiles.x].RemoveModifier(oldItem.InventoryStatModifier);
            }
        }

        if (newItem != null) {
            // Add the newItem modifiers
            foreach (Vector2Int tiles in newItem.InventoryStatModifier.AffectedTiles.GetSurroundingTilesList()) {
                if (x + tiles.x < 0 || x + tiles.x >= width || y + tiles.y < 0 || y + tiles.y >= height) {
                    continue;
                }
                // Update the affected tile effect list
                inventoryItems[y + tiles.y, x + tiles.x].AddModifier(newItem.InventoryStatModifier);
            }
        }
    }

    public bool InsertItem(Item item) {
        // Insert the item to inventoryItems in the first free position
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (inventoryItems[y, x].Item == null) {
                    inventoryItems[y, x].Item = item;
                    UpdateModifiersFrom(x, y, item, null);
                    return true;
                }
            }
        }
        return false;
    }

    public bool InsertItem(Item item, int x, int y) {
        // Verify that x and y are within the bounds of the inventoryItems array
        if (x < 0 || x >= width || y < 0 || y >= height) {
            return false;
        }

        // Insert the item to inventoryItems in the given position if the position is null
        if (inventoryItems[y, x].Item == null) {
            inventoryItems[y, x].Item = item;
            UpdateModifiersFrom(x, y, item, null);
            return true;
        }
        return false;
    }

    public bool SwapItemAt(Item item, out Item currentItem, int x, int y) {
        // Verify that x and y are within the bounds of the inventoryItems array
        if (x < 0 || x >= width || y < 0 || y >= height) {
            currentItem = item;
            return false;
        }

        // Swap the item at the given position with the item in the inventoryItems array
        currentItem = inventoryItems[y, x].Item;
        inventoryItems[y, x].Item = item;
        UpdateModifiersFrom(x, y, item, currentItem);
        return true;
    }

    public Item GetItemAt(int x, int y) {
        // Verify that x and y are within the bounds of the inventoryItems array
        if (x < 0 || x >= width || y < 0 || y >= height) {
            return null;
        }

        // Return the item at the given position
        InventorySlot slot = inventoryItems[y, x];
        if (slot != null) {
            return slot.Item;
        } else {
            return null;
        }
    }

    public InventorySlot GetSlotAt(int x, int y) {
        // Verify that x and y are within the bounds of the inventoryItems array
        if (x < 0 || x >= width || y < 0 || y >= height) {
            return null;
        }

        // Return the Slot at the given position
        return inventoryItems[y, x];
    }

    public Item PopItemAt(int x, int y) {
        // Verify that x and y are within the bounds of the inventoryItems array
        if (x < 0 || x >= width || y < 0 || y >= height) {
            return null;
        }

        // Pop the item at the given position and return it
        InventorySlot slot = inventoryItems[y, x];
        if (slot != null) {
            Item item = inventoryItems[y, x].Item;
            inventoryItems[y, x].Item = null;
            UpdateModifiersFrom(x, y, null, item);
            return item;
        }
        return null;
    }

    // Empty out the inventory
    public void Clear() {
        inventoryItems = new InventorySlot[height, width];
    }

    public AggregatedInventoryItemEffects GetAggregatedPlayerStats() {
        PlayerStatsModifier stats = new PlayerStatsModifier();
        List<SpawnableItemEffects> spawnableItems = new List<SpawnableItemEffects>();

        foreach (InventorySlot slot in inventoryItems) {
            if (slot.Item == null) {
                continue;
            }
            PlayerStatsModifier itemStats = slot.Item.StatModifier;
            float multiplier = slot.GetModifiersTotal();
            stats += itemStats * multiplier;

            if (slot.Item.ItemPrefab != null) {
                spawnableItems.Add(new SpawnableItemEffects(slot.Item.ItemPrefab, multiplier));
            }
        }

        return new AggregatedInventoryItemEffects(stats, spawnableItems);
    }

    public struct AggregatedInventoryItemEffects {
        public PlayerStatsModifier PlayerStats { get; private set; }
        public List<SpawnableItemEffects> Items { get; private set; }

        public AggregatedInventoryItemEffects(PlayerStatsModifier playerStats, List<SpawnableItemEffects> items) {
            PlayerStats = playerStats;
            Items = items;
        }
    }

    public struct SpawnableItemEffects {
        public GameObject Prefab { get; private set; }
        public float StatsMultiplier { get; private set; }
        public SpawnableItemEffects(GameObject prefab, float statsMultiplier) {
            Prefab = prefab;
            StatsMultiplier = statsMultiplier;
        }
    }
}