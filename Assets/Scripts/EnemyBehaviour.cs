using MognomUtils;
using System;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    [SerializeField] private int maxHealth;
    private float currentHealth;
    private float incommingDamage;
    [SerializeField] private float speed;

    [SerializeField] private float damage;

    private Vector3 destination;

    public event Action<EnemyBehaviour> OnDeath;

    private void OnEnable() {
        destination = PlayerHealth.I.transform.position;
        currentHealth = maxHealth;
    }

    private void Update() {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        PlayerHealth.I.TakeDamage(damage);

        this.gameObject.Recycle();
    }

    // Track the number of attacks comming towards this enemy
    public void IncreaseIncommingDamage(float damage) {
        incommingDamage += damage;
    }

    public bool CanTakeMoreDamage() {
        return incommingDamage < maxHealth;
    }

    public void ReceiveDamage(float damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            this.gameObject.Recycle();
        }
    }

    public void OnDisable() {
        OnDeath?.Invoke(this);
    }


}
