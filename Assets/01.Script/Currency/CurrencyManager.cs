using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance { get; private set; }

    private Dictionary<CurrencyType, int> currentCurrencies = new Dictionary<CurrencyType, int>();

    [SerializeField] private CurrencyDatabase currencyDatabase;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeCurrencies();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventManager.OnCurrencyAdded += AddCurrency;
        EventManager.OnCurrencyUsed += UseCurrency;
    }

    private void OnDisable()
    {
        EventManager.OnCurrencyAdded -= AddCurrency;
        EventManager.OnCurrencyUsed -= UseCurrency;
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
            return;

        currentCurrencies[type] = GetAmount(type) + amount;

        EventManager.CurrencyChanged(type, currentCurrencies[type]);
    }

    //재화 차감 
    public bool UseCurrency(CurrencyType type, int amount)
    {
        if (amount <= 0) return false;

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
}
