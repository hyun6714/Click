using UnityEngine;

[CreateAssetMenu(fileName = "AttackBuilding", menuName = "Building/Attack Building Data")]
public class AttackBuildingData : BuildingData
{
    [Header("타워 공격스텟")]
    public float attackRange = 3f; // 사거리
    public float attackDamage = 10f; // 공격력
    public float attackSpeed = 1.0f; // 공격 속도

    public GameObject projectilePrefab; // 발사체
}