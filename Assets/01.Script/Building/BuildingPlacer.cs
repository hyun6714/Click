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
    private Vector3Int mainBuildingCellPos;

    public TileBase transparentBlockingTile;
    private BuildingData currentBuildingToPlace;

    private void OnEnable()
    {
        EventManager.OnBuildingSelected += HandleBuildingSelected;
    }

    private void OnDisable()
    {
        EventManager.OnBuildingSelected -= HandleBuildingSelected;
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
                mainBuildingCellPos = pos;

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

            // 마우스 좌클릭 시 설치 시도
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPlaceBuilding();
        }

    }

    private void HandleBuildingSelected(BuildingData selectedBuilding)
    {
        currentBuildingToPlace = selectedBuilding;

        EventManager.ChangeGameMode(GameMode.Build);
        EventManager.TriggerBuildAddPopupToggle(true);
    }

    //몇번째 건물 인식
    public void SelectBuildingToPlace(int index)
    {
        if (availableBuildings == null || index < 0 || index >= availableBuildings.Count)
        {
            currentBuildingToPlace = null;
            return;
        }

        currentBuildingToPlace = availableBuildings[index];

        EventManager.ChangeGameMode(GameMode.Build);
    }

    void TryPlaceBuilding()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector3Int cellPos = buildingTilemap.WorldToCell(mouseWorldPos);
        cellPos.z = 0;

        if (!CheckCanBuildAt(cellPos))
        {
            return;
        }

        int treeCost = currentBuildingToPlace.treeCost;
        int rockCost = currentBuildingToPlace.rockCost;

        bool hasEnoughTree = treeCost <= 0 || EventManager.RequestCheckCurrency(CurrencyType.Tree, treeCost);

        bool hasEnoughRock = rockCost <= 0 || EventManager.RequestCheckCurrency(CurrencyType.Rock, rockCost);

        if (!hasEnoughTree || !hasEnoughRock)
        {
            EventManager.PlaySound(SoundCue.ResourceInsufficient);
            return;
        }

        // 나무부터 차감
        bool treeSpent = treeCost <= 0 || EventManager.RequestUseCurrency(CurrencyType.Tree, treeCost);

        if (!treeSpent)
        {
            return;
        }

        bool rockSpent = rockCost <= 0 || EventManager.RequestUseCurrency(CurrencyType.Rock, rockCost);

        if (!rockSpent)
        {
            if (treeCost > 0)
            {
                EventManager.CurrencyAdded(CurrencyType.Tree, treeCost);
            }

            return;
        }

        EventManager.TriggerRequestPlaceBuilding(cellPos, currentBuildingToPlace);
        hasMainBuilding = true;
    }

    private bool CheckCanBuildAt(Vector3Int startCellPos)
    {
        if (currentBuildingToPlace == null)
        {
            return false;
        }

        int width = currentBuildingToPlace.sizeWidth > 0 ? currentBuildingToPlace.sizeWidth : 1;
        int height = currentBuildingToPlace.sizeHeight > 0 ? currentBuildingToPlace.sizeHeight : 1;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPos = new Vector3Int(startCellPos.x + x, startCellPos.y + y, startCellPos.z);

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
            }
        }

        if (buildingTilemap != null)
        {
            bool isNearAnyBuilding = false;

            foreach (var pos in buildingTilemap.cellBounds.allPositionsWithin)
            {
                if (buildingTilemap.HasTile(pos))
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            Vector3Int cellPos = new Vector3Int(startCellPos.x + x, startCellPos.y + y, startCellPos.z);
                            int distance = Mathf.Abs(cellPos.x - pos.x) + Mathf.Abs(cellPos.y - pos.y);

                            if (distance <= requiredBuildingRange)
                            {
                                isNearAnyBuilding = true;
                                break;
                            }
                        }

                        if (isNearAnyBuilding)
                        {
                            break;
                        }
                    }
                }

                if (isNearAnyBuilding)
                {
                    break;
                }
            }

            if (!isNearAnyBuilding)
            {
                return false;
            }
        }

        return true;
    }

    public void ClearCurrentBuilding()
    {
        currentBuildingToPlace = null;
        EventManager.TriggerBuildAddPopupToggle(false);
    }

    public void CancelPlacing()
    {
        ClearCurrentBuilding();
        EventManager.ChangeGameMode(GameMode.Normal);
    }

    public void ConfirmBuild(Vector3Int cellPos, BuildingData data)
    {
        EventManager.TriggerRequestPlaceBuilding(cellPos, data);
    }
}
