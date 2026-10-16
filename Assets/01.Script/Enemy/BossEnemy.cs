using UnityEngine;

public class BossEnemy : EnemyBase
{
    [Header("보스 특화 설정")]
    [SerializeField] private int bossRewardGold = 100; // 보스 처치 시 대량의 골드

    protected override void Die()
    {
        Debug.Log($"[보스 처치 완료]");

        EventManager.CurrencyAdded(CurrencyType.Gold, bossRewardGold); 

        base.Die();
    }
}