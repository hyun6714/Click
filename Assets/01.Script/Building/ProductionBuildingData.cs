using UnityEngine;

public enum TargetResourceType
{
    Tree,
    Rock
}

[CreateAssetMenu(fileName = "ProductionBuilding", menuName = "Building/Production Building Data")]
public class ProductionBuildingData : BuildingData
{
    [Header("채집 전용 설정")]
    public TargetResourceType targetResourceType; // 자원 종류
    public float gatherInterval = 2f; // 채집 주기 (초)
    public int gatherRange = 5; // 채집 범위 (칸)

    public GameObject productionPrefab;
}