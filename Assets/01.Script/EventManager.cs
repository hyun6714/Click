using System;
using UnityEngine;

public static class EventManager
{
    //재화 얻을 시
    public static event Action<CurrencyType, int> OnCurrencyAdded;

    //재화 사용 시
    public static event Func<CurrencyType, int, bool> OnCurrencyUsed;

    //재화량 변화 시
    public static event Action<CurrencyType, int> OnCurrencyChanged;

    //상점에서 건물 선택 시 
    public static event Action<BuildingData> OnBuildingSelected;

    public static void TriggerBuildingSelected(BuildingData buildingData) => OnBuildingSelected?.Invoke(buildingData);

    public static void CurrencyAdded(CurrencyType type, int value) => OnCurrencyAdded?.Invoke(type, value);
    public static void CurrencyUsed(CurrencyType type, int value) => OnCurrencyUsed?.Invoke(type, value);
    public static void CurrencyChanged(CurrencyType type, int value) => OnCurrencyChanged?.Invoke(type, value);

    //건물 비용차감 
    public static bool RequestUseCurrency(CurrencyType type, int value)
    {
        if (OnCurrencyUsed != null)
        {
            return OnCurrencyUsed.Invoke(type, value);
        }
        return false;
    }

}

