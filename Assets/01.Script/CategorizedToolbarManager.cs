using UnityEngine;
using System.Collections.Generic;

public class CategorizedToolbarManager : MonoBehaviour
{
    [Header("카테고리별 Content 부모 (스크롤 뷰 내부)")]
    public Transform resourceContentParent; // 자원용 스크롤의 Content
    public Transform attackContentParent;   // 공격용 스크롤의 Content
    public Transform trapContentParent;     // 함정용 스크롤의 Content

    [Header("프리팹 및 데이터")]
    public GameObject slotPrefab;           // 슬롯 프리팹
    public List<BuildingData> allBuildings; // 전체 건물 리스트

    void Start()
    {
        InitCategorizedToolbar();
    }

    public void InitCategorizedToolbar()
    {
        ClearTransformChildren(resourceContentParent);
        ClearTransformChildren(attackContentParent);
        ClearTransformChildren(trapContentParent);

        Debug.Log($"[Toolbar] 전체 건물 개수: {allBuildings.Count}");

        for (int i = 0; i < allBuildings.Count; i++)
        {
            BuildingData building = allBuildings[i];

            if (!building.isUnlocked)
            {
                Debug.Log($"[Toolbar] 잠겨서 스킵된 건물: {building.buildingName}");
                continue;
            }

            Transform targetParent = null;
            switch (building.category)
            {
                case BuildingCategory.Resource:
                    targetParent = resourceContentParent;
                    break;
                case BuildingCategory.Attack:
                    targetParent = attackContentParent;
                    break;
                case BuildingCategory.Trap:
                    targetParent = trapContentParent;
                    break;
            }

            if (targetParent == null)
            {
                Debug.LogWarning($"[Toolbar] 카테고리 부모가 연결되지 않음: {building.buildingName}");
                continue;
            }

            // 해당 스크롤 뷰 안에 슬롯 생성 및 바인딩
            GameObject slotObj = Instantiate(slotPrefab, targetParent);
            BuildingSlot slotScript = slotObj.GetComponent<BuildingSlot>();

            if (slotScript != null)
            {
                slotScript.SetupSlot(building, i);
            }
            else
            {
                Debug.LogError("[Toolbar] 생성된 슬롯 프리팹에 BuildingSlot 스크립트가 없습니다!");
            }
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