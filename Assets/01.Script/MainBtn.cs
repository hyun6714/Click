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
    }

    public void GameStart()
    {
        SaveLoadManager.DeleteSaveData();
        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        var saveData = SaveLoadManager.LoadGame();

        if (saveData != null)
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            //없을 시 UI 
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

