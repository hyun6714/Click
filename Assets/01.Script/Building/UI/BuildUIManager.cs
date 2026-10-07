using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildUIManager : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    public Button demolishButton;

    public GameObject buildDelPopup; //철거 팝업
    public GameObject buildAddPopup; //설치 팝업

    private void Awake()
    {
        if (buildDelPopup != null)
        {
            buildDelPopup.SetActive(false);
        }

        if (buildAddPopup != null)
        {
            buildAddPopup.SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventManager.OnBuildMenuPopupToggle += ToggleBuildDelPopup;
        EventManager.OnBuilAddPopupToggle += ToggleBuildAddPopup;
    }

    private void OnDisable()
    {
        EventManager.OnBuildMenuPopupToggle -= ToggleBuildDelPopup;
        EventManager.OnBuilAddPopupToggle -= ToggleBuildAddPopup;
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
            Debug.LogWarning("철거 버튼 null");
        }
    }

    // 철거 버튼을 눌렀을 때 실행되는 함수
    void OnDemolishButtonClicked()
    {
        EventManager.TriggerDemolishButtonClicked();

        EventManager.TriggerBuildMenuPopupToggle();
    }

    private void ToggleBuildDelPopup()
    {
        if (buildDelPopup != null)
        {
            bool isActive = buildDelPopup.activeSelf;
            buildDelPopup.SetActive(!isActive); 
        }
    }

    private void ToggleBuildAddPopup(bool isOpen)
    {
        if (buildAddPopup != null)
        {
            buildAddPopup.SetActive(isOpen);
        }
    }
}
