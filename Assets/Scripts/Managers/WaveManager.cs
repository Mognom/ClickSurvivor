using MognomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : Singleton<WaveManager> {

    private List<EnemyBehaviour> enemyList;
    [SerializeField] private EnemyBehaviour smallEnemyPrefab;
    [SerializeField] private EnemyBehaviour bigEnemyPrefab;
    [SerializeField] private EnemyBehaviour giantEnemyPrefab;

    [SerializeField] private int enemyPoints;

    [SerializeField] private float spawnTime;
    // Radious of the circle around the player (0,0) where enemies are allowed to spawn 
    [SerializeField] private float spawnRangeRadious;

    [SerializeField] private IntEventChannel waveOverChannel;
    private float spawnRate;
    private float timeSinceLastEnemy;

    private int remainingEnemyPoints;

    private int smallCount;
    private int bigCount;
    private int giantCount;

    private AudioSource audioSource;

    private void Start() {
        enemyList = new List<EnemyBehaviour>();
        remainingEnemyPoints = enemyPoints * GameStateManager.I.CurrentWave;

        spawnRate = spawnTime / remainingEnemyPoints;

        audioSource = GetComponent<AudioSource>();
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
                    if (bigCount / 2 > giantCount && remainingEnemyPoints >= 4) {
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
        enemy.OnDeath -= EnemyDied;
        enemyList.Remove(enemy);

        if (remainingEnemyPoints <= 0 && enemyList.Count <= 0) {
            waveOverChannel.PostEvent(enemyPoints * GameStateManager.I.CurrentWave);
        }

        // TODO move to a global enemyAudio script
        audioSource.Play();
    }

    public List<EnemyBehaviour> GetCurrentEnemies() {
        return enemyList;
    }
    public EnemyBehaviour GetBestTarget() {
        List<EnemyBehaviour> enemies = WaveManager.I.GetCurrentEnemies();
        Vector3 playerPosition = PlayerHealth.I.transform.position;
        // Removed the ones that already are going to die
        enemies = enemies.FindAll(x => x.CanTakeMoreDamage());
        // Sort them and get the closest one
        enemies.Sort((x, y) => Calculate2DSquareDistance(x.transform.position, playerPosition).CompareTo(Calculate2DSquareDistance(y.transform.position, playerPosition)));
        return enemies.FirstOrDefault();
    }

    private float Calculate2DSquareDistance(Vector3 p0, Vector3 p1) {
        return Mathf.Pow(p0.x - p1.x, 2) + Mathf.Pow(p0.y - p1.y, 2);
    }
}
