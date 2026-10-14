using UnityEngine;

[CreateAssetMenu(fileName = "TrapData", menuName = "Building/TrapData")]
public class TrapBuildingData : BuildingData
{
    [Header("함정 전용 설정")]
    public int damage = 30;
    public float triggerRange = 3.0f; // 발동 범위

    public GameObject trapPrefab;
}