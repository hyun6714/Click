using UnityEngine;

public class BuildingDestroyerEnemy : EnemyBase
{
    [Header("건물 파괴자 특화 설정")]
    [SerializeField] private int extraDamageMultiplier = 2;

    protected override void MoveOrAttack()
    {
        if (target == null)
        {
            SetMoveAnimation(false);
            return;
        }

        Vector3 targetWorldPos = target.position;
        float distance = Vector2.Distance(transform.position, targetWorldPos);

        if (distance <= enemyData.attackRange)
        {
            SetMoveAnimation(false);

            if (attackCooldown <= 0)
            {
                float currentAttackSpeed = baseAttackSpeed;

                if (currentAttackSpeed <= 0f)
                {
                    attackCooldown = 1f;
                    return;
                }

                FaceDirection(targetWorldPos - transform.position);
                PlayAttackAnimation();
                attackCooldown = 1f / currentAttackSpeed;
            }
        }
        else
        {
            SetMoveAnimation(true);
            Move(targetWorldPos);
        }
    }

    protected override void Attack()
    {
        int finalDamage = Mathf.RoundToInt(baseAttackDamage);

        if (target != null)
        {
            if (sharedBuildingTilemap != null)
            {
                Vector3Int cellPos = sharedBuildingTilemap.WorldToCell(target.position);


                EventManager.TriggerRequestDamageBuilding(cellPos, finalDamage);
            }
            else if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(finalDamage);
            }
        }
    }
}