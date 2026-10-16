using UnityEngine;

public class BuildingDestroyerEnemy : EnemyBase
{
    [Header("건물 파괴자 특화 설정")]
    [SerializeField] private int extraDamageMultiplier = 2;

    protected override void MoveOrAttack()
    {
        if (target == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= enemyData.attackRange)
        {
            if (attackCooldown <= 0)
            {
                Attack();
                attackCooldown = 1f / enemyData.attackSpeed;
            }
        }

        else
        {
            Move(target.position);
        }
    }
    protected override void Attack()
    {
        if (target != null)
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                int finalDamage = enemyData.attackDamage * extraDamageMultiplier;
                damageable.TakeDamage(finalDamage);
                Debug.Log($"[건물 파괴자] 공격 데미지: {finalDamage}");
            }
        }
    }
}