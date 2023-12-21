using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour {
    [SerializeField] private bool isLoot;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image borderImage;

    private int index;

    private void Awake() {
        index = transform.GetSiblingIndex();
        borderImage.gameObject.SetActive(false);
    }
    public void OnButtonClick() {
        InventoryManager.I.OnMouseDownInventory(isLoot, index);
    }

    public void SetSprite(Sprite sprite) {
        itemImage.sprite = sprite;
        Color c = itemImage.color;
        c.a = sprite ? 255 : 0;
        itemImage.color = c;
    }

    public void SetBorderState(bool state) {
        borderImage.gameObject.SetActive(state);
    }
}
