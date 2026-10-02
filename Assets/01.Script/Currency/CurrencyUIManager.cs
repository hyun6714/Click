using UnityEngine;
using TMPro;

public class CurrencyUIManager : MonoBehaviour
{
    [Header("자원 텍스트")]
    public TextMeshProUGUI clickText;
    public TextMeshProUGUI treeText;
    public TextMeshProUGUI rockText;
    public TextMeshProUGUI goldText;

    private void OnEnable()
    {
        EventManager.OnCurrencyChanged += UpdateCurrencyUI;
    }

    private void OnDisable()
    {
        EventManager.OnCurrencyChanged -= UpdateCurrencyUI;
    }

    private void UpdateCurrencyUI(CurrencyType type, int currentAmount)
    {
        switch (type)
        {
            case CurrencyType.Click:
                if (clickText != null)
                    clickText.text = $"{currentAmount.ToString("N0")}/40";
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
}