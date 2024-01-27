using MognomUtils;
using UnityEngine;

public class TurretController : MonoBehaviour {
    // Stats
    [SerializeField] private int baseAttacksChargeTime;
    [SerializeField] private int baseAttackDamage;
    [SerializeField] private ProyectileBehaviour proyectilePrefab;
    [SerializeField] private float rotationTime;
    [SerializeField] private float freezeAfterShootTime;

    private AudioSource turretAudioSource;

    private float powerUp = 1;
    private float attackDamage;
    private float attackChargeTime;
    private float currentAttackCharge;
    private float freezeCounter;

    private EnemyBehaviour currentTarget;
    private float rotationSpeed;
    private void OnEnable() {
        currentAttackCharge = 0;
        attackChargeTime = baseAttacksChargeTime / powerUp;
        attackDamage = baseAttackDamage * powerUp;
        turretAudioSource = GetComponent<AudioSource>();
        freezeCounter = freezeAfterShootTime;
    }

    // Update is called once per frame
    private void Update() {
        currentAttackCharge += Time.deltaTime;
        if (freezeCounter < freezeAfterShootTime) {
            freezeCounter += Time.deltaTime;
            return;
        }

        currentTarget = WaveManager.I.GetBestTarget();
        if (currentTarget != null) {
            // Face towards target
            Vector3 direction = currentTarget.transform.position - transform.position;

            float targetAngle = Vector2.SignedAngle(Vector3.up, direction);
            float newZAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref rotationSpeed, rotationTime);
            transform.rotation = Quaternion.Euler(0, 0, newZAngle);

            // Attack target if charged and facing it
            if (currentAttackCharge > attackChargeTime && Mathf.Abs(targetAngle - newZAngle) % 360 < 2) {
                currentAttackCharge = 0;
                freezeCounter = 0;
                ProyectileBehaviour newProyectile = proyectilePrefab.Spawn(transform.position, Quaternion.identity);
                newProyectile.SetTargetAndDamage(currentTarget, attackDamage);
                currentTarget.IncreaseIncommingDamage(attackDamage);
                turretAudioSource.Play();
            }
        }
    }

    public void SetPowerUp(float powerUp) {
        this.powerUp = powerUp;
        // Update the base values
        attackChargeTime = baseAttacksChargeTime / powerUp;
        attackDamage = baseAttackDamage * powerUp;
    }
}
