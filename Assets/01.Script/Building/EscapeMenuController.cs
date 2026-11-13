using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscapeMenuController : MonoBehaviour
{
    [Header("ÆË¾÷°ú ¸ðµå ÂüÁ¶")]
    [SerializeField] private GameObject pausePopup;
    [SerializeField] private GameObject soundPopup;
    [SerializeField] private BuildingPlacer buildingPlacer;
    [SerializeField] private BuildingDel buildingDel;

    [Header("ÆË¾÷ ¹öÆ°")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button soundButton;

    [Header("¾À ¼³Á¤")]
    [SerializeField] private string mainMenuSceneName = "MainScenes";

    private GameMode currentMode = GameMode.Normal;

    private void Awake()
    {
        if (pausePopup != null)
        {
            pausePopup.SetActive(false);
        }
        if (soundPopup != null)
        {
            soundPopup.SetActive(false);
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

        if (soundButton != null)
        {
            soundButton.onClick.AddListener(OpenSoundPopup);
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

        if (soundButton != null)
        {
            soundButton.onClick.RemoveListener(OpenSoundPopup);
        }

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


    public void OpenSoundPopup()
    {
        if (soundPopup != null)
        {
            soundPopup.SetActive(true);
        }
    }

    public void CloseSoundPopup()
    {
        if (soundPopup != null)
        {
            soundPopup.SetActive(false);
        }
    }

    private void OpenPausePopup()
    {
        if (pausePopup == null)
        {
            return;
        }

        pausePopup.SetActive(true);
        EventManager.ChangeGameMode(GameMode.Paused);
    }

    public void ResumeGame()
    {
        if (pausePopup != null)
        {
            pausePopup.SetActive(false);
        }

        EventManager.ChangeGameMode(GameMode.Normal);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}