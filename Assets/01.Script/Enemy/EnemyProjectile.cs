using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage;
    private Transform target;
    private Vector3 targetWorldPosition;
    private Vector3Int targetBuildingCell;
    private float speed;
    private GameObject projectilePrefab;

    private bool targetsBuilding;

    // 성처럼 Transform으로 표현되는 타겟
    public void Initialize(Transform targetTransform, int attackDamage, float projectileSpeed, GameObject prefabRef)
    {
        target = targetTransform;
        damage = attackDamage;
        speed = Mathf.Max(0.01f, projectileSpeed);
        projectilePrefab = prefabRef;
        targetsBuilding = false;

        gameObject.SetActive(true);
    }

    // 타일 셀로 관리되는 일반 건물 타겟
    public void InitializeBuilding(Vector3 targetPosition, Vector3Int buildingCell, int attackDamage, float projectileSpeed, GameObject prefabRef)
    {
        target = null;
        targetWorldPosition = targetPosition;
        targetBuildingCell = buildingCell;
        damage = attackDamage;
        speed = Mathf.Max(0.01f, projectileSpeed);
        projectilePrefab = prefabRef;
        targetsBuilding = true;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        Vector3 destination;

        if (targetsBuilding)
        {
            destination = targetWorldPosition;
        }
        else
        {
            if (target == null)
            {
                ReturnToPool();
                return;
            }

            destination = target.position;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, destination) <= 0.2f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (targetsBuilding)
        {
            EventManager.TriggerRequestDamageBuilding(
                targetBuildingCell,
                damage
            );
        }
        else if (target != null &&
                 target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);
        EventManager.TriggerRequestReturnProjectile(
            projectilePrefab,
            gameObject
        );
    }
}