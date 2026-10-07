using System;
using UnityEngine;

public static class EventManager
{
    //UI 전용 현재 재화량 가져오기 
    public static event Action<CurrencyType> OnRequestCurrencyValue;
    //재화 얻을 시
    public static event Action<CurrencyType, int> OnCurrencyAdded;
    //재화 사용 시
    public static event Func<CurrencyType, int, bool> OnCurrencyUsed;
    //재화량 변화 시
    public static event Action<CurrencyType, int> OnCurrencyChanged;

    //상점에서 건물 선택 시 
    public static event Action<BuildingData> OnBuildingSelected;

    //클릭 최대지 가져옴
    public static event Action<int> OnMaxClickChanged;
    public static event Action OnRequestMaxClick;

    //건물 설치 시 
    public delegate void RequestPlaceBuildingHandler(Vector3Int cellPosition, BuildingData buildingData);
    public static event RequestPlaceBuildingHandler OnRequestPlaceBuilding;

    //철거 버튼 누를 때
    public static event Action OnDemolishButtonClicked;

    //건물 철거 팝업
    public static event Action OnBuildMenuPopupToggle;

    //건물 설치 팝업
    public static event Action<bool> OnBuilAddPopupToggle;

    //특정 좌표 건물 철거 
    public delegate void RequestDemolishHandler(Vector3Int cellPosition);
    public static event RequestDemolishHandler OnRequestDemolish;

    //재화 충분한지 확인 하는 이벤트 
    public static event Func<CurrencyType, int, bool> OnCheckCurrency;

    //날짜 변경 이벤트(하루 바뀔 떄마다)
    public delegate void DayChangedHandler(int day);
    public static event DayChangedHandler OnDayChanged;
    
    //일,시,분,초 상세 시간 변경
    public delegate void TimeDetailedChangedHandler(int day, float hour, float minute, float second);
    public static event TimeDetailedChangedHandler OnTimeDetailedChanged;

    //날짜 스킵버튼 이벤트
    public delegate void SkipButtonClickHandler();
    public static event SkipButtonClickHandler OnSkipButtonClicked;

    public static void RequestCurrencyValue(CurrencyType type) => OnRequestCurrencyValue?.Invoke(type);
    public static void TriggerBuildingSelected(BuildingData buildingData) => OnBuildingSelected?.Invoke(buildingData);
    public static void CurrencyAdded(CurrencyType type, int value) => OnCurrencyAdded?.Invoke(type, value);
    public static void CurrencyUsed(CurrencyType type, int value) => OnCurrencyUsed?.Invoke(type, value);
    public static void CurrencyChanged(CurrencyType type, int value) => OnCurrencyChanged?.Invoke(type, value);
    public static void MaxClickChanged(int maxAmount) => OnMaxClickChanged?.Invoke(maxAmount);
    public static void RequestMaxClick() => OnRequestMaxClick?.Invoke();
    public static void TriggerRequestPlaceBuilding(Vector3Int cellPosition, BuildingData buildingData) => OnRequestPlaceBuilding?.Invoke(cellPosition, buildingData);
    public static void TriggerDemolishButtonClicked() => OnDemolishButtonClicked?.Invoke();
    public static void TriggerBuildMenuPopupToggle() => OnBuildMenuPopupToggle?.Invoke();
    public static void TriggerBuildAddPopupToggle(bool isOpen) => OnBuilAddPopupToggle?.Invoke(isOpen);
    public static void TriggerRequestDemolish(Vector3Int cellPosition) => OnRequestDemolish?.Invoke(cellPosition);
    public static void TriggerDayChanged(int day) => OnDayChanged?.Invoke(day);
    public static void TriggerTimeDetailedChanged(int day, float hour, float minute, float second) => OnTimeDetailedChanged?.Invoke(day, hour, minute, second);
    public static void TriggerSkipButtonClicked() => OnSkipButtonClicked?.Invoke();


    //건물 비용차감 
    public static bool RequestUseCurrency(CurrencyType type, int value)
    {
        if (OnCurrencyUsed != null)
        {
            return OnCurrencyUsed.Invoke(type, value);
        }
        return false;
    }

    public static bool RequestCheckCurrency(CurrencyType type, int value)
    {
        if (OnCheckCurrency != null)
        {
            return OnCheckCurrency.Invoke(type, value);
        }
        return false;
    }

}

