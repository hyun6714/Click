using UnityEngine;
using UnityEngine.UI;

public class LicensePopupController : MonoBehaviour
{
    [Header("라이선스 텍스트/이미지 팝업 패널")]
    [SerializeField] private GameObject licensePopupPanel;

    [Header("라이선스 페이지 설정 (1~5페이지 오브젝트 배열 순서대로 드래그)")]
    [SerializeField] private GameObject[] licensePages; 

    [Header("메인 화면에 있는 [?] 버튼")]
    [SerializeField] private Button questionMarkButton;

    [Header("이전 / 다음 / 닫기 버튼")]
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;

    private int currentPageIndex = 0;

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
        if (questionMarkButton != null)
        {
            questionMarkButton.onClick.AddListener(ToggleLicensePopup);
        }
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseLicensePopup);
        }
    }

    private void Start()
    {
        if (licensePopupPanel != null)
        {
            licensePopupPanel.SetActive(false);
        }
    }

    public void ToggleLicensePopup()
    {
        if (licensePopupPanel != null)
        {
            bool isActive = licensePopupPanel.activeSelf;

            if (!isActive)
            {
                licensePopupPanel.SetActive(true);
                ShowPage(0);
            }
            else
            {
                licensePopupPanel.SetActive(false);
            }
        }
    }

    // 팝업 닫기
    public void CloseLicensePopup()
    {
        if (licensePopupPanel != null)
        {
            licensePopupPanel.SetActive(false);
        }
    }

    private void ShowPage(int index)
    {
        currentPageIndex = index;

        for (int i = 0; i < licensePages.Length; i++)
        {
            if (licensePages[i] != null)
            {
                licensePages[i].SetActive(i == currentPageIndex);
            }
        }

        if (prevButton != null)
        {
            prevButton.gameObject.SetActive(currentPageIndex > 0);
        }

        bool isLastPage = (currentPageIndex == licensePages.Length - 1);

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(!isLastPage);
        }

        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(isLastPage);
        }
    }

    // 다음 페이지 버튼
    public void OnClickNextPage()
    {
        if (currentPageIndex < licensePages.Length - 1)
        {
            ShowPage(currentPageIndex + 1);
        }
    }

    public void OnClickPrevPage()
    {
        if (currentPageIndex > 0)
        {
            ShowPage(currentPageIndex - 1);
        }
    }
}