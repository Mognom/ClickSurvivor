using MognomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour {

    [SerializeField] private ProyectileBehaviour proyectilePrefab;
    [SerializeField] private int clicksPerAttack;
    private int currentClicks;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        // Setup input
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.PlayerClick.performed += OnPlayerClick;

        currentClicks = 0;
    }

    private void OnPlayerClick(InputAction.CallbackContext context) {
        currentClicks++;
        if (currentClicks >= clicksPerAttack) {
            EnemyBehaviour target = FindBestTarget();
            if (target != null) {
                ProyectileBehaviour newProyectile = proyectilePrefab.Spawn(transform.position, Quaternion.identity);
                newProyectile.SetTarget(target);
                target.IncreaseIncommingDamage(newProyectile.GetDamage());
                currentClicks -= clicksPerAttack;
            }
        }
    }

    private EnemyBehaviour FindBestTarget() {
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
