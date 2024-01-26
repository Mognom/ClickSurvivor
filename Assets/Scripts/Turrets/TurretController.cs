using UnityEngine;
using MognomUtils;
using System.Security.Cryptography;

public class TurretController : MonoBehaviour {
    // Stats
    [SerializeField] private int baseAttacksChargeTime;
    [SerializeField] private int baseAttackDamage;
    [SerializeField] private ProyectileBehaviour proyectilePrefab;

    private AudioSource turretAudioSource;

    private float powerUp = 1;
    private float attackDamage;
    private float attackChargeTime;
    private float currentAttackCharge;

    private void OnEnable() {
        currentAttackCharge = 0;
        attackChargeTime = baseAttacksChargeTime / powerUp;
        attackDamage = baseAttackDamage * powerUp;
        turretAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update() {
        currentAttackCharge += Time.deltaTime;

        if (currentAttackCharge > attackChargeTime) {

            EnemyBehaviour target = WaveManager.I.GetBestTarget();
            if (target != null) {
                currentAttackCharge = 0;
                ProyectileBehaviour newProyectile = proyectilePrefab.Spawn(transform.position, Quaternion.identity);
                newProyectile.SetTargetAndDamage(target, attackDamage);
                target.IncreaseIncommingDamage(attackDamage);
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
