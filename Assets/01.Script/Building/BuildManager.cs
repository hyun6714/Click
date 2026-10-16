using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingManager : MonoBehaviour
{
    [Header("건물 타일맵 참조")]
    public Tilemap buildingTilemap;

    private Dictionary<Vector3Int, int> buildingHpDictionary = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, BuildingData> buildingDataDictionary = new Dictionary<Vector3Int, BuildingData>();

    [Header("메인 건물 전용 데이터")]
    public BuildingData mainBuildingData;

    private void OnEnable()
    {
        EventManager.OnRequestDemolish += HandleDemolishRequest;
        EventManager.OnRequestPlaceBuilding += HandlePlaceBuildingRequest;
        EventManager.OnRequestDamageBuilding += HandleDamageBuildingRequest;
    }

    private void OnDisable()
    {
        EventManager.OnRequestDemolish -= HandleDemolishRequest;
        EventManager.OnRequestPlaceBuilding -= HandlePlaceBuildingRequest;
        EventManager.OnRequestDamageBuilding -= HandleDamageBuildingRequest;
    }

    void Start()
    {
        EventManager.TriggerBuildingManagerReady(buildingTilemap);
        RegisterInitialBuildings();
    }

    public List<Vector3Int> GetAllBuildingPositions()
    {
        return new List<Vector3Int>(buildingHpDictionary.Keys);
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
                    buildingHpDictionary[pos] = mainBuildingData.maxBuildingHp;
                    buildingDataDictionary[pos] = mainBuildingData;

                    Debug.Log($"[초기 메인 건물 등록] 좌표 {pos} | 건물: {mainBuildingData.buildingName} | HP: {mainBuildingData.maxBuildingHp}");

                    EventManager.TriggerRequestSpawnMainCastle(pos, mainBuildingData);
                }
            }
        }
    }

    //건물 설치 신호 받을 시 설치 
    private void HandlePlaceBuildingRequest(Vector3Int cellPosition, BuildingData data)
    {
        buildingTilemap.SetTile(cellPosition, data.buildingTile);

        buildingHpDictionary[cellPosition] = data.maxBuildingHp;
        buildingDataDictionary[cellPosition] = data;

        Debug.Log($"[설치 완료] 좌표 {cellPosition} | 건물: {data.buildingName} | HP: {data.maxBuildingHp}");

        if (data.category == BuildingCategory.Main)
        {
            EventManager.TriggerRequestSpawnMainCastle(cellPosition, data);
        }
        else if (data is AttackBuildingData attackData)
        {
            EventManager.TriggerRequestSpawnTower(cellPosition, attackData);
        }

        else if (data is ProductionBuildingData prodData)
        {
            EventManager.TriggerRequestSpawnResourceBuilding(cellPosition, prodData);
        }

        else if (data is TrapBuildingData trapData)
        {
            EventManager.TriggerRequestSpawnTrapBuilding(cellPosition, trapData);
        }
    }

    //건물 공격 받을 시 
    private void HandleDamageBuildingRequest(Vector3Int cellPosition, int damage)
    {
        if (buildingHpDictionary.ContainsKey(cellPosition))
        {
            buildingHpDictionary[cellPosition] -= damage;
            int currentHp = buildingHpDictionary[cellPosition];

            Debug.Log($"[건물 피격] 좌표 {cellPosition} | 남은 HP: {currentHp}");

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

            if (data.category == BuildingCategory.Main)
            {
                Debug.Log("[게임 오버] 메인 성이 파괴");

            }

            else
            {
                int refundWood = Mathf.FloorToInt(data.treeCost * 0.5f);
                int refundStone = Mathf.FloorToInt(data.rockCost * 0.5f);

                if (refundWood > 0) EventManager.CurrencyAdded(CurrencyType.Tree, refundWood);
                if (refundStone > 0) EventManager.CurrencyAdded(CurrencyType.Rock, refundStone);
            }

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