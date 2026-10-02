using UnityEngine;
using TMPro;

public class CurrencyUIManager : MonoBehaviour
{
    [Header("자원 텍스트")]
    public TextMeshProUGUI clickText;
    public TextMeshProUGUI treeText;
    public TextMeshProUGUI rockText;
    public TextMeshProUGUI goldText;

    private int currentMaxClick = 40;

    private void OnEnable()
    {
        EventManager.OnCurrencyChanged += UpdateCurrencyUI;
        EventManager.OnMaxClickChanged += UpdateMaxClickUI;
    }

    private void OnDisable()
    {
        EventManager.OnCurrencyChanged -= UpdateCurrencyUI;
        EventManager.OnMaxClickChanged -= UpdateMaxClickUI;
    }

    private void Start()
    {
        EventManager.RequestCurrencyValue(CurrencyType.Click);
        EventManager.RequestCurrencyValue(CurrencyType.Tree);
        EventManager.RequestCurrencyValue(CurrencyType.Rock);
        EventManager.RequestCurrencyValue(CurrencyType.Gold);
    }

    private void UpdateCurrencyUI(CurrencyType type, int currentAmount)
    {
        switch (type)
        {
            case CurrencyType.Click:
                if (clickText != null)
                    clickText.text = $"{currentAmount.ToString("N0")}/{currentMaxClick}";
                break;

            case CurrencyType.Tree:
                if (treeText != null)
                    treeText.text = currentAmount.ToString("N0");
                break;

            case CurrencyType.Rock:
                if (rockText != null)
                    rockText.text = currentAmount.ToString("N0");
                break;

            case CurrencyType.Gold:
                if (goldText != null)
                    goldText.text = currentAmount.ToString("N0");
                break;
        }
    }

    private void UpdateMaxClickUI(int newMax)
    {
        currentMaxClick = newMax;
    }
}