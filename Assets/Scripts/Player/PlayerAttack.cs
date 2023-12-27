using MognomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static Item;

public class PlayerAttack : MonoBehaviour {

    [SerializeField] private ProyectileBehaviour proyectilePrefab;
    private PlayerInputActions playerInputActions;

    [SerializeField] private MultiProgressBar progressBar;

    // Stats
    [SerializeField] private int clicksPerAttack;
    [SerializeField] private int baseAttackDamage;
    private float attackDamage;
    private float increasePerClick = 1f;
    private float clicksPerSecond;


    private float currentClicks;

    private AudioSource attackAudioSource;

    private void Awake() {
        // Setup input
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.PlayerClick.performed += OnPlayerClick;

        currentClicks = 0;
        attackAudioSource = GetComponent<AudioSource>();
    }

    private void OnDisable() {
        playerInputActions.Player.PlayerClick.performed -= OnPlayerClick;
    }

    private void Start() {
        attackDamage = baseAttackDamage;
        ApplyStatsModifiers();
    }

    private void ApplyStatsModifiers() {
        if (InventoryManager.I) {
            PlayerStatsModifier statsModifier = InventoryManager.I.GetAggregatedInventoryItemEffects().PlayerStats;
            // Update the stats the base player would use
            clicksPerSecond = statsModifier.ClicksPerSecond;
            attackDamage += statsModifier.DamageIncrease;
            attackDamage *= 1 + statsModifier.DamageMultiplier;

            increasePerClick = 1 + statsModifier.ClickMultiplier;
        }
    }

    public void Update() {
        float oldCurrentClicks = currentClicks;

        if (currentClicks >= clicksPerAttack) {
            EnemyBehaviour target = FindBestTarget();
            if (target != null) {
                ProyectileBehaviour newProyectile = proyectilePrefab.Spawn(transform.position, Quaternion.identity);
                newProyectile.SetTargetAndDamage(target, attackDamage);
                target.IncreaseIncommingDamage(attackDamage);
                attackAudioSource.Play();
                currentClicks -= clicksPerAttack;
            }
        }

        // Add autoclicks
        currentClicks += clicksPerSecond * Time.deltaTime;
        //if (currentClicks != oldCurrentClicks) { 
        progressBar.SetValue(currentClicks / clicksPerAttack);
        //}
    }

    private void OnPlayerClick(InputAction.CallbackContext context) {
        currentClicks += increasePerClick;
        progressBar.SetValue(currentClicks / clicksPerAttack);
        progressBar.PlayEffect();
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
