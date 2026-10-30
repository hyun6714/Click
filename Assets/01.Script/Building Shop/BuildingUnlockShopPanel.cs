using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildingUnlockShopPanel : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private GameObject shopPanelRoot; // 실제 켜지고 꺼질 상점 창 오브젝트
    [SerializeField] private Button openShopButton;
    [SerializeField] private Button closeShopButton;

    private List<BuildingUnlockSlotUI> slotUIs = new List<BuildingUnlockSlotUI>();

    private void Awake()
    {
        slotUIs.AddRange(GetComponentsInChildren<BuildingUnlockSlotUI>(true));

        if (openShopButton != null)
        {
            openShopButton.onClick.AddListener(() => {
                bool isOpen = shopPanelRoot != null && !shopPanelRoot.activeSelf;
                EventManager.TriggerUnlockShopPopupToggle(isOpen);
            });
        }

        if (closeShopButton != null)
        {
            closeShopButton.onClick.AddListener(() => {
                EventManager.TriggerUnlockShopPopupToggle(false);
            });
        }

        if (shopPanelRoot != null)
        {
            shopPanelRoot.SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventManager.OnBuildingUnlockUpdated += RefreshAllSlots;
        EventManager.OnUnlockShopPopupToggle += HandleShopPopupToggle;
    }

    private void OnDisable()
    {
        EventManager.OnBuildingUnlockUpdated -= RefreshAllSlots;
        EventManager.OnUnlockShopPopupToggle -= HandleShopPopupToggle;
    }

    private void HandleShopPopupToggle(bool isOpen)
    {
        if (shopPanelRoot != null)
        {
            shopPanelRoot.SetActive(isOpen);
        }

        if (isOpen)
        {
            RefreshAllSlots();
        }
    }

    private void RefreshAllSlots()
    {
        foreach (var slot in slotUIs)
        {
            if (slot != null)
            {
                slot.UpdateUIState();
            }
        }
    }
}