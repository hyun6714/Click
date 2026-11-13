using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class BuildingDel : MonoBehaviour
{
    [Header("바닥/건물 타일맵")]
    public Tilemap referenceTilemap; //바닥 타일맵
    public Tilemap buildingTilemap; //건물 타일맵
    public BuildingPlacer buildingPlacer;

    [Header("철거 후 환급량")]
    [SerializeField] private float refundRate = 0.5f;

    private bool isDemolishMode = false;

    private void Awake()
    {
        buildingPlacer = FindAnyObjectByType<BuildingPlacer>();

        if (buildingPlacer == null)
        {
        }
    }
    private void OnEnable()
    {
        EventManager.OnDemolishButtonClicked += ToggleDemolishMode;
        EventManager.OnBuildingSelected += HandleBuildingSelected;
        EventManager.OnGameModeChanged += HandleGameModeChanged;
        EventManager.OnRequestIsDemolishMode += GetIsDemolishMode;
    }

    private void OnDisable()
    {
        EventManager.OnDemolishButtonClicked -= ToggleDemolishMode;
        EventManager.OnBuildingSelected -= HandleBuildingSelected;
        EventManager.OnGameModeChanged -= HandleGameModeChanged;
        EventManager.OnRequestIsDemolishMode -= GetIsDemolishMode;
    }

    void Update()
    {
        if (!isDemolishMode)
        {
            return;
        }
        ProcessDemolishInput();
    }

    private void ProcessDemolishInput()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector3Int cellPos = buildingTilemap.WorldToCell(mouseWorldPos);
            cellPos.z = 0;

            if (!buildingTilemap.HasTile(cellPos))
            {
                return;
            }

            EventManager.TriggerRequestDemolish(cellPos);
        }
    }

    public void ToggleDemolishMode()
    {
        isDemolishMode = !isDemolishMode;

        if (isDemolishMode)
        {
            if (buildingPlacer != null)
            {
                buildingPlacer.ClearCurrentBuilding();
            }

            EventManager.ChangeGameMode(GameMode.Del);
        }
        else
        {
            EventManager.ChangeGameMode(GameMode.Normal);
            EventManager.TriggerBuildMenuPopupToggle();
        }
    }

    private void HandleBuildingSelected(BuildingData buildingData)
    {
        if (isDemolishMode)
        {
            isDemolishMode = false;
            EventManager.TriggerBuildMenuPopupToggle();
        }
    }

    private void HandleGameModeChanged(GameMode newMode)
    {
        if (newMode != GameMode.Del && isDemolishMode)
        {
            isDemolishMode = false;
            EventManager.TriggerBuildMenuPopupToggle();
        }
    }

    private bool GetIsDemolishMode()
    {
        return isDemolishMode;
    }
}
