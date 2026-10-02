using System;

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

    //철거 버튼 누를 때
    public static event Action OnDemolishButtonClicked;

    //철거 팝업
    public static event Action OnBuildMenuPopupToggle;
    
    //재화 충분한지 확인 하는 이벤트 
    public static event Func<CurrencyType, int, bool> OnCheckCurrency;

    public static void RequestCurrencyValue(CurrencyType type) => OnRequestCurrencyValue?.Invoke(type);
    public static void TriggerBuildingSelected(BuildingData buildingData) => OnBuildingSelected?.Invoke(buildingData);
    public static void CurrencyAdded(CurrencyType type, int value) => OnCurrencyAdded?.Invoke(type, value);
    public static void CurrencyUsed(CurrencyType type, int value) => OnCurrencyUsed?.Invoke(type, value);
    public static void CurrencyChanged(CurrencyType type, int value) => OnCurrencyChanged?.Invoke(type, value);
    public static void MaxClickChanged(int maxAmount) => OnMaxClickChanged?.Invoke(maxAmount);
    public static void RequestMaxClick() => OnRequestMaxClick?.Invoke();
    public static void TriggerDemolishButtonClicked() => OnDemolishButtonClicked?.Invoke();
    public static void TriggerBuildMenuPopupToggle() => OnBuildMenuPopupToggle?.Invoke();

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

