using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class BuildingManager : MonoBehaviour
{
    [Header("건물 타일맵 참조")]
    public Tilemap buildingTilemap;

    private Dictionary<Vector3Int, int> buildingHpDictionary = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, BuildingData> buildingDataDictionary = new Dictionary<Vector3Int, BuildingData>();

    [Header("메인 건물 전용 데이터")]
    public BuildingData mainBuildingData;

    [SerializeField] private GameObject demolitionEffectPrefab;
    [SerializeField] private float demolitionEffectDuration = 1f;
    private bool isInitialized = false;

    public bool IsInitialized => isInitialized;

    private void OnEnable()
    {
        EventManager.OnRequestDemolish += HandleDemolishRequest;
        EventManager.OnRequestPlaceBuilding += HandlePlaceBuildingRequest;
        EventManager.OnRequestDamageBuilding += HandleDamageBuildingRequest;
        EventManager.OnRequestSyncBuildings += HandleSyncBuildings;
        EventManager.OnCastleDestroyed += HandleCastleDestroyed;
    }

    private void OnDisable()
    {
        EventManager.OnRequestDemolish -= HandleDemolishRequest;
        EventManager.OnRequestPlaceBuilding -= HandlePlaceBuildingRequest;
        EventManager.OnRequestDamageBuilding -= HandleDamageBuildingRequest;
        EventManager.OnRequestSyncBuildings -= HandleSyncBuildings;
        EventManager.OnCastleDestroyed -= HandleCastleDestroyed;
    }

    private void Awake()
    {
        timeMgr = Object.FindFirstObjectByType<GameTimeManager>();
        currencyMgr = Object.FindFirstObjectByType<CurrencyManager>();
        weatherMgr = Object.FindFirstObjectByType<WeatherManager>();
        unlockMgr = Object.FindFirstObjectByType<BuildingUnlockManager>();
        buildingPlacer = Object.FindFirstObjectByType<BuildingPlacer>();
    }

    public List<Vector3Int> GetAllBuildingPositions()
    {
        return new List<Vector3Int>(buildingHpDictionary.Keys);
    }


    //건물 설치 신호 받을 시 설치 
    private void PlaceBuildingSilently(Vector3Int cellPosition, BuildingData data)
    {
        if (buildingTilemap != null && data.buildingTile != null)
        {
            buildingTilemap.SetTile(cellPosition, data.buildingTile);

            if (data is BarricadeBuildingData)
            {
                Vector3Int mainCell = cellPosition;
                bool foundMainBuilding = false;

                foreach (var building in buildingDataDictionary)
                {
                    if (building.Value != null &&
                        building.Value.category == BuildingCategory.Main)
                    {
                        mainCell = building.Key;
                        foundMainBuilding = true;
                        break;
                    }
                }

                if (foundMainBuilding)
                {
                    int deltaX = Mathf.Abs(cellPosition.x - mainCell.x);
                    int deltaY = Mathf.Abs(cellPosition.y - mainCell.y);

                    bool flip = deltaY <= deltaX;

                    Vector3 scale = flip
                        ? new Vector3(-1f, 1f, 1f)
                        : Vector3.one;

                    buildingTilemap.SetTransformMatrix(
                        cellPosition,
                        Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale)
                    );
                }
            }
        }

        int maxHp = data is BarricadeBuildingData barricadeHpData ? barricadeHpData.maxHealth : data.maxBuildingHp;

        buildingHpDictionary[cellPosition] = maxHp;
        buildingDataDictionary[cellPosition] = data;

        if (!(data is TrapBuildingData))
        {
            EventManager.TriggerBuildingRegistered(cellPosition);
        }

        Debug.Log($"[설치 완료] 좌표 {cellPosition} | 건물: {data.buildingName} | HP: {maxHp}");

        if (data is AttackBuildingData attackData)
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
        else if (data is FirepitData firepitData)
        {
            EventManager.TriggerRequestSpawnFirepit(cellPosition, firepitData);
        }
        else if (data is ClickBuildingData clickData)
        {
            EventManager.TriggerRequestSpawnBuilding(cellPosition, clickData);
        }
        else if (data is ProductionBoostBuildingData clickBoData)
        {
            EventManager.TriggerRequestSpawnBuilding(cellPosition, clickBoData);
        }
        else if (data is BarricadeBuildingData barricadeData)
        {
            EventManager.TriggerRequestSpawnBuilding(cellPosition, barricadeData);
        }

    }

    //건물 공격 받을 시 
    private void HandleDamageBuildingRequest(Vector3Int cellPosition, int damage)
    {
        if (damage <= 0 || !buildingHpDictionary.TryGetValue(cellPosition, out int currentHp))
        {
            return;
        }


        int remainingHp = Mathf.Max(0, currentHp - damage);
        buildingHpDictionary[cellPosition] = remainingHp;
        PlayTileHitEffect(cellPosition);
        EventManager.PlaySound(SoundCue.BuildingDemolish);
        Debug.Log($"[건물 피격] 좌표 {cellPosition} | 남은 HP: {remainingHp}");

        if (remainingHp <= 0)
        {
            HandleDemolishRequest(cellPosition);
        }
    }

    public bool TryGetBuildingHealth(Vector3Int cellPosition, out int health)
    {
        return buildingHpDictionary.TryGetValue(cellPosition, out health);
    }

    //건물 철거,파괴 시 호출
    private void HandleDemolishRequest(Vector3Int cellPosition)
    {
        if (buildingDataDictionary.ContainsKey(cellPosition))
        {
            EventManager.PlaySound(SoundCue.BuildingDemolish);

            BuildingData data = buildingDataDictionary[cellPosition];
            Vector3 effectPosition = buildingTilemap.GetCellCenterWorld(cellPosition);

            if (data.category == BuildingCategory.Main)
            {
                Debug.Log("[게임 오버] 메인 성이 파괴");

                EventManager.TriggerCastleDestroyed();
            }

            else
            {
                int refundWood = Mathf.FloorToInt(data.treeCost * 0.5f);
                int refundStone = Mathf.FloorToInt(data.rockCost * 0.5f);

                if (refundWood > 0)
                {
                    EventManager.CurrencyAdded(CurrencyType.Tree, refundWood);
                }

                if (refundStone > 0)
                {
                    EventManager.CurrencyAdded(CurrencyType.Rock, refundStone);
                }

            }

            buildingTilemap.SetTile(cellPosition, null);

            if (!(data is TrapBuildingData) && demolitionEffectPrefab != null)
            {
                GameObject effectInstance = Instantiate(demolitionEffectPrefab, effectPosition, Quaternion.identity);
                Destroy(effectInstance, demolitionEffectDuration);
            }

            buildingHpDictionary.Remove(cellPosition);
            buildingDataDictionary.Remove(cellPosition);
            EventManager.TriggerBuildingUnregistered(cellPosition);

            Debug.Log($"[건물 파괴,철거 성공] 좌표 {cellPosition} 철거 및 데이터 삭제 완료");

            TriggerSave();
        }
        else
        {
            Debug.Log("철거할 건물의 데이터가 존재하지 않습니다.");
        }
    }


    public void RegisterMainBuilding(Vector3Int cellPos, BuildingData data)
    {
        if (data == null)
        {
            Debug.LogError("mainBuildingData가 비어 있습니다.");
            return;
        }

        buildingDataDictionary[cellPos] = data;
        buildingHpDictionary[cellPos] = data.maxBuildingHp;
    }

    private void HandleSyncBuildings()
    {
        if (buildingTilemap != null)
        {
            EventManager.TriggerBuildingManagerReady(buildingTilemap);
        }

        foreach (var pos in buildingHpDictionary.Keys)
        {
            if (buildingDataDictionary.TryGetValue(pos, out BuildingData data) && data is TrapBuildingData)
            {
                continue;
            }

            EventManager.TriggerBuildingRegistered(pos);
        }

        foreach (var kvp in buildingDataDictionary)
        {
            Vector3Int pos = kvp.Key;
            BuildingData data = kvp.Value;

            if (data is AttackBuildingData attackData)
            {
                EventManager.TriggerRequestSpawnTower(pos, attackData);
            }
            else if (data is ProductionBuildingData prodData)
            {
                EventManager.TriggerRequestSpawnResourceBuilding(pos, prodData);
            }
            else if (data is TrapBuildingData trapData)
            {
                EventManager.TriggerRequestSpawnTrapBuilding(pos, trapData);
            }
            else if (data is FirepitData firepitData)
            {
                EventManager.TriggerRequestSpawnFirepit(pos, firepitData);
            }
            else if (data is ClickBuildingData clickData)
            {
                EventManager.TriggerRequestSpawnBuilding(pos, data);
            }
            else if (data is ProductionBoostBuildingData clickBoData)
            {
                EventManager.TriggerRequestSpawnBuilding(pos, data);
            }
            else if (data is BarricadeBuildingData barricadeData)
            {
                EventManager.TriggerRequestSpawnBuilding(pos, barricadeData);
            }
        }
    }

    private void HandleCastleDestroyed()
    {
        buildingHpDictionary.Clear();
        buildingDataDictionary.Clear();

        if (buildingTilemap != null)
        {
            buildingTilemap.ClearAllTiles();
        }

        Debug.Log($"빌드 타일 Clear ");
    }

    private void HandlePlaceBuildingRequest(Vector3Int cellPosition, BuildingData data)
    {
        PlaceBuildingSilently(cellPosition, data);
        EventManager.PlaySound(SoundCue.BuildingPlace);
        TriggerSave();
    }

}