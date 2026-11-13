using UnityEngine;
using System.Collections.Generic;

public class CategorizedToolbarManager : MonoBehaviour
{
    [Header("카테고리별 Content 부모 (스크롤 뷰 내부)")]
    public Transform attackContentParent;   // 공격용 스크롤의 Content

    [Header("프리팹 및 데이터")]
    public GameObject slotPrefab;           // 슬롯 프리팹
    public List<BuildingData> allBuildings; // 전체 건물 리스트

    private void OnEnable()
    {
        EventManager.OnBuildingUnlockUpdated += InitCategorizedToolbar;
    }

    private void OnDisable()
    {
        EventManager.OnBuildingUnlockUpdated -= InitCategorizedToolbar;
    }

    public void InitCategorizedToolbar()
    {
        ClearTransformChildren(attackContentParent);

        for (int i = 0; i < allBuildings.Count; i++)
        {
            BuildingData building = allBuildings[i];

            if (building == null)
            {
                continue;
            }

            if (!building.isUnlocked)
            {
                continue;
            }

            Transform targetParent = attackContentParent;

            if (targetParent == null)
            {
                continue;
            }

            GameObject slotObj = Instantiate(slotPrefab, targetParent);
            BuildingSlot slotScript = slotObj.GetComponent<BuildingSlot>();

            if (slotScript != null)
            {
                slotScript.SetupSlot(building, i);
            }
        }

        void ClearTransformChildren(Transform parent)
        {
            if (parent == null)
            {
                return;
            }

            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}