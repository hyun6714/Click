using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AttackBuilding : MonoBehaviour, IDamageable
{
    private AttackBuildingData towerData;
    private Vector3Int cellPosition;
    private int currentHealth;

    private float cooldown = 0f;

    public void Initialize(AttackBuildingData data, Vector3Int pos)
    {
        towerData = data;
        cellPosition = pos;
        currentHealth = towerData.maxBuildingHp;
    }

    void Update()
    {
        if (towerData == null)
        {
            return;
        }

        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            return;
        }

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

        if (target.TryGetComponent<IDamageable>(out var damageableTarget))
        {
            damageableTarget.TakeDamage(Mathf.RoundToInt(towerData.attackDamage));
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[타워 피격] {towerData.buildingName} 남은 체력: {currentHealth} / {towerData.maxBuildingHp}");

        if (currentHealth <= 0)
        {
            DestroyTower();
        }
    }

    private void DestroyTower()
    {
        Debug.Log($"[타워 파괴] {towerData.buildingName} 파괴 및 타일 제거");

        EventManager.TriggerRequestDemolish(cellPosition);

        Destroy(gameObject);
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