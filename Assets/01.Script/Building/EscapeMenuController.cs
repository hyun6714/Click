using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscapeMenuController : MonoBehaviour
{
    [Header("팝업과 모드 참조")]
    [SerializeField] private GameObject pausePopup;
    [SerializeField] private BuildingPlacer buildingPlacer;
    [SerializeField] private BuildingDel buildingDel;

    [Header("팝업 버튼")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("씬 설정")]
    [SerializeField] private string mainMenuSceneName = "MainScenes";

    private GameMode currentMode = GameMode.Normal;

    private void Awake()
    {
        if (pausePopup != null)
        {
            pausePopup.SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventManager.OnGameModeChanged += HandleGameModeChanged;

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ResumeGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    private void OnDisable()
    {
        EventManager.OnGameModeChanged -= HandleGameModeChanged;

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(ResumeGame);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitGame);
        }

        Time.timeScale = 1f;
    }

    private void HandleGameModeChanged(GameMode newMode)
    {
        currentMode = newMode;
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        if (currentMode == GameMode.Build)
        {
            buildingPlacer?.CancelPlacing();
            return;
        }

        if (currentMode == GameMode.Del)
        {
            buildingDel?.ToggleDemolishMode();
            return;
        }

        if (currentMode == GameMode.Paused)
        {
            ResumeGame();
            return;
        }

        OpenPausePopup();
    }

    private void OpenPausePopup()
    {
        if (pausePopup == null)
        {
            Debug.LogError("Pause Popup이 연결되지 않음");
            return;
        }

        pausePopup.SetActive(true);
        EventManager.ChangeGameMode(GameMode.Paused);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (pausePopup != null)
        {
            pausePopup.SetActive(false);
        }

        Time.timeScale = 1f;
        EventManager.ChangeGameMode(GameMode.Normal);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}