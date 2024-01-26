using MognomUtils;
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
            EnemyBehaviour target = WaveManager.I.GetBestTarget();
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
}
