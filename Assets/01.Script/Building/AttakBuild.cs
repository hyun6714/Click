using UnityEngine;

public class AttakBuild : MonoBehaviour
{
    private AttackBuildingData towerData;
    private float cooldown = 0f;

    public void Initialize(AttackBuildingData data)
    {
        towerData = data;
    }

    void Update()
    {
        if (towerData == null) return;

        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        // AttackBuildingData에 있는 사거리 사용
        Transform target = FindNearestEnemy(towerData.attackRange);
        if (target != null)
        {
            Attack(target);
            cooldown = 1f / towerData.attackSpeed;
        }
    }

    Transform FindNearestEnemy(float range)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, LayerMask.GetMask("Enemy"));
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }
        return nearest;
    }

    void Attack(Transform target)
    {
        Debug.Log($"{towerData.buildingName} 공격! 데미지: {towerData.attackDamage}");
        //적 체력 감소 
    }

    void OnDrawGizmosSelected()
    {
        if (towerData != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, towerData.attackRange);
        }
    }
}