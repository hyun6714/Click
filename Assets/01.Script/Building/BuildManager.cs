using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingManager : MonoBehaviour
{
    [Header("건물타일맵 참조")]
    public Tilemap buildingTilemap;

    private Dictionary<Vector3Int, int> buildingHpDictionary = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, BuildingData> buildingDataDictionary = new Dictionary<Vector3Int, BuildingData>();

    [Header("메인 건물 전용 데이터")]
    public BuildingData mainBuildingData;

    private void OnEnable()
    {
        EventManager.OnRequestDemolish += HandleDemolishRequest;
        EventManager.OnRequestPlaceBuilding += HandlePlaceBuildingRequest;
    }

    private void OnDisable()
    {
        EventManager.OnRequestDemolish -= HandleDemolishRequest;
        EventManager.OnRequestPlaceBuilding -= HandlePlaceBuildingRequest;
    }

    void Start()
    {
        RegisterInitialBuildings();
    }

    // 게임 시작 시 메인 건물 자동 등록
    private void RegisterInitialBuildings()
    {
        if (buildingTilemap == null || mainBuildingData == null)
        {
            Debug.LogWarning("타일맵 or 메인건물 데이터 null");
            return;
        }

        foreach (var pos in buildingTilemap.cellBounds.allPositionsWithin)
        {
            if (buildingTilemap.HasTile(pos))
            {
                if (!buildingHpDictionary.ContainsKey(pos))
                {
                    buildingHpDictionary[pos] = mainBuildingData.buildingHp;
                    buildingDataDictionary[pos] = mainBuildingData;

                    Debug.Log($"[초기 메인 건물 등록] 좌표 {pos} | 건물: {mainBuildingData.buildingName} | HP: {mainBuildingData.buildingHp}");
                }
            }
        }
    }

    //건물 설치 신호 받을 시 설치 
    private void HandlePlaceBuildingRequest(Vector3Int cellPosition, BuildingData data)
    {
        buildingTilemap.SetTile(cellPosition, data.buildingTile);

        buildingHpDictionary[cellPosition] = data.buildingHp;
        buildingDataDictionary[cellPosition] = data;

        Debug.Log($"[설치 완료] 좌표 {cellPosition} | 건물: {data.buildingName} | HP: {data.buildingHp}");
    }

    //건물 공격 받을 시 
    public void DamageBuilding(Vector3Int cellPosition, int damage)
    {
        if (buildingHpDictionary.ContainsKey(cellPosition))
        {
            buildingHpDictionary[cellPosition] -= damage;
            int currentHp = buildingHpDictionary[cellPosition];

            Debug.Log($"[건물 피격] 좌표 {cellPosition} | 남은 HP: {currentHp}");

            // 체력이 0 이하가 되면 철거(파괴)
            if (currentHp <= 0)
            {
                HandleDemolishRequest(cellPosition);
            }
        }
    }

    //건물 철거,파괴 시 호출
    private void HandleDemolishRequest(Vector3Int cellPosition)
    {
        if (buildingDataDictionary.ContainsKey(cellPosition))
        {
            BuildingData data = buildingDataDictionary[cellPosition];

            int refundWood = Mathf.FloorToInt(data.treeCost * 0.5f);
            int refundStone = Mathf.FloorToInt(data.rockCost * 0.5f);

            if (refundWood > 0) EventManager.CurrencyAdded(CurrencyType.Tree, refundWood);
            if (refundStone > 0) EventManager.CurrencyAdded(CurrencyType.Rock, refundStone);

            buildingTilemap.SetTile(cellPosition, null);

            buildingHpDictionary.Remove(cellPosition);
            buildingDataDictionary.Remove(cellPosition);

            Debug.Log($"[건물 철거 성공] 좌표 {cellPosition} 철거 및 데이터 삭제 완료");
        }
        else
        {
            Debug.Log("철거할 건물의 데이터가 존재하지 않습니다.");
        }
    }
}