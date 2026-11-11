using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [Header("원거리 발사 위치")]
    [SerializeField] private Transform firePoint;

    public override void Initialize(EnemyData data, Vector3 spawnPosition, Transform castleTarget)
    {
        base.Initialize(data, spawnPosition, castleTarget);

        hasBuildingTarget = false;
        currentBuildingCell = Vector3Int.zero;
    }

    protected override void PlayAttackAnimation()
    {
        if (!HasValidTarget())
        {
            SetMoveAnimation(true);
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        EventManager.PlaySound(SoundCue.EnemyAttack);

        Attack();
    }

    protected override void Attack()
    {
        if (enemyData == null || enemyData.projectilePrefab == null)
        {  
            return; 
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        int finalDamage = GetCurrentAttackDamage();

        Debug.Log($"[원거리 공격] 계산된 최종 데미지: {finalDamage}");

        if (hasBuildingTarget && sharedBuildingTilemap != null)
        {
            if (!activeBuildingPositions.Contains(currentBuildingCell))
            {
                hasBuildingTarget = false;
                return;
            }

            Vector3 buildingWorldPos = sharedBuildingTilemap.GetCellCenterWorld(currentBuildingCell);

            EventManager.TriggerRequestSpawnBuildingProjectile(
                enemyData.projectilePrefab,
                spawnPos,
                buildingWorldPos,
                currentBuildingCell,
                finalDamage,
                enemyData.projectileSpeed
            );
            return;
        }

        if (target == null)
        {
            return;
        }

        // 주변 건물이 타겟이 아닐 때 기본 타겟인 성으로 발사
        EventManager.TriggerRequestSpawnProjectile(
            enemyData.projectilePrefab,
            spawnPos,
            target,
            finalDamage,
            enemyData.projectileSpeed
        );

        Debug.Log($"[{enemyData.enemyName}] {target.name}을 향해 투사체 발사");
    }
    private bool HasValidTarget()
    {
        if (hasBuildingTarget && sharedBuildingTilemap != null)
        {
            return activeBuildingPositions.Contains(currentBuildingCell);
        }

        return target != null && target.gameObject.activeInHierarchy;
    }

}
