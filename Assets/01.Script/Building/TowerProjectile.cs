using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    private Transform target;
    private int damage;
    private float speed;
    private bool initialized;

    public void Initialize(Transform newTarget, int newDamage, float newSpeed)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) <= 0.05f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (target != null)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
            {
                damageable = target.GetComponentInParent<IDamageable>();
            }

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}