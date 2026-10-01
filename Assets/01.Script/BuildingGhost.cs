using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BuildingGhost : MonoBehaviour
{
    [Header("미리보기 전용 타일맵")]
    public Tilemap previewTilemap;
    public Tilemap referenceTilemap; // 좌표 변환을 위한 기준 타일맵 (buildingTilemap 등)

    [Header("고스트 시각적 피드백 색상")]
    public Color availableColor = new Color(0f, 1f, 0f, 0.6f); //설치 가능: 초록색
    public Color blockedColor = new Color(1f, 0f, 0f, 0.6f); //설치 불가능 : 빨간색

    [Header("중복 설치 방지 및 장애물 체크용 타일맵들")]
    public Tilemap buildingTilemap;
    public Tilemap stoneTilemap;
    public Tilemap treeTilemap;
    public Tilemap groundTilemap;

    [Header("건설 범위 제한 설정")]
    public int requiredBuildingRange = 3;

    [Header("건설 범위 시각화용 타일맵")]
    public Tilemap rangeTilemap; 
    public Tile ofRangeTile;

    private bool hasMainBuilding = false;

    private BuildingData currentBuilding;
    private Vector3Int lastCellPos = new Vector3Int(-999, -999, -999); 

    private void OnEnable()
    {
        StaticEventManager.OnBuildingSelected += HandleBuildingSelected;
    }

    private void OnDisable()
    {
        StaticEventManager.OnBuildingSelected -= HandleBuildingSelected;
        ClearGhost();
    }

    private void HandleBuildingSelected(BuildingData building)
    {
        currentBuilding = building;
        lastCellPos = new Vector3Int(-999, -999, -999);
    }

    void Update()
    {
        if (currentBuilding == null || previewTilemap == null || referenceTilemap == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClearGhost();
            return;
        }

        if (Mouse.current.delta.ReadValue() == Vector2.zero)
        {
            return;
        }
        UpdateGhostPosition();
    }

    private void UpdateGhostPosition()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector3Int cellPos = referenceTilemap.WorldToCell(mouseWorldPos);
        cellPos.z = 0;

        if (cellPos != lastCellPos)
        {
            lastCellPos = cellPos;

            previewTilemap.ClearAllTiles();
            previewTilemap.SetTile(cellPos, currentBuilding.buildingTile);

            bool canBuild = CheckCanBuildAt(cellPos);
            previewTilemap.color = canBuild ? availableColor : blockedColor;


            DrawBuildRange(cellPos);
        }
    }

    private void DrawBuildRange(Vector3Int centerCell)
    {
        if (rangeTilemap == null || ofRangeTile == null)
        {
            return;
        }

        rangeTilemap.ClearAllTiles();

        int range = requiredBuildingRange;
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > range)
                {
                    continue;
                }

                Vector3Int drawPos = centerCell + new Vector3Int(x, y, 0);
                rangeTilemap.SetTile(drawPos, ofRangeTile);
            }
        }
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
            bool isMapEmpty = true;
            foreach (var pos in buildingTilemap.cellBounds.allPositionsWithin)
            {
                if (buildingTilemap.HasTile(pos))
                {
                    isMapEmpty = false;
                    break;
                }
            }

            if (!isMapEmpty)
            {
                bool hasBuildingNearby = false;
                int range = requiredBuildingRange;

                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        if (Mathf.Abs(x) + Mathf.Abs(y) > range)
                        {
                            continue;
                        }

                        Vector3Int checkPos = cellPos + new Vector3Int(x, y, 0);

                        if (buildingTilemap.HasTile(checkPos))
                        {
                            hasBuildingNearby = true;
                            break;
                        }
                    }

                    if (hasBuildingNearby)
                    {
                        break;
                    }
                }

                if (!hasBuildingNearby)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void ClearGhost()
    {
        currentBuilding = null;
        lastCellPos = new Vector3Int(-999, -999, -999);

        if (previewTilemap != null)
        {
            previewTilemap.ClearAllTiles();
            previewTilemap.color = Color.white;
        }

        if (rangeTilemap != null)
        {
            rangeTilemap.ClearAllTiles();
        }
    }
}