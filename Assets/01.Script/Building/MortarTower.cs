using UnityEngine;

public class MortarTower : AttackBuilding
{
    [Header("박격포 전용 설정")]
    [SerializeField] private float splashRadius = 2.5f;

    protected override void Attack(Transform target)
    {
        if (target == null || TowerData.projectilePrefab == null)
        {
            return;
        }

        if (Animator != null)
        {
            Animator.SetTrigger("Attack");
        }

        Vector3 spawnPos = FirePoint != null ? FirePoint.position : transform.position;

        GameObject projObj = Instantiate(TowerData.projectilePrefab, spawnPos, Quaternion.identity);
        TowerProjectile projectile = projObj.GetComponent<TowerProjectile>();

        if (projectile != null)
        {
            int damage = Mathf.RoundToInt(GetCurrentAttackDamage());

            projectile.InitializeMortar(target, damage, TowerData.projectileSpeed, splashRadius);
        }
        else
        {
            Debug.LogError($"[{TowerData.buildingName}] 프리팹에 TowerProjectile 스크립트가 없습니다!");
            Destroy(projObj);
        }
    }
}