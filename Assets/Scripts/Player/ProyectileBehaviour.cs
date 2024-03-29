using MognomUtils;
using UnityEngine;

public class ProyectileBehaviour : MonoBehaviour {

    [SerializeField] private float speed;
    private float damage;

    private EnemyBehaviour target;

    public float GetDamage() {
        return damage;
    }

    private void Update() {
        if (target != null && target.isActiveAndEnabled) {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        } else {
            this.Recycle();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (target.gameObject.Equals(collision.gameObject)) {
            target.ReceiveDamage(damage);
            this.Recycle();
        }
    }

    public void SetTargetAndDamage(EnemyBehaviour target, float damage) {
        this.target = target;
        this.damage = damage;
        // Set the right rotation towards the target
        transform.right = transform.position - target.transform.position;
    }

    private void OnDisable() {
        target = null;
    }
}
