using System;
using UnityEngine;

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

    private int lastRecordedHour = -1;

    private void OnEnable()
    {
        EventManager.OnTimeDetailedChanged += HandleTimeChanged;
    }

    private void OnDisable()
    {
        EventManager.OnTimeDetailedChanged -= HandleTimeChanged;
    }

    private void Start()
    {
        ChangeWeather(WeatherType.Sunny);
    }
    private void HandleTimeChanged(int day, float hour, float minute, float second)
    {
        int currentHour = Mathf.FloorToInt(hour);

        if (currentHour != lastRecordedHour)
        {
            lastRecordedHour = currentHour;

            if (currentHour % 6 == 0)
            {
                PickRandomWeather();
            }
        }
    }

    private void PickRandomWeather()
    {
        WeatherType[] weathers = { WeatherType.Sunny, WeatherType.Rain, WeatherType.Storm };
        WeatherType randomWeather = weathers[UnityEngine.Random.Range(0, weathers.Length)];

        ChangeWeather(randomWeather);   
    }

    public void ChangeWeather(WeatherType newWeather)
    {
        CurrentWeather = newWeather;

        EventManager.TriggerWeatherChanged(newWeather);

        Debug.Log($"[WeatherManager] ³¯¾¾ º¯°æµÊ: {newWeather}");
    }
}