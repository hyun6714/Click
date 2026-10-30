using UnityEngine;
using UnityEngine.UI; // UI 컴포넌트를 사용하기 위해 필요합니다.
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
        ClosePopupAndDim();

        SaveLoadManager.DeleteSaveData();

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // 메인 메뉴 버튼에 연결할 함수
    public void MainMenu()
    {
        ClosePopupAndDim();

        SaveLoadManager.DeleteSaveData();

        SceneManager.LoadScene("MainScenes");
    }
}