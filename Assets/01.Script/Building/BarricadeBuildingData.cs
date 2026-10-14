using UnityEngine;

[CreateAssetMenu(fileName = "NewBarricadeData", menuName = "Building/Barricade Data")]
public class BarricadeBuildingData : BuildingData
{
    [Header("바리케이트 고유 설정")]
    public int maxHealth = 100;
}