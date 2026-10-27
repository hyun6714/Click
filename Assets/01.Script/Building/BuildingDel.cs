using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

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
            Debug.LogWarning("씬에서 BuildingPlacer를 찾지 못했습니다!");
        }
    }
    private void OnEnable()
    {
        EventManager.OnDemolishButtonClicked += ToggleDemolishMode;
        EventManager.OnBuildingSelected += HandleBuildingSelected;
        EventManager.OnGameModeChanged += HandleGameModeChanged;
    }

    private void OnDisable()
    {
        EventManager.OnDemolishButtonClicked -= ToggleDemolishMode;
        EventManager.OnBuildingSelected -= HandleBuildingSelected;
        EventManager.OnGameModeChanged -= HandleGameModeChanged;
    }

    void Update()
    {
        if (isDemolishMode && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleDemolishMode();
            Debug.Log("철거모드 off");
            return;
        }

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
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector3Int cellPos = buildingTilemap.WorldToCell(mouseWorldPos);
            cellPos.z = 0;

            if (!buildingTilemap.HasTile(cellPos))
            {
                Debug.Log("철거할 건물이 없습니다.");
                return;
            }

            EventManager.TriggerRequestDemolish(cellPos);
        }
    }

    public void ToggleDemolishMode()
    {
        isDemolishMode = !isDemolishMode;
        Debug.Log(isDemolishMode ? "철거 모드 ON" : "철거 모드 OFF");

        GameMode currentMode = isDemolishMode ? GameMode.Del : GameMode.Normal;
        EventManager.ChangeGameMode(currentMode);

        if (!isDemolishMode)
        {
            EventManager.TriggerBuildMenuPopupToggle();
        }
    }

    private void HandleBuildingSelected(BuildingData buildingData)
    {
        if (isDemolishMode)
        {
            isDemolishMode = false;
            Debug.Log("건물 선택으로 인해 철거 모드 해제");
        }
    }

    private void HandleGameModeChanged(GameMode newMode)
    {
        if (newMode != GameMode.Del && isDemolishMode)
        {
            isDemolishMode = false;
            Debug.Log("모드 전환되어 철거 모드 자동 해제");
            EventManager.TriggerBuildMenuPopupToggle();
        }
    }
}
