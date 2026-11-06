using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainBtn : MonoBehaviour
{
    [Header("씬 이동 설정")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("버튼 참조")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button settingsCloseButton;


    [Header("팝업 참조")]
    [SerializeField] private GameObject settingsPopup;

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(GameStart);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(GameExit);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
        }

        if (settingsCloseButton != null)
        {
            settingsCloseButton.onClick.AddListener(CloseSettings);
        }
    }

    private void OnDestroy()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(GameStart);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(ContinueGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(GameExit);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OpenSettings);
        }

        if (settingsCloseButton != null)
        {
            settingsCloseButton.onClick.RemoveListener(CloseSettings);
        }
    }

    public void GameStart()
    {
        SaveData oldData = SaveLoadManager.LoadGame();

        SaveLoadManager.DeleteSaveData();

        SaveData newData = new SaveData();

        if (oldData != null)
        {
            newData.sfxVolume = oldData.sfxVolume;
            newData.bgmVolume = oldData.bgmVolume;
            newData.screenWidth = oldData.screenWidth;
            newData.screenHeight = oldData.screenHeight;
            newData.isFullScreen = oldData.isFullScreen;

            SaveLoadManager.SaveRawData(newData);
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        var saveData = SaveLoadManager.LoadGame();

        if (saveData != null)
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void OpenSettings()
    {
        if (settingsPopup != null)
        {
            settingsPopup.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPopup != null)
        {
            settingsPopup.SetActive(false);
        }
    }

    public void GameExit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

