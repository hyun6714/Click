using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingSlot : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    public Button slotButton; // 클릭용 버튼
    public Image iconImage; // 건물의 타일이나 스프라이트 아이콘을 보여줄 이미지
    public TextMeshProUGUI nameText; // 건물 이름 텍스트
    public TextMeshProUGUI costText; // 건설 비용 텍스트

    [Header("데이터 참조")]
    private BuildingData assignedBuilding; // 이 슬롯이 품고 있는 건물 데이터
    private int buildingIndex; // 전체 리스트에서의 인덱스

    public void SetupSlot(BuildingData data, int index)
    {
        assignedBuilding = data;
        buildingIndex = index;

        //텍스트 설정
        if (nameText != null)
        {
            nameText.text = data.buildingName;
        }

        if (costText != null)
        {
            costText.text = $"자원: 나무 {data.treeCost}개/ 돌 {data.rockCost}개";
        }
            
        if (iconImage != null && data.buildingTile != null)
        {
            iconImage.sprite = data.buildingIcon;
        }

        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(OnSlotClicked);
        }
    }

    //슬롯을 클릭했을 때 실행되는 함수
    void OnSlotClicked()
    {
        Debug.Log($"선택된 건물: {assignedBuilding.buildingName} (인덱스: {buildingIndex})");

        EventManager.TriggerBuildingSelected(assignedBuilding);
    }
}