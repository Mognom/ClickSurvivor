using MognomUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : Singleton<WaveManager> {

    private List<EnemyBehaviour> enemyList;
    [SerializeField] private EnemyBehaviour smallEnemyPrefab;
    [SerializeField] private EnemyBehaviour bigEnemyPrefab;
    [SerializeField] private EnemyBehaviour giantEnemyPrefab;

    [SerializeField] private int difficultyLevel;
    [SerializeField] private int enemyPoints;

    [SerializeField] private float spawnTime;
    // Radious of the circle around the player (0,0) where enemies are allowed to spawn 
    [SerializeField] private float spawnRangeRadious;

    [SerializeField] private FloatEventChannel waveOverChannel;
    private float spawnRate;
    private float timeSinceLastEnemy;

    private int remainingEnemyPoints;

    private int smallCount;
    private int bigCount;
    private int giantCount;

    protected override void Awake() {
        base.Awake();
        enemyList = new List<EnemyBehaviour>();
        remainingEnemyPoints = enemyPoints * difficultyLevel;

        spawnRate = spawnTime / remainingEnemyPoints;
    }

    // Update is called once per frame
    private void Update() {
        if (remainingEnemyPoints > 0) {
            timeSinceLastEnemy += Time.deltaTime;
            if (timeSinceLastEnemy > spawnRate) {
                Vector3 spawnLocation = (Quaternion.Euler(0, 0, Random.Range(0, 360f)) * Vector2.up) * spawnRangeRadious;
                spawnLocation.z = 0;

                EnemyBehaviour newEnemy;
                if (smallCount / 2 > bigCount && remainingEnemyPoints >= 2) {
                    if (bigCount  / 2 > giantCount && remainingEnemyPoints >= 4) {
                        newEnemy = giantEnemyPrefab.Spawn(spawnLocation, Quaternion.identity);
                        remainingEnemyPoints -= 4;
                        giantCount++;
                    } else {
                        newEnemy = bigEnemyPrefab.Spawn(spawnLocation, Quaternion.identity);
                        remainingEnemyPoints -= 2;
                        bigCount++;
                    }
                } else {
                    newEnemy = smallEnemyPrefab.Spawn(spawnLocation, Quaternion.identity);
                    remainingEnemyPoints -= 1;
                    smallCount++;
                }
                newEnemy.OnDeath += EnemyDied;
                enemyList.Add(newEnemy);

                timeSinceLastEnemy = 0;
            }
        }
    }

    private void EnemyDied(EnemyBehaviour enemy) {
        enemyList.Remove(enemy);

        if (enemyList.Count <= 0) {
            waveOverChannel.PostEvent(difficultyLevel);

            // TODO remove in favor of LootManager / SceneManager / GameManager

            SceneManager.LoadScene(1);
        }
    }

    public List<EnemyBehaviour> GetCurrentEnemies() {
        return enemyList;
    }
}
