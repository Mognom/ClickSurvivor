using UnityEngine;
using Random = UnityEngine.Random;

public class LootManager : MonoBehaviour {

    [SerializeField] private IntEventChannel waveOverChannel;
    [SerializeField] private Item[] items;

    private void Awake() {
        waveOverChannel.Channel += OnWaveOver;
    }

    private void OnWaveOver(int enemyPoints) {
        int i0 = Random.Range(0, items.Length);
        int i1 = Random.Range(0, items.Length);
        int i2 = Random.Range(0, items.Length);
        InventoryManager.I.AddLoot(items[i0], items[i1], items[i2]);
    }
}
