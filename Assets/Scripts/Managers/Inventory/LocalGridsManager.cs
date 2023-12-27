using MognomUtils;
using UnityEngine;

public class LocalGridsManager : Singleton<LocalGridsManager> {
    [SerializeField] private InventoryButton[] playerInventoryGrid;
    [SerializeField] private InventoryButton[] lootInventoryGrid;

    public void SetSpritePlayerGrid(Sprite sprite, int i) {
        playerInventoryGrid[i].SetSprite(sprite);
    }

    public void SetSpriteLootGrid(Sprite sprite, int i) {
        lootInventoryGrid[i].SetSprite(sprite);
    }

    public void SetBorderPlayerGrid(bool state, int i) {
        playerInventoryGrid[i].SetBorderState(state);
    }

    public void SetBorderLootGrid(bool state, int i) {
        lootInventoryGrid[i].SetBorderState(state);
    }
}
