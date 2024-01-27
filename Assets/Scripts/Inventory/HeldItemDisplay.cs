using UnityEngine;
using static Item.InventoryStatsModifier;

public class HeldItemDisplay : MonoBehaviour {

    [SerializeField] private GameObject[] affectedIcons;
    [SerializeField] private float verticalOffset;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Start() {
        InventoryManager.I.currentlyHeldItemChange += OnCurrentlyHeldItemChange;

        foreach (GameObject affectedIcon in affectedIcons) {
            affectedIcon.SetActive(false);
        }
    }

    private void OnCurrentlyHeldItemChange(Item item) {
        if (item != null) {
            spriteRenderer.sprite = item.ItemSprite;

            SurroundingTiles tiles = item.InventoryStatModifier.AffectedTiles;

            // Disable the affected icon tiles from the last item
            foreach (GameObject affectedIcon in affectedIcons) {
                affectedIcon.SetActive(false);
            }

            // Enable the affected icon tiles for the current item
            foreach (Vector2Int tile in item.InventoryStatModifier.AffectedTiles.GetSurroundingTilesList()) {
                int affectedIconIndex = tile.y * 3 + tile.x + 4;
                affectedIcons[affectedIconIndex].SetActive(true);
            }
        }
        this.gameObject.SetActive(item != null);
    }

    public void Update() {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = 0;
        newPos.y += verticalOffset;
        transform.position = newPos;
    }
}
