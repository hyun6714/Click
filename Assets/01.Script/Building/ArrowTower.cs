using UnityEngine;

public class ArrowTower : AttackBuilding
{
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

            projectile.Initialize(target, damage, TowerData.projectileSpeed);
        }
        else
        {
            Debug.LogError($"[{TowerData.buildingName}] 프리팹에 TowerProjectile 스크립트가 없습니다!");
            Destroy(projObj);
        }
    }
}