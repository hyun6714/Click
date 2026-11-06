using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettingsPopup : MonoBehaviour
{
    [Header("오디오 믹서")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("슬라이더")]
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;


    [SerializeField] private GameObject targetPopupUI;

    [Header("닫기 버튼")]
    [SerializeField] private Button closeButton;

    private const string SfxParameter = "SFXVolume";
    private const string BgmParameter = "BGMVolume";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeVolumeOnLoad()
    {
        SaveData data = SaveLoadManager.LoadGame();
        if (data != null)
        {

        }
    }

    private void Awake()
    {
        LoadAndApplySettings();
    }

    private void Start()
    {
        LoadAndApplySettings();
    }


    private void OnEnable()
    {
        LoadAndApplySettings();

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        }

        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.onValueChanged.AddListener(SetBgmVolume);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    private void OnDisable()
    {
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveListener(SetSfxVolume);
        }

        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.onValueChanged.RemoveListener(SetBgmVolume);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Close);
        }
    }

    private void LoadAndApplySettings()
    {
        SaveData data = SaveLoadManager.LoadGame();

        float sfxVolume = (data != null) ? data.sfxVolume : 1f;
        float bgmVolume = (data != null) ? data.bgmVolume : 1f;

        ApplyVolume(SfxParameter, sfxVolume);
        ApplyVolume(BgmParameter, bgmVolume);

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.SetValueWithoutNotify(sfxVolume);
        }

        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.minValue = 0f;
            bgmVolumeSlider.maxValue = 1f;
            bgmVolumeSlider.SetValueWithoutNotify(bgmVolume);
        }
    }

    private void SetSfxVolume(float value)
    {
        ApplyVolume(SfxParameter, value);
        SaveVolumeData();
    }

    private void SetBgmVolume(float value)
    {
        ApplyVolume(BgmParameter, value);
        SaveVolumeData();
    }

    private void SaveVolumeData()
    {
        SaveData data = SaveLoadManager.LoadGame() ?? new SaveData();

        if (sfxVolumeSlider != null)
        {
            data.sfxVolume = sfxVolumeSlider.value;
        }

        if (bgmVolumeSlider != null)
        {
            data.bgmVolume = bgmVolumeSlider.value;
        }

        SaveLoadManager.SaveRawData(data);
        Debug.Log($"[SoundSettings] 볼륨 JSON 저장 완료 - SFX: {data.sfxVolume}, BGM: {data.bgmVolume}");
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

    private void Close()
    {
        if (targetPopupUI != null)
        {
            targetPopupUI.SetActive(false);
        }
    }
}