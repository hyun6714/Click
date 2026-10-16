using UnityEngine;

public class FastEnemy : EnemyBase
{
    [Header("스피드형 특화 설정")]
    [SerializeField] private float speedMultiplier = 1.5f;

    protected override void Move(Vector3 destPosition)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            destPosition,
            enemyData.moveSpeed * speedMultiplier * Time.deltaTime
        );
    }
}