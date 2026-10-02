using UnityEngine;

public enum CurrencyType
{
    Gold, 
    Tree,
    Rock,
    Click
}

[System.Serializable]
public class CurrencyData
{
    public CurrencyType type;
    public string currencyName;    // 재화 이름
    public Sprite icon;            // UI에 띄울 아이콘 이미지
    public int initialAmount = 0;
}
