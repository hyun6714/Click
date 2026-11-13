using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [Header("원거리 발사 위치")]
    [SerializeField] private Transform firePoint;

    [SerializeField] private float floatSpeed = 5f;
    [SerializeField] private float floatAmount = 0.1f;


    public override void Initialize(EnemyData data, Vector3 spawnPosition, Transform castleTarget)
    {
        base.Initialize(data, spawnPosition, castleTarget);

        hasBuildingTarget = false;
        currentBuildingCell = Vector3Int.zero;
    }

    private void LateUpdate()
    {
        if (enemyData == null || !gameObject.activeSelf)
        {
            return;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.transform.localRotation = Quaternion.identity;

            float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
            Vector3 localPos = spriteRenderer.transform.localPosition;

            spriteRenderer.transform.localPosition = new Vector3(localPos.x, floatOffset, localPos.z);
        }
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
