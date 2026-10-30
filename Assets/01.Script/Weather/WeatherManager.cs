using System;
using UnityEngine;
using Rnd = UnityEngine.Random;

public enum WeatherType
{
    Sunny, // ¸¼À½
    Rain, // ºñ
    Storm, // ÆøÇ³¿ì
    Snow // ´«
}

public class WeatherManager : MonoBehaviour
{
    public WeatherType CurrentWeather { get; private set; } = WeatherType.Sunny;

    private void OnEnable()
    {
        EventManager.OnDayChanged += HandleDayChanged;
    }

    private void OnDisable()
    {
        EventManager.OnDayChanged -= HandleDayChanged;
    }

    private void Start()
    {
        SaveData saveData = SaveLoadManager.LoadGame();

        if (saveData != null)
        {
            LoadWeatherData(saveData);
        }
        else
        {
            DetermineAndTriggerWeather(1);
        }
    }

    private void HandleDayChanged(int day)
    {
        DetermineAndTriggerWeather(day);
    }

    private void DetermineAndTriggerWeather(int day)
    {
        WeatherType selectedWeather;

        float roll = Rnd.value;

        if (day >= 1 && day <= 10)
        {
            // 1~10ÀÏ: ¸¼À½(Sunny) ¶Ç´Â ºñ(Rain)
            selectedWeather = roll < 0.5f ? WeatherType.Sunny : WeatherType.Rain;
        }
        else if (day >= 11 && day <= 20)
        {
            // 11~20ÀÏ: ¸¼À½(Sunny), ºñ(Rain), ÆøÇ³¿ì(Storm) ¼¯ÀÓ
            if (roll < 0.4f)
            {
                selectedWeather = WeatherType.Sunny;
            }
            else if (roll < 0.8f)
            {
                selectedWeather = WeatherType.Rain;
            }
            else
            {
                selectedWeather = WeatherType.Storm;
            }
        }
        else if (day >= 21 && day <= 30)
        {
            // 21~30ÀÏ: ¹«Á¶°Ç ´«(Snow) °­Á¦! (È­·Î Å¸ÀÌ¹Ö)
            selectedWeather = WeatherType.Snow;
        }
        else
        {
            // 30ÀÏ ÀÌÈÄ ±× ¿Ü ±¸°£
            selectedWeather = WeatherType.Sunny;
        }

        ChangeWeather(selectedWeather);
        Debug.Log($"[WeatherManager] {day}ÀÏÂ÷ ³¯¾¾ °áÁ¤: {selectedWeather}");
    }

    public void ChangeWeather(WeatherType newWeather)
    {
        CurrentWeather = newWeather;

        EventManager.TriggerWeatherChanged(newWeather);

        Debug.Log($"[WeatherManager] ³¯¾¾ º¯°æµÊ: {newWeather}");
    }

    public void SaveWeatherData(SaveData data)
    {
        if (data == null)
        {
            return;
        }

        data.currentWeather = CurrentWeather.ToString(); // ÇöÀç ³¯¾¾ »óÅÂ ÀúÀå
    }

    public void LoadWeatherData(SaveData data)
    {
        if (data == null || string.IsNullOrEmpty(data.currentWeather))
        {
            return;
        }

        if (Enum.TryParse(data.currentWeather, out WeatherType loadedWeather))
        {
            ChangeWeather(loadedWeather);
            Debug.Log($"[WeatherManager] ³¯¾¾ µ¥ÀÌÅÍ º¹¿ø ¿Ï·á: {loadedWeather}");
        }
    }
}