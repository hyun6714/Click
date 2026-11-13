using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenSettings : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Dropdown resolutionDropdown; // 해상도 선택 드롭다운
    [SerializeField] private Toggle fullScreenToggle;         // 전체 화면 체크박스

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private int currentResolutionIndex = 0;

    public static ScreenSettings instance;

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
        }

        LoadAndApplyAllSettings();
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
        LoadAndApplyAllSettings();
        ReconnectUI();
    }

    private void Start()
    {
        ReconnectUI();
    }


    private void ReconnectUI()
    {
        if (resolutionDropdown == null)
        {
            resolutionDropdown = FindFirstObjectByType<TMP_Dropdown>(FindObjectsInactive.Include);
        }

        if (fullScreenToggle == null)
        {
            fullScreenToggle = FindFirstObjectByType<Toggle>(FindObjectsInactive.Include);
        }

        if (resolutionDropdown != null)
        {
            resolutions = Screen.resolutions;
            filteredResolutions = new List<Resolution>();

            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();

            // 저장된 데이터 불러오기
            SaveData data = SaveLoadManager.LoadGame();
            int targetWidth = (data != null && data.screenWidth > 0) ? data.screenWidth : Screen.width;
            int targetHeight = (data != null && data.screenHeight > 0) ? data.screenHeight : Screen.height;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                if (!options.Contains(option))
                {
                    options.Add(option);
                    filteredResolutions.Add(resolutions[i]);

                    // ⭐ 저장되어 있던 해상도와 일치하는 항목을 찾아 드롭다운 인덱스로 지정!
                    if (resolutions[i].width == targetWidth && resolutions[i].height == targetHeight)
                    {
                        currentResolutionIndex = filteredResolutions.Count - 1;
                    }
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);
            resolutionDropdown.RefreshShownValue();

            resolutionDropdown.onValueChanged.RemoveListener(SetResolutionByIndex);
            resolutionDropdown.onValueChanged.AddListener(SetResolutionByIndex);
        }

        if (fullScreenToggle != null)
        {
            SaveData data = SaveLoadManager.LoadGame();
            bool isFull = (data != null) ? data.isFullScreen : Screen.fullScreen;

            fullScreenToggle.SetIsOnWithoutNotify(isFull);

            fullScreenToggle.onValueChanged.RemoveListener(SetFullScreen);
            fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
        }
    }


    // 전체 화면 / 창 모드 변경 함수 
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        SaveData data = SaveLoadManager.LoadGame() ?? new SaveData();
        data.isFullScreen = isFullScreen;
        SaveLoadManager.SaveRawData(data);

    }

    // 해상도 변경 함수
    public void SetResolutionByIndex(int resolutionIndex)
    {
        if (filteredResolutions == null || resolutionIndex < 0 || resolutionIndex >= filteredResolutions.Count)
        {
            return;
        }

        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetResolution(int width, int height, bool isFullScreen)
    {
        Screen.SetResolution(width, height, isFullScreen);

        SaveData data = SaveLoadManager.LoadGame() ?? new SaveData();
        data.screenWidth = width;
        data.screenHeight = height;
        data.isFullScreen = isFullScreen;

        SaveLoadManager.SaveRawData(data);
    }

    private void LoadAndApplyAllSettings()
    {
        SaveData data = SaveLoadManager.LoadGame();

        if (data != null)
        {
            int width = data.screenWidth > 0 ? data.screenWidth : 1920;
            int height = data.screenHeight > 0 ? data.screenHeight : 1080;
            bool isFullScreen = data.isFullScreen;

            Screen.SetResolution(width, height, isFullScreen);
        }
        else
        {
            Screen.SetResolution(1920, 1080, true);
        }
    }
}
