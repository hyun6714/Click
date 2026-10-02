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
    }

    private void OnDisable()
    {
        EventManager.OnDemolishButtonClicked -= ToggleDemolishMode;
        EventManager.OnBuildingSelected -= HandleBuildingSelected;
    }

    void Update()
    {
        if (isDemolishMode && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleDemolishMode();
            Debug.Log("철거모드 off");
            return;
        }

        if (!isDemolishMode) return;

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

            // 자동 연결된 buildingPlacer에게 데이터 조회 후 철거 시도
            if (buildingPlacer != null)
            {
                BuildingData targetData = buildingPlacer.GetBuildingDataAt(cellPos);
                TryDemolishAt(cellPos, targetData);
            }
            else
            {
                Debug.LogWarning("BuildingPlacer가 할당되지 않았습니다!");
            }
        }
    }

    public void ToggleDemolishMode()
    {
        isDemolishMode = !isDemolishMode;
        Debug.Log(isDemolishMode ? "철거 모드 ON" : "철거 모드 OFF");

        if (!isDemolishMode)
        {
            EventManager.TriggerBuildMenuPopupToggle();
        }
    }

    public void TryDemolishAt(Vector3Int cellPos, BuildingData buildingData)
    {
        if (!buildingTilemap.HasTile(cellPos))
        {
            Debug.Log("철거할 건물이 없습니다.");
            return;
        }

        if (buildingData != null)
        {
            int refundWood = Mathf.FloorToInt(buildingData.treeCost * refundRate);
            int refundStone = Mathf.FloorToInt(buildingData.rockCost * refundRate);

            if (refundWood > 0) EventManager.CurrencyAdded(CurrencyType.Tree, refundWood);
            if (refundStone > 0) EventManager.CurrencyAdded(CurrencyType.Rock, refundStone);

            Debug.Log("건물 철거 성공 및 비용 환급 완료!");
        }

        buildingTilemap.SetTile(cellPos, null);

        if (buildingPlacer != null)
        {
            buildingPlacer.RemoveBuildingData(cellPos);
        }
    }

    private void HandleBuildingSelected(BuildingData buildingData)
    {
        if (isDemolishMode)
        {
            ToggleDemolishMode();
            Debug.Log("건물 선택으로 인해 철거 모드 해제");
        }
    }
}
