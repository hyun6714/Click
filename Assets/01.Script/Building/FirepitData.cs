using UnityEngine;

[CreateAssetMenu(fileName = "NewFirepitData", menuName = "Building/FirepitData")]
public class FirepitData : BuildingData
{
    [Header("화로 특화 설정")]
    public float effectRadius = 3.0f; // 온기 범위

    [Header("프리팹 설정")]
    public GameObject firepitPrefab;
}