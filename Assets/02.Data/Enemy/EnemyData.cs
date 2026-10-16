using UnityEngine;

public enum EnemyType
{
    Normal,
    Ranged,
    Fast,
    Tanker,
    Destroyer,
    Boss
}
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName;
    public EnemyType enemyType;
    public GameObject enemyPrefab;

    [Header("스탯")]
    public int maxHp = 50;
    public float moveSpeed = 2.0f;
    public int attackDamage = 10;
    public float attackSpeed = 1.0f;
    public float attackRange = 0.5f;

    [Header("보상")]
    public int rewardGold = 10; // 처치 시 얻는 재화

    [Header("원거리 공격 전용 설정")]
    public GameObject projectilePrefab; // 발사할 총알 프리팹
    public float projectileSpeed = 10f;
}