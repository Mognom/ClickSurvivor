using MognomUtils;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Item;

public class InventoryManager : PersistentSingleton<InventoryManager> {

    [SerializeField] private Vector2Int playerInventoryDimensions;
    [SerializeField] private Vector2Int lootInventoryDimensions;
    //[SerializeField] private SpriteRenderer heldItemRendered;

    private Inventory playerInventory;
    private Inventory lootInventory;

    private Item _currentlyHeldItem;
    private Item CurrentlyHeldItem {
        get {
            return _currentlyHeldItem;
        }
        set {
            _currentlyHeldItem = value;
            currentlyHeldItemChange?.Invoke(value);
        }
    }
    public event Action<Item> currentlyHeldItemChange;

    // TODO remove debug values
    public Item debugItem;

    protected override void Awake() {
        base.Awake();

        // Create a Inventory object with the inventoryDimensions sizes
        playerInventory = new Inventory(playerInventoryDimensions.x, playerInventoryDimensions.y);
        lootInventory = new Inventory(lootInventoryDimensions.x, lootInventoryDimensions.y);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //private void Update() {
    //    if (CurrentlyHeldItem != null) {
    //        // Draw a greyed out item on the cursor as it is being dragged
    //        heldItemRendered.gameObject.SetActive(true);
    //        heldItemRendered.sprite = CurrentlyHeldItem.ItemSprite;
    //        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        newPos.z = 0;
    //        heldItemRendered.transform.position = newPos;
    //    } else {
    //        heldItemRendered.gameObject.SetActive(false);
    //    }
    //}

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
        Debug.Log("Scene changed");
        // TODO Fully render both inventories

        //playerInventory

        // DEBUG
        // TODO remove
        lootInventory.InsertItem(debugItem, 0, 0);
        lootInventory.InsertItem(debugItem, 0, 1);
        lootInventory.InsertItem(debugItem, 0, 2);
        LocalGridsManager.I.SetSpriteLootGrid(debugItem.ItemSprite, 0);
        LocalGridsManager.I.SetSpriteLootGrid(debugItem.ItemSprite, 1);
        LocalGridsManager.I.SetSpriteLootGrid(debugItem.ItemSprite, 2);

    }

    private void CleanupLoot() {
        lootInventory.Clear();
    }

    public void OnMouseDownInventory(bool isLoot, int slotIndex) {
        if (CurrentlyHeldItem) {
            PlaceItem(isLoot, slotIndex);
        } else {
            TakeItem(isLoot, slotIndex);
        }
    }

    private void TakeItem(bool isLoot, int slotIndex) {
        Vector2Int coordinates = GetCoordinatesFromIndex(isLoot, slotIndex);
        if (isLoot) {
            CurrentlyHeldItem = lootInventory.PopItemAt(coordinates.x, coordinates.y);
        } else {
            CurrentlyHeldItem = playerInventory.PopItemAt(coordinates.x, coordinates.y);
            // Update the invetory slot border with the new setup
            SetBordersOnGrid(isLoot, coordinates.x, coordinates.y);
        }

        // If an item was taken
        if (CurrentlyHeldItem) {
            // Update the rendered slot to be null
            SetSpriteOnGrid(null, slotIndex, isLoot);
        }
    }

    private void PlaceItem(bool isLoot, int slotIndex) {
        Item targetSpot;
        Vector2Int coordinates = GetCoordinatesFromIndex(isLoot, slotIndex);
        // Take the item that (if any) in the target slot, and place the held item
        if (isLoot) {
            targetSpot = lootInventory.PopItemAt(coordinates.x, coordinates.y);
            lootInventory.InsertItem(CurrentlyHeldItem, coordinates.x, coordinates.y);
        } else {
            targetSpot = playerInventory.PopItemAt(coordinates.x, coordinates.y);
            playerInventory.InsertItem(CurrentlyHeldItem, coordinates.x, coordinates.y);
            // Update the invetory slot border with the new setup
            SetBordersOnGrid(isLoot, coordinates.x, coordinates.y);
        }

        // Redraw the sprite to show the new item placed
        SetSpriteOnGrid(CurrentlyHeldItem.ItemSprite, slotIndex, isLoot);

        Debug.Log(CurrentlyHeldItem.StatModifier.ClicksPerSecond);
        Debug.Log(playerInventory.GetAggregatedPlayerStats().PlayerStats.ClicksPerSecond);

        // Update the currently held item if there was something on the spot
        CurrentlyHeldItem = targetSpot;
    }

    private Vector2Int GetCoordinatesFromIndex(bool isLoot, int slotIndex) {
        int x, y;
        if (isLoot) {
            x = slotIndex % lootInventoryDimensions.x;
            y = slotIndex / lootInventoryDimensions.x;
        } else {
            x = slotIndex % playerInventoryDimensions.x;
            y = slotIndex / playerInventoryDimensions.x;
        }
        return new Vector2Int(x, y);
    }

    private int GetIndexFromCoordinates(bool isLoot, int x, int y) {
        int index;
        if (isLoot) {
            index = x + y * lootInventoryDimensions.x;
        } else {
            index = x + y * playerInventoryDimensions.x;
        }
        return index;
    }

    private void SetSpriteOnGrid(Sprite sprite, int index, bool isLoot) {
        if (isLoot) {
            LocalGridsManager.I.SetSpriteLootGrid(sprite, index);
        } else {
            LocalGridsManager.I.SetSpritePlayerGrid(sprite, index);
        }
    }

    private void SetBordersOnGrid(bool isLoot, int x, int y) {
        if (isLoot) {
            return;
        }

        for (int i = x - 1; i <= x + 1; i++) {
            for (int j = y - 1; j <= y + 1; j++) {
                if (i >= 0 && i < playerInventoryDimensions.x && j >= 0 && j < playerInventoryDimensions.y) {
                    bool borderEnabled = playerInventory.GetSlotAt(i, j).GetModifiers().Count > 0;
                    int index = GetIndexFromCoordinates(isLoot, i, j);
                    if (isLoot) {
                        LocalGridsManager.I.SetBorderLootGrid(borderEnabled, index);
                    } else {
                        LocalGridsManager.I.SetBorderPlayerGrid(borderEnabled, index);
                    }
                }
            }
        }
    }
}

