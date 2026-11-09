using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingSelector : MonoBehaviour
{
    [Header("UI 참조")]
    public BuildingInfoUI buildingInfoUI;
    [SerializeField] private BuildingManager buildingManager;

    [Header("레이어 설정")]
    public LayerMask buildingLayer;
    public LayerMask trapLayer;

    private void Update()
    {
        bool isDelMode = false;
        if (EventManager.OnRequestIsDemolishMode != null)
        {
            isDelMode = EventManager.OnRequestIsDemolishMode.Invoke();
        }

        if (isDelMode)
        {
            return;
        }

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
        LayerMask combinedLayer = buildingLayer | trapLayer;

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos2D, Vector2.zero, 0f, combinedLayer);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (clickedObject.TryGetComponent<AttackBuilding>(out var attackBuilding))
            {
                if (buildingInfoUI != null)
                {
                    buildingInfoUI.ShowBuildingInfo(attackBuilding.CellPosition, attackBuilding.TowerData, GetBuildingHealth(attackBuilding.CellPosition, attackBuilding.CurrentHealth));
                }
            }

            else if (clickedObject.TryGetComponent<ResourceBuilding>(out var resourceBuilding))
            {
                if (buildingInfoUI != null)
                {
                    buildingInfoUI.ShowBuildingInfo(resourceBuilding.CellPosition, resourceBuilding.BuildingData, GetBuildingHealth(resourceBuilding.CellPosition, resourceBuilding.CurrentHealth));
                }
            }

            else if (clickedObject.TryGetComponent<ClickBuilding>(out var clickBuilding))
            {
                buildingInfoUI.ShowBuildingInfo(clickBuilding.CellPosition, clickBuilding.ClickData, GetBuildingHealth(clickBuilding.CellPosition, clickBuilding.CurrentHealth));
            }

            else if (clickedObject.TryGetComponent<ProductionBoostBuilding>(out var boostBuilding))
            {
                buildingInfoUI.ShowBuildingInfo(boostBuilding.CellPosition, boostBuilding.BoostData, GetBuildingHealth(boostBuilding.CellPosition, boostBuilding.CurrentHealth));
            }

            else if (clickedObject.TryGetComponent<BarricadeBuilding>(out var barricadeBuilding))
            {
                buildingInfoUI.ShowBuildingInfo(barricadeBuilding.CellPosition, barricadeBuilding.BarricadeData, GetBuildingHealth(barricadeBuilding.CellPosition, barricadeBuilding.CurrentHealth));
            }

            else if (clickedObject.GetComponentInParent<TrapBuilding>() is var trap && trap != null)
            {
                buildingInfoUI.ShowBuildingInfo(trap.CellPosition, trap.TrapData, trap.CurrentHealth);
            }
            else if (clickedObject.GetComponentInParent<BombBuilding>() is var bomb && bomb != null)
            {
                buildingInfoUI.ShowBuildingInfo(bomb.CellPosition, bomb.TrapData, bomb.CurrentHealth);
            }

            else if (clickedObject.TryGetComponent<Firepit>(out var firepitBuilding))
            {
                buildingInfoUI.ShowBuildingInfo(firepitBuilding.CellPosition, firepitBuilding.FirepitData, GetBuildingHealth(firepitBuilding.CellPosition, firepitBuilding.CurrentHealth));
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

    private int GetBuildingHealth(Vector3Int cellPosition, int fallbackHealth)
    {
        if (buildingManager != null &&
            buildingManager.TryGetBuildingHealth(cellPosition, out int health))
        {
            return health;
        }

        return fallbackHealth;
    }
}