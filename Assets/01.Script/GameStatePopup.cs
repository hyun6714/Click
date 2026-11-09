using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStatePopup : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Dim")]
    [SerializeField] private GameObject dimObject;

    [Header("데이터베이스 참조")]
    [SerializeField] private CurrencyDatabase currencyDatabase;


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

        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.ForceResetWaveState();
        }

        SaveData oldData = SaveLoadManager.LoadGame();

        SaveLoadManager.DeleteSaveData();

        SaveData newData = new SaveData();

        if (currencyDatabase != null)
        {
            newData.InitWithDatabase(currencyDatabase);
        }

        if (oldData != null)
        {
            newData.sfxVolume = oldData.sfxVolume;
            newData.bgmVolume = oldData.bgmVolume;
            newData.screenWidth = oldData.screenWidth;
            newData.screenHeight = oldData.screenHeight;
            newData.isFullScreen = oldData.isFullScreen;

            SaveLoadManager.SaveRawData(newData);
        }
        else
        {
            if (currencyDatabase != null)
            {
                newData.InitWithDatabase(currencyDatabase);
            }
            SaveLoadManager.SaveRawData(newData);
        }
    }
}