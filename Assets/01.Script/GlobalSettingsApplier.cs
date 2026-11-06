using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GlobalSettingsApplier : MonoBehaviour
{
    [Header("오디오 믹서 연결")]
    [SerializeField] private AudioMixer audioMixer;

    private const string SfxParameter = "SFXVolume";
    private const string BgmParameter = "BGMVolume";

    public static GlobalSettingsApplier instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ApplyAllSettings();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyAllSettings();
    }

    public void ApplyAllSettings()
    {
        SaveData data = SaveLoadManager.LoadGame();

        if (data != null)
        {
            int width = data.screenWidth > 0 ? data.screenWidth : 1920;
            int height = data.screenHeight > 0 ? data.screenHeight : 1080;
            bool isFullScreen = data.isFullScreen;

            Screen.SetResolution(width, height, isFullScreen);

            ApplyVolume(SfxParameter, data.sfxVolume);
            ApplyVolume(BgmParameter, data.bgmVolume);

            Debug.Log($"[GlobalSettings] 씬 전환 감지 - 설정 적용 완료 (해상도: {width}x{height}, BGM: {data.bgmVolume}, SFX: {data.sfxVolume})");
        }
        else
        { 
            Screen.SetResolution(1920, 1080, true);
        }
    }

    private void ApplyVolume(string parameterName, float value)
    {
        if (audioMixer == null)
        {
            return;
        }

        float decibels = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(parameterName, decibels);
    }
}