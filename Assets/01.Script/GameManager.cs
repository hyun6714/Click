using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI Popups")]
    [SerializeField] private GameObject gameClearPopup;
    [SerializeField] private GameObject gameOverPopup;
    [SerializeField] private GameObject dimObject;

    private void OnEnable()
    {
        EventManager.OnCastleDestroyed += HandleGameOver;
        EventManager.OnGameCleared += HandleGameClear;
    }

    private void OnDisable()
    {
        EventManager.OnCastleDestroyed -= HandleGameOver;
        EventManager.OnGameCleared -= HandleGameClear;
    }

    private void HandleGameOver()
    {
        if (dimObject != null)
        {
            dimObject.SetActive(true);
        }


        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(true);
        }

        Time.timeScale = 0;
    }

    private void HandleGameClear()
    {
        if (dimObject != null)
        {
            dimObject.SetActive(true);
        }

        if (gameClearPopup != null)
        {
            gameClearPopup.SetActive(true);
        }

        Time.timeScale = 0f; 
    }
}
