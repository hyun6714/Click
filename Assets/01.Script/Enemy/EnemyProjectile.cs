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
    public void Initialize(Transform targetTransform, int attackDamage, float projectileSpeed, GameObject prefab)
    {
        target = targetTransform;
        damage = attackDamage;
        speed = Mathf.Max(0.01f, projectileSpeed);
        projectilePrefab = prefab;
        targetsBuilding = false;

        gameObject.SetActive(true);
    }

    // 타일 셀로 관리되는 일반 건물 타겟
    public void InitializeBuilding(Vector3 targetPosition, Vector3Int buildingCell, int attackDamage, float projectileSpeed, GameObject prefab)
    {
        target = null;
        targetWorldPosition = targetPosition;
        targetBuildingCell = buildingCell;
        damage = attackDamage;
        speed = Mathf.Max(0.01f, projectileSpeed);
        projectilePrefab = prefab;
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

        Vector3 direction = (destination - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
        Debug.Log($"[디버그] 투사체 타격 발생 데미지 값: {damage}, 건물타겟여부: {targetsBuilding}");

        if (targetsBuilding)
        {
            Debug.Log($"[디버그] 건물 셀({targetBuildingCell})에 데미지 이벤트 요청 전송");
            EventManager.TriggerRequestDamageBuilding(targetBuildingCell, damage);
        }
        else
        {
            if (target != null)
            {
                Debug.Log($" 타겟팅됨 타겟 이름: {target.name}");

                if (target.TryGetComponent<IDamageable>(out var damageable))
                {
                    Debug.Log($"성에 {damage} 데미지 적용 함수 호출");
                    damageable.TakeDamage(damage);
                }
                else
                {
                    Debug.LogWarning($"타겟({target.name})에 IDamageable 컴포넌트 없음");
                }
            }
            else
            {
                Debug.LogWarning("투사체가 타겟 없음");
            }
        }
        

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);

        if (projectilePrefab != null)
        {
            EventManager.TriggerRequestReturnProjectile(projectilePrefab, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}