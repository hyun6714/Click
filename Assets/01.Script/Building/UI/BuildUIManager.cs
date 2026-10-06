using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildUIManager : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    public Button demolishButton;

    public GameObject buildDelPopup; //철거 팝업

    private void Awake()
    {
        if (buildDelPopup != null)
        {
            buildDelPopup.SetActive(false);
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

    private void TogglePopup()
    {
        if (buildDelPopup != null)
        {
            bool isActive = buildDelPopup.activeSelf;
            buildDelPopup.SetActive(!isActive); 
        }
    }
}
