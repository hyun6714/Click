using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class GameTimeManager : MonoBehaviour
{
    [Header("시간 설정")]
    [SerializeField] private int currentDay = 1; // 일 
    [SerializeField] private float currentHour = 7f; // 시
    [SerializeField] private float currentMinute = 0f; // 분
    [SerializeField] private float currentSecond = 0f; // 초

    public int CurrentDay => currentDay;
    public float CurrentHour => currentHour;
    public float CurrentMinute => currentMinute;
    public float CurrentSecond => currentSecond;

    [Header("속도 조절")]
    [SerializeField] private float realSecondsPerSecond = 1f;
    private float timer = 0f;

    [Header("시간 스킵 설정")]
    [SerializeField] private float targetMorningHour = 8f;

    [Header("스킵 연출 설정")]
    [SerializeField] private float skipAnimationDuration = 0.5f;
    private bool isSkipping = false;

    private bool isInitialized = false;

    private BuildingManager buildingMgr;
    private CurrencyManager currencyMgr;
    private WeatherManager weatherMgr;
    private BuildingUnlockManager unlockMgr;

    private void OnEnable()
    {
        EventManager.OnSkipButtonClicked += SkipToNextDay;
    }

    private void OnDisable()
    {
        EventManager.OnSkipButtonClicked -= SkipToNextDay;
    }
    private void Awake()
    {
        buildingMgr = Object.FindFirstObjectByType<BuildingManager>();
        currencyMgr = Object.FindFirstObjectByType<CurrencyManager>();
        weatherMgr = Object.FindFirstObjectByType<WeatherManager>();
        unlockMgr = Object.FindFirstObjectByType<BuildingUnlockManager>();
    }

    private void Start()
    {
        EventManager.TriggerTimeDetailedChanged(currentDay, currentHour, currentMinute, currentSecond);

        SaveData saveData = SaveLoadManager.LoadGame();

        if (saveData != null)
        {
            LoadTimeData(saveData.day, saveData.hour, saveData.minute, saveData.second);
        }
        else
        {
            EventManager.TriggerTimeDetailedChanged(currentDay, currentHour, currentMinute, currentSecond);
            EventManager.TriggerDayChanged(currentDay);
        }

        isInitialized = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= realSecondsPerSecond)
        {
            timer -= 1f; // 남은 잔여 시간 보존
            AdvanceSecond();
        }
    }

    private void AdvanceSecond()
    {
        currentSecond += 1f;

        if (currentSecond >= 60f)
        {
            currentSecond = 0f;
            currentMinute += 1f;

            if (currentMinute >= 60f)
            {
                currentMinute = 0f;
                currentHour += 1f;

                if (currentHour >= 24f)
                {
                    currentHour = 0f;
                    AdvanceDay();
                }
            }
        }

        EventManager.TriggerTimeDetailedChanged(currentDay, currentHour, currentMinute, currentSecond);
    }

    private void AdvanceDay()
    {
        currentDay++;
        Debug.Log($"[날짜 변경] 하루 지남 현재 {currentDay}일차");

        EventManager.CurrencyAdded(CurrencyType.Click, 10);
        EventManager.TriggerDayChanged(currentDay);

        TriggerSave();
    }

    public void SkipToNextDay()
    {
        if (isSkipping)
        {
            return;
        }

        StartCoroutine(SkipTimeRoutine());
    }

    private IEnumerator SkipTimeRoutine()
    {
        isSkipping = true;

        int targetDay = currentDay + 1;
        float startHour = currentHour;
        float startMinute = currentMinute;

        float currentTotalMinutes = (currentHour * 60f) + currentMinute;
        float targetTotalMinutes = (targetMorningHour * 60f);

        if (currentTotalMinutes >= targetTotalMinutes)
        {
            targetTotalMinutes += 24f * 60f; 
        }

        float elapsed = 0f;
        timer = 0f;

        while (elapsed < skipAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / skipAnimationDuration);

            float lerpMinutes = Mathf.Lerp(currentTotalMinutes, targetTotalMinutes, t);

            currentHour = Mathf.Floor(lerpMinutes / 60f) % 24f;
            currentMinute = Mathf.Floor(lerpMinutes % 60f);
            currentSecond = 0f; 

            EventManager.TriggerTimeDetailedChanged(currentDay, currentHour, currentMinute, currentSecond);

            yield return null;
        }


        currentDay = targetDay;
        currentHour = targetMorningHour;
        currentMinute = 0f;
        currentSecond = 0f;
        timer = 0f;

        Debug.Log($"[날짜 스킵] 현재 {currentDay}일차, 오전 {currentHour}시");

        EventManager.CurrencyAdded(CurrencyType.Click, 10);
        EventManager.TriggerDayChanged(currentDay);
        EventManager.TriggerTimeDetailedChanged(currentDay, currentHour, currentMinute, currentSecond);

        TriggerSave();

        isSkipping = false;
    }

    public void TriggerSave()
    {
        if (!isInitialized)
        {
            return;
        }

        Debug.Log("[디버그] TriggerSave 함수 진입 성공");
        GameTimeManager timeMgr = this;

        SaveLoadManager.SaveGame(timeMgr, currencyMgr, buildingMgr, weatherMgr, unlockMgr);
    }

    public void LoadTimeData(int loadedDay, float loadedHour, float loadedMinute, float loadedSecond)
    {
        currentDay = loadedDay;
        currentHour = loadedHour;
        currentMinute = loadedMinute;
        currentSecond = loadedSecond;
        timer = 0f;
        EventManager.TriggerTimeDetailedChanged(currentDay, currentHour, currentMinute, currentSecond);
        EventManager.TriggerDayChanged(currentDay);

        Debug.Log($"[GameTimeManager] 데이터 로드 완료: {currentDay}일차, {currentHour}시 {currentMinute}분");
    }
}