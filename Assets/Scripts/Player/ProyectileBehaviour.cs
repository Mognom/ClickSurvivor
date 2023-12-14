using MognomUtils;
using UnityEngine;

public class ProyectileBehaviour : MonoBehaviour {

    [SerializeField] private float speed;
    [SerializeField] private float damage;

    private EnemyBehaviour target;

    public float GetDamage() {
        return damage;
    }

    private void Update() {
        if (target != null) {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (target.gameObject.Equals(collision.gameObject)) {
            target.ReceiveDamage(damage);
            this.gameObject.Recycle();
        }
    }

    public void SetTarget(EnemyBehaviour target) {
        this.target = target;
    }

    private void OnDisable() {
        target = null;
    }
}
