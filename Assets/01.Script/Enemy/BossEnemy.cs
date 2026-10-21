using UnityEngine;

public class BossEnemy : EnemyBase
{
    public void InitializeBoss(EnemyData data, Vector3 spawnPos, Transform target)
    {
        base.Initialize(data, spawnPos, target);
    }

    protected override void Die()
    {
        Debug.Log($"[보스 처치 완료]");

        EventManager.TriggerRequestReturnPool(enemyData.enemyType, gameObject);

        base.Die();
    }
}