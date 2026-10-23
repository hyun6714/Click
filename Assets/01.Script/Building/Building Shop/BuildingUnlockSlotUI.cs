using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingUnlockSlotUI : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private BuildingUnlockData nodeData;

    [Header("UI 컴포넌트")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text costText;
    public Button unlockButton;
    public GameObject lockedOverlay; // 잠김 상태를 가릴 어두운 이미지나 자물쇠 아이콘

    private BuildingUnlockData assignedNode;
    private BuildingUnlockManager unlockManager;

    private void Awake()
    {
        if (nodeData != null)
        {
            assignedNode = nodeData;
        }

        unlockManager = FindFirstObjectByType<BuildingUnlockManager>();
    }

    private void Start()
    {
        if (unlockButton != null)
        {
            unlockButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(OnClickUnlockButton);
        }

        UpdateUIState();
    }

    private void OnEnable()
    {
        EventManager.OnBuildingUnlockUpdated += UpdateUIState;
    }

    private void OnDisable()
    {
        EventManager.OnBuildingUnlockUpdated -= UpdateUIState;
    }

    public void UpdateUIState()
    {
        if (assignedNode == null)
        {
            return;
        }

        // 아이콘 셋팅
        if (iconImage != null && assignedNode.icon != null)
        {
            iconImage.sprite = assignedNode.icon;
        }

        // 이름 셋팅
        if (nameText != null)
        {
            nameText.text = assignedNode.nodeName;
        }

        // 해금 상태에 따른 UI 변경
        if (assignedNode.isUnlocked)
        {
            if (unlockButton != null)
            {
                unlockButton.interactable = false;
            }

            if (lockedOverlay != null)
            {
                lockedOverlay.SetActive(false);
            }

            if (costText != null)
            {
                costText.text = "해금됨";
            }
        }
        else if (assignedNode.isAvailable)
        {
            if (unlockButton != null)
            {
                unlockButton.interactable = true;
            }

            if (lockedOverlay != null)
            {
                lockedOverlay.SetActive(false);
            }

            if (costText != null)
            {
                costText.text = $"{assignedNode.unlockCost} G";
            }
        }
        else
        {
            if (unlockButton != null)
            {
                unlockButton.interactable = false;
            }

            if (lockedOverlay != null)
            {
                lockedOverlay.SetActive(true);
            }

            if (costText != null)
            {
                costText.text = $"{assignedNode.unlockCost} G";
            }
        }
    }

    private void OnClickUnlockButton()
    {
        if (unlockManager != null)
        {
            unlockManager.TryUnlockNode(assignedNode);
            UpdateUIState(); // UI 갱신
        }
    }
}