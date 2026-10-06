using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    [Header("타일맵 연결")]
    public Tilemap buildingTilemap; 
    public Tilemap groundTilemap; 
    public Tilemap treeTilemap;
    public Tilemap stoneTilemap;

    [Header("건설 범위 제한 설정")]
    public int requiredBuildingRange = 3;

    [Header("설치할 건물 데이터")]
    public List<BuildingData> availableBuildings;

    [Header("메인 건물 설치 여부 (자동 관리)")]
    private bool hasMainBuilding = false;

    public TileBase transparentBlockingTile;
    private BuildingData currentBuildingToPlace;

    private void OnEnable()
    {
        EventManager.OnBuildingSelected += HandleBuildingSelected;
        EventManager.OnDemolishButtonClicked += CancelPlacing;
    }

    private void OnDisable()
    {
        EventManager.OnBuildingSelected -= HandleBuildingSelected;
        EventManager.OnDemolishButtonClicked -= CancelPlacing;
    }

    void Start()
    {
        CheckExistingBuildings();
    }

    private void CheckExistingBuildings()
    {
        if (buildingTilemap == null)
        {
            return;
        }

        foreach (var pos in buildingTilemap.cellBounds.allPositionsWithin)
        {
            if (buildingTilemap.HasTile(pos))
            {
                hasMainBuilding = true;
                break;
            }
        }
    }

    void Update()
    {
        if (Mouse.current == null || currentBuildingToPlace == null)
        {
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (currentBuildingToPlace == null)
        {
            Debug.Log("설치모드 OFF");
        }

            // 마우스 좌클릭 시 설치 시도
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPlaceBuilding();
        }

    }

    private void HandleBuildingSelected(BuildingData selectedBuilding)
    {
        currentBuildingToPlace = selectedBuilding;
        Debug.Log($"설치 대기 중: {currentBuildingToPlace.buildingName}");
    }

    //몇번째 건물 인식
    public void SelectBuildingToPlace(int index)
    {
        if (availableBuildings == null || index < 0 || index >= availableBuildings.Count)
        {
            Debug.LogWarning("잘못된 건물 인덱스입니다!");
            currentBuildingToPlace = null;
            return;
        }

        currentBuildingToPlace = availableBuildings[index];
        Debug.Log($"건물 선택됨: {currentBuildingToPlace.buildingName} (인덱스: {index})");
    }

    void TryPlaceBuilding()
    {
        Debug.Log("설치모드 ON");

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector3Int cellPos = buildingTilemap.WorldToCell(mouseWorldPos);
        cellPos.z = 0;

        if (!CheckCanBuildAt(cellPos))
        {
            Debug.Log("이곳에는 건물을 지을 수 없습니다!");
            return;
        }

        int treeCost = currentBuildingToPlace.treeCost;
        int rockCost = currentBuildingToPlace.rockCost;

        bool success = EventManager.RequestCheckCurrency(CurrencyType.Tree, treeCost) &&
                       EventManager.RequestCheckCurrency(CurrencyType.Rock, rockCost);

        if (!success)
        {
            Debug.Log("자원 부족");
            return;
        }

        EventManager.RequestUseCurrency(CurrencyType.Tree, treeCost);
        EventManager.RequestUseCurrency(CurrencyType.Rock, rockCost);
        EventManager.TriggerRequestPlaceBuilding(cellPos, currentBuildingToPlace);

        Debug.Log($"[설치 완료] 좌표 {cellPos} | 건물: {currentBuildingToPlace.buildingName}");

        hasMainBuilding = true;
        currentBuildingToPlace = null;
    }

    private bool CheckCanBuildAt(Vector3Int cellPos)
    {
        if (groundTilemap != null && !groundTilemap.HasTile(cellPos))
        {
            return false;
        }

        if (buildingTilemap != null && buildingTilemap.HasTile(cellPos))
        {
            return false;
        }

        if (stoneTilemap != null && stoneTilemap.HasTile(cellPos))
        {
            return false;
        }

        if (treeTilemap != null && treeTilemap.HasTile(cellPos))
        {
            return false;
        }

        if (buildingTilemap != null)
        {
            Vector3Int? mainBuildingPos = null;

            foreach (var pos in buildingTilemap.cellBounds.allPositionsWithin)
            {
                if (buildingTilemap.HasTile(pos))
                {
                    TileBase tile = buildingTilemap.GetTile(pos);

                    if (transparentBlockingTile != null && tile == transparentBlockingTile)
                    {
                        continue; 
                    }

                    mainBuildingPos = pos;
                    break;
                }
            }

            if (mainBuildingPos.HasValue)
            {
                int distance = Mathf.Abs(cellPos.x - mainBuildingPos.Value.x) + Mathf.Abs(cellPos.y - mainBuildingPos.Value.y);

                if (distance > requiredBuildingRange)
                {
                    return false;
                }
            }
        }


        return true;
    }
       
    public void CancelPlacing()
    {
        currentBuildingToPlace = null;
        Debug.Log("설치 모드 OFF");
    }

    public void ConfirmBuild(Vector3Int cellPos, BuildingData data)
    {
        EventManager.TriggerRequestPlaceBuilding(cellPos, data);
    }
}
