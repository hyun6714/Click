using UnityEngine;
using UnityEngine.UI;

public class BuildUIManager : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    public Button demolishButton;

    public GameObject buildDelPopup; //철거 팝업

    private void Awake()
    {
        buildDelPopup.SetActive(false);
    }

    void Start()
    {
        if (demolishButton == null)
        {
            demolishButton = GetComponentInChildren<Button>();
        }

        if (demolishButton != null)
        {
            demolishButton.onClick.RemoveAllListeners();
            demolishButton.onClick.AddListener(OnDemolishButtonClicked);
        }
        else
        {
            Debug.LogWarning("BuildUIManager: 철거 버튼을 찾지 못했습니다!");
        }
    }

    private void OnEnable()
    {
        EventManager.OnBuildMenuPopupToggle += TogglePopup;
    }

    private void OnDisable()
    {
        EventManager.OnBuildMenuPopupToggle -= TogglePopup;
    }


    // 철거 버튼을 눌렀을 때 실행되는 함수
    void OnDemolishButtonClicked()
    {
        EventManager.TriggerDemolishButtonClicked();

        EventManager.TriggerBuildMenuPopupToggle();
    }

    private void TogglePopup()
    {
        if (buildDelPopup != null)
        {
            bool isActive = buildDelPopup.activeSelf;
            buildDelPopup.SetActive(!isActive); 
        }
    }
}
