using UnityEngine;

public class BossEnemy : EnemyBase
{
    public void InitializeBoss(EnemyData data, Vector3 spawnPos, Transform target)
    {
        base.Initialize(data, spawnPos, target);
    }

    protected override void Die()
    {
        base.Die();
    }
}