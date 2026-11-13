using UnityEngine;

public class TankerEnemy : EnemyBase
{
    [Header("탱커형 특화 설정")]
    [SerializeField] private int armor = 5; // 받는 데미지 감소량

    public override void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - armor, 1);

        base.TakeDamage(finalDamage);
    }
}