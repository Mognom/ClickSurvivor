using MognomUtils;
using UnityEngine;

public class PlayerHealth : Singleton<PlayerHealth> {
    [SerializeField] private int maxHealth;
    [SerializeField] private IntEventChannel waveOverChannel;
    private float currentHealth;
    protected override void Awake() {
        base.Awake();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            waveOverChannel.PostEvent(0);
        }
    }
}
