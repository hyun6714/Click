using UnityEngine;

public class MagicTower : AttackBuilding
{
    [Header("마법 타워 타입 선택")]
    [SerializeField] private bool isSlowMagic = false;       // 이속 감소 마법 타워
    [SerializeField] private bool isDotMagic = false;       // 도트 데미지 마법 타워
    [SerializeField] private bool isHeavyAreaMagic = false; // 강한 범위공격 마법 타워

    [Header("마법 상세 옵션 설정")]
    [SerializeField] private float magicSplashRadius = 2.0f; // 범위 마법일 때 폭발 반경
    [SerializeField] private float slowAmount = 0.3f;        // 슬로우 비율
    [SerializeField] private float slowDuration = 2.0f;      // 슬로우 지속 시간
    [SerializeField] private float dotDamage = 2.0f;         // 틱당 도트 데미지
    [SerializeField] private float dotDuration = 3.0f;       // 도트 지속 시간

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

            float radius = 0f;
            float sAmt = 0f, sDur = 0f;
            bool dFlag = false;
            float dDmg = 0f, dDur = 0f;

            if (isSlowMagic)
            {
                radius = magicSplashRadius;
                sAmt = slowAmount;
                sDur = slowDuration;
            }
            else if (isDotMagic)
            {
                dFlag = true;
                dDmg = dotDamage;
                dDur = dotDuration;
            }
            else if (isHeavyAreaMagic)
            {
                radius = magicSplashRadius;
            }

            projectile.InitializeMagic(target, damage, TowerData.projectileSpeed, radius, sAmt, sDur, dFlag, dDmg, dDur);
        }
        else
        {
            Destroy(projObj);
        }
    }
}