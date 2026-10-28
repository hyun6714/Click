using UnityEngine;

public class TankerEnemy : EnemyBase
{
    [Header("탱커형 특화 설정")]
    [SerializeField] private int armor = 5; // 받는 데미지 감소량

    public override void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - armor, 1);

        Debug.Log($"[탱커 방어] {armor} 만큼 데미지 경감 최종 데미지: {finalDamage}");

        base.TakeDamage(finalDamage);
    }
}