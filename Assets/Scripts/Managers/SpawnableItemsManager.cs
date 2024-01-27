using MognomUtils;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class SpawnableItemsManager : MonoBehaviour {

    private void Start() {
        List<SpawnableItemEffects> turrets = InventoryManager.I.GetAggregatedInventoryItemEffects().SpawnableItems;
        float baseX = -1.5f;
        float baseY = 2.5f;
        foreach (SpawnableItemEffects turret in turrets) {

            float spawnX = baseX + turret.Pos.Item1;
            float spawnY = baseY - turret.Pos.Item2;
            if (turret.Pos.Item2 >= 2) {
                spawnY -= 2f;
            }
            Vector3 pos = new Vector3(spawnX, spawnY, 0);
            GameObject newTurret = turret.Prefab.Spawn(pos, Quaternion.identity);
            newTurret.GetComponent<TurretController>().SetPowerUp(turret.StatsMultiplier);
        }
    }
}
