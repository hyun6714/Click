using UnityEngine;

public class WeatherEffectController : MonoBehaviour
{
    [Header("³¯¾¾º° ÀÌÆåÆ®")]
    [SerializeField] private GameObject rainParticleObject;
    [SerializeField] private GameObject snowParticleObject;

    private void OnEnable()
    {
        EventManager.OnWeatherChanged += HandleWeatherChanged;
    }

    private void OnDisable()
    {
        EventManager.OnWeatherChanged -= HandleWeatherChanged;
    }

    private void HandleWeatherChanged(WeatherType newWeather)
    {
        bool isRaining = (newWeather == WeatherType.Rain || newWeather == WeatherType.Storm);
        bool isSnowing = (newWeather == WeatherType.Snow);

        if (rainParticleObject != null)
        {
            rainParticleObject.SetActive(isRaining);
        }

        if (snowParticleObject != null)
        {
            snowParticleObject.SetActive(isSnowing);
        }
    }
}