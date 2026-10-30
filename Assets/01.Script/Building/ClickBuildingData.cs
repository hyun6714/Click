using UnityEngine;

[CreateAssetMenu(fileName = "NewClickBuildingData", menuName = "Building/Click Building Data")]
public class ClickBuildingData : BuildingData
{
    [Header("클릭 최대치 증가")]
    public int maxClickBonus = 10;

    public GameObject clickMaxPrefab;
}