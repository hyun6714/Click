using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

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
        StaticEventManager.OnBuildingSelected += HandleBuildingSelected;
    }

    private void OnDisable()
    {
        StaticEventManager.OnBuildingSelected -= HandleBuildingSelected;
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

    private void HandleBuildingSelected(BuildingData selectedBuilding)
    {
        currentBuildingToPlace = selectedBuilding;
        Debug.Log($"[Placer] 설치 대기 중: {currentBuildingToPlace.buildingName}");
    }

    public void SelectBuildingToPlace(int index)
    {
        if(availableBuildings == null || index < 0 || index >= availableBuildings.Count)
        {
            Debug.LogWarning("잘못된 건물 인덱스입니다!");
            currentBuildingToPlace = null;
            return;
        }

        currentBuildingToPlace = availableBuildings[index];
        Debug.Log($"건물 선택됨: {currentBuildingToPlace.buildingName} (인덱스: {index})");
    }

    void Update()
    {
        if (Mouse.current == null || currentBuildingToPlace == null)
        {
            return;
        }

        // 마우스 좌클릭 시 설치 시도
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPlaceBuilding();
        }

    }

    void TryPlaceBuilding()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector3Int cellPos = buildingTilemap.WorldToCell(mouseWorldPos);
        cellPos.z = 0;

        if (!CheckCanBuildAt(cellPos))
        {
            Debug.Log("이곳에는 건물을 지을 수 없습니다!");
            return;
        }

        // 최종 설치 성공
        buildingTilemap.SetTile(cellPos, currentBuildingToPlace.buildingTile);
        Debug.Log($"{currentBuildingToPlace.buildingName} 설치 완료! 좌표: {cellPos}");

        hasMainBuilding = true;

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

    void CancelPlacing()
    {
        currentBuildingToPlace = null;
        Debug.Log("설치 모드 해제됨");
    }
}
