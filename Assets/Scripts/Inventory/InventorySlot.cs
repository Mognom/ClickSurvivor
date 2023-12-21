using System.Collections.Generic;
using static Item;

public class InventorySlot {
    public Item Item { get; set; }
    private List<InventoryStatsModifier> modifiers;

    public InventorySlot(Item item) {
        this.Item = item;
        modifiers = new List<InventoryStatsModifier>();
    }

    public List<InventoryStatsModifier> GetModifiers() {
        return modifiers;
    }

    public float GetModifiersTotal() {
        float result = 1;
        foreach (var modifier in modifiers) {
            result *= modifier.EffectMultiplier;
        }
        return result;
    }

    public Item GetItem() {
        return Item;
    }

    public void AddModifier(InventoryStatsModifier modifier) {
        // Check if modifier is already in the list
        modifiers.Add(modifier);
    }

    public void RemoveModifier(InventoryStatsModifier modifier) {
        modifiers.Remove(modifier);
    }
}