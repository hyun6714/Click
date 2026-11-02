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

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= enemyData.attackRange)
        {
            SetMoveAnimation(false);

            if (attackCooldown <= 0)
            {
                float speedMultiplier = (currentWeather == WeatherType.Storm) ? 1.2f : (currentWeather == WeatherType.Snow ? 0.8f : 1.0f);
                float currentAttackSpeed = baseAttackSpeed * speedMultiplier;

                if (currentAttackSpeed <= 0f)
                {
                    attackCooldown = 1f;
                    return;
                }

                FaceDirection(target.position - transform.position);
                PlayAttackAnimation();
                attackCooldown = 1f / currentAttackSpeed;
            }
        }

        else
        {
            SetMoveAnimation(true);
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