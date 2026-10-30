using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [Header("클릭 최대 횟수")] 
    [SerializeField] private int maxClickLimit = 40;

    [Header("일일 생산량 보너스")]
    [SerializeField] private int dailyProductionBonus = 0;

    private Dictionary<CurrencyType, int> currentCurrencies = new Dictionary<CurrencyType, int>();

    [SerializeField] private CurrencyDatabase currencyDatabase;

    private void OnEnable()
    {
        EventManager.OnCurrencyAdded += AddCurrency;
        EventManager.OnCurrencyUsed += UseCurrency;
        EventManager.OnRequestCurrencyValue += SendCurrentValue;
        EventManager.OnCheckCurrency += CheckHasEnoughCurrency;
        EventManager.OnAddMaxClick += AddMaxClick;
        EventManager.OnAddDailyProductionBonus += AddDailyProductionBonus;
        EventManager.OnDayChanged += HandleDayChanged;
    }

    private void OnDisable()
    {
        EventManager.OnCurrencyAdded -= AddCurrency;
        EventManager.OnCurrencyUsed -= UseCurrency;
        EventManager.OnRequestCurrencyValue -= SendCurrentValue;
        EventManager.OnCheckCurrency -= CheckHasEnoughCurrency;
        EventManager.OnAddMaxClick -= AddMaxClick;
        EventManager.OnAddDailyProductionBonus -= AddDailyProductionBonus;
        EventManager.OnDayChanged -= HandleDayChanged;
    }

    void Start()
    {
        SaveData loadedData = SaveLoadManager.LoadGame();
        if (loadedData != null)
        {
            LoadCurrencyData(loadedData);
        }
        else
        {
            InitializeCurrencies();
        }
    }

    private void InitializeCurrencies()
    {
        if (currencyDatabase == null)
        {
            Debug.Log("재화 데이터베이스가 없습니다");
            return;
        }

        foreach (CurrencyData info in currencyDatabase.currencies)
        {
            currentCurrencies[info.type] = info.initialAmount;

            EventManager.CurrencyChanged(info.type, info.initialAmount);
        }
    }

    public CurrencyData GetInfo(CurrencyType type)
    {
        return currencyDatabase.GetCurrencyInfo(type);
    }

    //현재 유저가 가진 재화 수량 반환
    public int GetAmount(CurrencyType type)
    {
        if (currentCurrencies.TryGetValue(type, out int amount))
        {
            return amount;
        }

        return 0;
    }

    //재화 획득
    public void AddCurrency(CurrencyType type, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        int current = GetAmount(type);
        int newAmount = current + amount;

        if (type == CurrencyType.Click)
        {
            newAmount = Mathf.Min(newAmount, maxClickLimit);
        }

        currentCurrencies[type] = newAmount;

        EventManager.CurrencyChanged(type, currentCurrencies[type]);
    }

    //재화 차감 
    public bool UseCurrency(CurrencyType type, int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        int current = GetAmount(type);
        if (current < amount)
        {
            Debug.Log($"{type} 재화가 부족합니다");
            return false;
        }

        currentCurrencies[type] = current - amount;

        EventManager.CurrencyChanged(type, currentCurrencies[type]);

        return true;
    }

    // 재화 값 설정
    public void SetCurrency(CurrencyType type, int amount)
    {
        currentCurrencies[type] = amount;

        EventManager.CurrencyChanged(type, currentCurrencies[type]);
    }

    private void SendCurrentValue(CurrencyType type)
    {
        EventManager.CurrencyChanged(type, GetAmount(type));
    }

    public void SaveCurrencyData(SaveData data)
    {
        if (data == null)
        {
            return;
        }

        data.clickCurrency = GetAmount(CurrencyType.Click);
        data.treeCurrency = GetAmount(CurrencyType.Tree);
        data.rockCurrency = GetAmount(CurrencyType.Rock);
        data.goldCurrency = GetAmount(CurrencyType.Gold);

        data.savedMaxClickLimit = maxClickLimit;
        data.savedDailyProductionBonus = dailyProductionBonus;
    }

    public void LoadCurrencyData(SaveData data)
    {
        if (data == null)
        {
            return;
        }

        SetCurrency(CurrencyType.Click, data.clickCurrency);
        SetCurrency(CurrencyType.Tree, data.treeCurrency);
        SetCurrency(CurrencyType.Rock, data.rockCurrency);
        SetCurrency(CurrencyType.Gold, data.goldCurrency);

        maxClickLimit = data.savedMaxClickLimit;
        dailyProductionBonus = data.savedDailyProductionBonus;
        EventManager.TriggerMaxClickChanged(maxClickLimit);

        Debug.Log("[CurrencyManager] 재화 데이터 복원 완료");
    }

    public int GetMaxClick()
    {
        return maxClickLimit;
    }

    public void AddMaxClick(int amount)
    {
        maxClickLimit += amount;

        EventManager.TriggerMaxClickChanged(maxClickLimit);
    }

    private void HandleDayChanged(int currentDay)
    {
        int baseClick = 10;
        int totalClickToAdd = baseClick + dailyProductionBonus;

        AddCurrency(CurrencyType.Click, totalClickToAdd);
        Debug.Log($"[일일 생산] 총 {totalClickToAdd}");
    }

    public void AddDailyProductionBonus(int amount)
    {
        dailyProductionBonus += amount;
    }

    private bool CheckHasEnoughCurrency(CurrencyType type, int value)
    {
        return GetAmount(type) >= value;
    }
}
