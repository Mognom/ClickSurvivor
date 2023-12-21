using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[CreateAssetMenu(menuName = "InventoryItem/NewItem")]
public class Item : ScriptableObject {
    [SerializeField] private float dropChance;
    public float DropChance => dropChance;
    [SerializeField] private string itemName;
    public string ItemName => itemName;
    [SerializeField] private string description;
    public string Description => description;

    // Effects on surrounding inventory items
    [SerializeField] private InventoryStatsModifier inventoryStatModifier;
    public InventoryStatsModifier InventoryStatModifier => inventoryStatModifier;

    // Passive effects on the player
    [SerializeField] private PlayerStatsModifier playerStatModifier;
    public PlayerStatsModifier StatModifier => playerStatModifier;

    // Item to be spawned in the scene when this item is active
    [SerializeField] private GameObject itemPrefab;
    public GameObject ItemPrefab => itemPrefab;

    [SerializeField] private Sprite itemSprite;
    public Sprite ItemSprite => itemSprite;


    [Serializable]
    public struct InventoryStatsModifier {
        [SerializeField] private float effectMultiplier;
        public float EffectMultiplier => effectMultiplier;

        // Which tiles are affected
        [SerializeField] private SurroundingTiles affectedTiles;
        public SurroundingTiles AffectedTiles => affectedTiles;

        [Serializable]
        public struct SurroundingTiles {
            [SerializeField] private bool topLeft;
            public Vector2Int TopLeft => topLeft ? new Vector2Int(-1, -1) : Vector2Int.zero;
            [SerializeField] private bool top;
            public Vector2Int Top => top ? new Vector2Int(0, -1) : Vector2Int.zero;
            [SerializeField] private bool topRight;
            public Vector2Int TopRight => topRight ? new Vector2Int(1, -1) : Vector2Int.zero;
            [SerializeField] private bool left;
            public Vector2Int Left => left ? new Vector2Int(-1, 0) : Vector2Int.zero;
            [SerializeField] private bool right;
            public Vector2Int Right => right ? new Vector2Int(1, 0) : Vector2Int.zero;
            [SerializeField] private bool bottomLeft;
            public Vector2Int BottomLeft => bottomLeft ? new Vector2Int(-1, 1) : Vector2Int.zero;
            [SerializeField] private bool bottom;
            public Vector2Int Bottom => bottom ? new Vector2Int(0, 1) : Vector2Int.zero;
            [SerializeField] private bool bottomRight;
            public Vector2Int BottomRight => bottomRight ? new Vector2Int(1, 1) : Vector2Int.zero;

            public List<Vector2Int> GetSurroundingTilesList() {
                List<Vector2Int> surroundingTiles = new List<Vector2Int>();
                if (topLeft) surroundingTiles.Add(TopLeft);
                if (top) surroundingTiles.Add(Top);
                if (topRight) surroundingTiles.Add(TopRight);
                if (left) surroundingTiles.Add(Left);
                if (right) surroundingTiles.Add(Right);
                if (bottomLeft) surroundingTiles.Add(BottomLeft);
                if (bottom) surroundingTiles.Add(Bottom);
                if (bottomRight) surroundingTiles.Add(BottomRight);
                return surroundingTiles;
            }
        }
    }

    [Serializable]
    public struct PlayerStatsModifier {
        // Health modifiers
        [SerializeField] private float healthMultiplier;
        public float HealthMultiplier => healthMultiplier;
        [SerializeField] private float healthIncrease;
        public float HealthIncrease => healthIncrease;

        // Attack modifiers
        [SerializeField] private float clickMultiplier;
        public float ClickMultiplier => clickMultiplier;

        [SerializeField] private float damageMultiplier;
        public float DamageMultiplier => damageMultiplier;

        [SerializeField] private float damageIncrease;
        public float DamageIncrease => damageIncrease;

        [SerializeField] private float clicksPerSecond;
        public float ClicksPerSecond => clicksPerSecond;

        // Drop chance modifiers
        [SerializeField] private float dropChanceIncrease;
        public float DropChanceIncrease => dropChanceIncrease;

        public static PlayerStatsModifier operator *(PlayerStatsModifier left, float right) {
            left.healthMultiplier *= right;
            left.clickMultiplier *= right;
            left.damageMultiplier *= right;
            left.damageIncrease *= right;
            left.clicksPerSecond *= right;
            left.dropChanceIncrease *= right;
            return left;
        }

        public static PlayerStatsModifier operator +(PlayerStatsModifier left, PlayerStatsModifier right) {
            PlayerStatsModifier result = new PlayerStatsModifier();
            result.healthMultiplier = left.healthMultiplier + right.healthMultiplier;
            result.clickMultiplier = left.clickMultiplier + right.clickMultiplier;
            result.damageMultiplier = left.damageMultiplier + right.damageMultiplier;
            result.damageIncrease = left.damageIncrease + right.damageIncrease;
            result.clicksPerSecond = left.clicksPerSecond + right.clicksPerSecond;
            result.dropChanceIncrease = left.dropChanceIncrease + right.dropChanceIncrease;
            return result;
        }
    }
}