using UnityEngine;
using UnityEngine.UI;

public class TutorialPopupController : MonoBehaviour
{
    [Header("UI 팝업 패널")]
    [SerializeField] private GameObject tutorialPopupPanel;

    [Header("페이지 설정")]
    [SerializeField] private GameObject[] tutorialPages;

    [Header("이전 / 다음 / 시작하기 버튼")]
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button startButton;

    [Header("튜토 다시 보는 버튼")]
    [SerializeField] private Button manualTutorialButton;

    private int currentPageIndex = 0;
    private bool isManualOpen = false;

    private void Awake()
    {
        if (prevButton != null)
        {
            prevButton.onClick.AddListener(OnClickPrevPage);
        }
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnClickNextPage);
        }
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnClickStartGame);
        }
        if (manualTutorialButton != null)
        {
            manualTutorialButton.onClick.AddListener(ShowTutorialManually);
        }
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("HasSeenTutorial", 0) == 1)
        {
            if (tutorialPopupPanel != null)
            {
                tutorialPopupPanel.SetActive(false);
            }

            Time.timeScale = 1f;
            return;
        }

        if (tutorialPopupPanel != null)
        {
            tutorialPopupPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        isManualOpen = false;
        OpenPopup();
    }

    private void OpenPopup()
    {
        if (tutorialPopupPanel != null)
        {
            tutorialPopupPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        ShowPage(0);
    }

    private void ShowPage(int index)
    {
        currentPageIndex = index;

        for (int i = 0; i < tutorialPages.Length; i++)
        {
            if (tutorialPages[i] != null)
            {
                tutorialPages[i].SetActive(i == currentPageIndex);
            }
        }

        if (prevButton != null)
        {
            prevButton.gameObject.SetActive(currentPageIndex > 0);
        }

        bool isLastPage = (currentPageIndex == tutorialPages.Length - 1);

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(!isLastPage);
        }

        if (startButton != null)
        {
            startButton.gameObject.SetActive(isLastPage);
        }
    }

    //다음 버튼
    public void OnClickNextPage()
    {
        if (currentPageIndex < tutorialPages.Length - 1)
        {
            ShowPage(currentPageIndex + 1);
        }
    }

    
    //이전 버튼
    public void OnClickPrevPage()
    {
        if (currentPageIndex > 0)
        {
            ShowPage(currentPageIndex - 1);
        }
    }

    //Start 버튼
    public void OnClickStartGame()
    {
        if (tutorialPopupPanel != null)
        {
            tutorialPopupPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        if (!isManualOpen)
        {
            PlayerPrefs.SetInt("HasSeenTutorial", 1);
            PlayerPrefs.Save();
        }
    }

    public void ShowTutorialManually()
    {
        isManualOpen = true; 
        OpenPopup();
    }
}