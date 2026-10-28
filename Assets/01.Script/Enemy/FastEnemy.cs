using UnityEngine;

public class FastEnemy : EnemyBase
{
    [Header("스피드형 특화 설정")]
    [SerializeField] private float speedMultiplier = 1.5f;

    protected override float GetCurrentMoveSpeed()
    {
        return base.GetCurrentMoveSpeed() * speedMultiplier;
    }
}