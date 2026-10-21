using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingSelector : MonoBehaviour
{
    [Header("UI 참조")]
    public BuildingInfoUI buildingInfoUI;

    [Header("레이어 설정")]
    public LayerMask buildingLayer;

    private void Update()
    {
        // 마우스 좌클릭
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            TrySelectBuilding();
        }
    }

    private void TrySelectBuilding()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 mouseWorldPos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos2D, Vector2.zero, 0f, buildingLayer);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (clickedObject.TryGetComponent<AttackBuilding>(out var attackBuilding))
            {
                if (buildingInfoUI != null)
                {
                    buildingInfoUI.ShowBuildingInfo(attackBuilding.CellPosition, attackBuilding.TowerData, attackBuilding.CurrentHealth);
                }
            }

            else if (clickedObject.TryGetComponent<ResourceBuilding>(out var resourceBuilding))
            {
                if (buildingInfoUI != null)
                {
                    buildingInfoUI.ShowBuildingInfo(resourceBuilding.CellPosition, resourceBuilding.BuildingData, resourceBuilding.CurrentHealth);
                }
            }
        }
        else
        {
            if (buildingInfoUI != null)
            {
                buildingInfoUI.HidePanel();
            }
        }
    }
}