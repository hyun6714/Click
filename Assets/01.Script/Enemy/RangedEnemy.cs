using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [Header("원거리 발사 위치")]
    [SerializeField] private Transform firePoint;

    protected override void Attack()
    {
        if (enemyData.projectilePrefab == null)
        {
            return;
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        int finalDamage = GetCurrentAttackDamage();

        if (hasBuildingTarget && sharedBuildingTilemap != null)
        {
            Vector3 buildingWorldPos = sharedBuildingTilemap.CellToWorld(currentBuildingCell) + new Vector3(0.5f, 0.5f, 0f);

            EventManager.TriggerRequestDamageBuilding(currentBuildingCell, finalDamage);
            Debug.Log($"[{enemyData.enemyName}] 타일 건물 {currentBuildingCell}에 원거리 공격");
        }

        EventManager.TriggerRequestSpawnProjectile(
            enemyData.projectilePrefab,
            spawnPos,
            target,
            finalDamage, 
            enemyData.projectileSpeed
        );

        Debug.Log($"[{enemyData.enemyName}] {target.name}게 원거리 공격 발사");
    }
}
