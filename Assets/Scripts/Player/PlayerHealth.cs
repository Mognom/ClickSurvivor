using MognomUtils;
using UnityEngine;

public class PlayerHealth : Singleton<PlayerHealth> {
    [SerializeField] private int maxHealth;
    private float currentHealth;
    protected override void Awake() {
        base.Awake();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            Debug.Log("YOU LOST");
        }
    }
}
