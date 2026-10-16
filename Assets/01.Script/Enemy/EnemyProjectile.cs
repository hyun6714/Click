using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage;
    private Transform target;
    private float speed;
    private GameObject projectilePrefab; // 투사체 프리펩

    public void Initialize(Transform targetTransform, int attackDamage, float projectileSpeed, GameObject prefabRef)
    {
        target = targetTransform;
        damage = attackDamage;
        speed = projectileSpeed;
        projectilePrefab = prefabRef;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (target == null || !gameObject.activeSelf)
        {
            ReturnToPool();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);

        EventManager.TriggerRequestReturnProjectile(projectilePrefab, gameObject);
    }
}