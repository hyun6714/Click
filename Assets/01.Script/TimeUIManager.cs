using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeUIManager : MonoBehaviour
{
    public TextMeshProUGUI timeText; // 일,시,분,초 텍스트

    [SerializeField] private Button skipButton;

    private void Awake()
    {
        if (skipButton == null) //정적이벤트로 연결 후 Find 삭제 
        {
            GameObject btnObj = GameObject.Find("SkipButton");
            if (btnObj != null)
            {
                skipButton = btnObj.GetComponent<Button>();
            }
        }
    }

    private void OnEnable()
    {
        EventManager.OnTimeDetailedChanged += UpdateTimeUI;

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(() => EventManager.TriggerSkipButtonClicked());
        }
    }

    private void OnDisable()
    {
        EventManager.OnTimeDetailedChanged -= UpdateTimeUI;

        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(() => EventManager.TriggerSkipButtonClicked());
        }
    }

    private void UpdateTimeUI(int day, float hour, float minute, float second)
    {
        if (timeText != null)
        {
            timeText.text = $"Day {day} - {hour:00}:{minute:00}:{second:00}";
        }
    }
}