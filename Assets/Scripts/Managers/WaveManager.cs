using MognomUtils;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager> {

    private List<EnemyBehaviour> enemyList;
    [SerializeField] private EnemyBehaviour baseEnemyPrefab;
    [SerializeField] private int difficultyLevel;
    [SerializeField] private int enemyPoints;

    [SerializeField] private float spawnTime;
    // Radious of the circle around the player (0,0) where enemies are allowed to spawn 
    [SerializeField] private float spawnRangeRadious;

    [SerializeField] private FloatEventChannel enemyDeathChannel;
    private float spawnRate;
    private float timeSinceLastEnemy;

    private int remainingEnemyPoints;

    protected override void Awake() {
        base.Awake();
        enemyList = new List<EnemyBehaviour> ();
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
                EnemyBehaviour newEnemy = baseEnemyPrefab.Spawn(spawnLocation, Quaternion.identity);
                newEnemy.OnDeath += EnemyDied;
                enemyList.Add(newEnemy);

                timeSinceLastEnemy = 0;
                remainingEnemyPoints--;
            }
        }
    }

    private void EnemyDied(EnemyBehaviour enemy) {
        enemyList.Remove(enemy);
    }

    public List<EnemyBehaviour> GetCurrentEnemies() {
        return enemyList;
    }
}
