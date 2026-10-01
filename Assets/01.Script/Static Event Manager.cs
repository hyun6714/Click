using System;
using UnityEngine;

public static class StaticEventManager
{
    //상점에서 건물 선택시 
    public static event Action<BuildingData> OnBuildingSelected;

    public static void TriggerBuildingSelected(BuildingData buildingData) => OnBuildingSelected?.Invoke(buildingData);
    
}

