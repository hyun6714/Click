using UnityEngine;

[CreateAssetMenu(fileName = "NewProductionBoostData", menuName = "Building/Production Boost Data")]
public class ProductionBoostBuildingData : BuildingData
{
    [Header("하루 당 얻는 클릭 증가량")]
    public int dailyProductionBonus = 10;

    public GameObject ClickBoPrefab;
}