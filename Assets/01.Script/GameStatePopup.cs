using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStatePopup : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Dim Background")]
    [SerializeField] private GameObject dimObject;


    void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(MainMenu);
        }
    }

    private void ClosePopupAndDim()
    {
        gameObject.SetActive(false);

        if (dimObject != null)
        {
            dimObject.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    // 다시하기 버튼에 연결할 함수
    public void RestartGame()
    {
        ResetGameKeepSettings();

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // 메인 메뉴 버튼에 연결할 함수
    public void MainMenu()
    {
        ResetGameKeepSettings();

        SceneManager.LoadScene("MainScenes");
    }

    private void ResetGameKeepSettings()
    {
        ClosePopupAndDim();

        SaveData oldData = SaveLoadManager.LoadGame();

        SaveLoadManager.DeleteSaveData();

        if (oldData != null)
        {
            SaveData newData = new SaveData();
            newData.sfxVolume = oldData.sfxVolume;
            newData.bgmVolume = oldData.bgmVolume;
            newData.screenWidth = oldData.screenWidth;
            newData.screenHeight = oldData.screenHeight;
            newData.isFullScreen = oldData.isFullScreen;

            SaveLoadManager.SaveRawData(newData);
        }
    }
}