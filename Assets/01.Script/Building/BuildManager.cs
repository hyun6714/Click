using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

public class BuildingManager : MonoBehaviour
{
    [Header("건물 타일맵 참조")]
    public Tilemap buildingTilemap;

    private Dictionary<Vector3Int, int> buildingHpDictionary = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, BuildingData> buildingDataDictionary = new Dictionary<Vector3Int, BuildingData>();

    [Header("메인 건물 전용 데이터")]
    public BuildingData mainBuildingData;

    private bool isInitialized = false;

    private GameTimeManager timeMgr;
    private CurrencyManager currencyMgr;
    private WeatherManager weatherMgr;
    private BuildingUnlockManager unlockMgr;
    private BuildingPlacer buildingPlacer;

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

    void Start()
    {
        InitializeGameAsync();
    }

    private async void InitializeGameAsync()
    {
        await Task.Yield();

        SaveData loadedData = SaveLoadManager.LoadGame();

        if (loadedData != null)
        {
            if (buildingPlacer != null)
            {
                LoadBuildingData(loadedData, buildingPlacer.availableBuildings);
            }
        }

        EventManager.TriggerBuildingManagerReady(buildingTilemap);

        isInitialized = true;
        Debug.Log("[BuildingManager] 비동기 지연 로드 및 초기화 완료");
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
        }

        buildingHpDictionary[cellPosition] = data.maxBuildingHp;
        buildingDataDictionary[cellPosition] = data;

        EventManager.TriggerBuildingRegistered(cellPosition);

        Debug.Log($"[설치 완료] 좌표 {cellPosition} | 건물: {data.buildingName} | HP: {data.maxBuildingHp}");

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

                EventManager.TriggerCastleDestroyed();
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
            EventManager.TriggerBuildingUnregistered(cellPosition);

            Debug.Log($"[건물 철거 성공] 좌표 {cellPosition} 철거 및 데이터 삭제 완료");

            TriggerSave();
        }
        else
        {
            Debug.Log("철거할 건물의 데이터가 존재하지 않습니다.");
        }
    }

    private void HandleSyncBuildings()
    {
        if (buildingTilemap != null)
        {
            EventManager.TriggerBuildingManagerReady(buildingTilemap);
        }

        foreach (var pos in buildingHpDictionary.Keys)
        {
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
        TriggerSave();
    }

    public void SaveBuildingData(SaveData data)
    {
        if(data == null)
        {
            return;
        }

        data.savedBuildings.Clear();

        foreach (var pair in buildingDataDictionary)
        {
            Vector3Int pos = pair.Key;
            BuildingData buildingData = pair.Value;

            SaveData.BuildingSaveEntry entry = new SaveData.BuildingSaveEntry();
            entry.x = pos.x;
            entry.y = pos.y;
            entry.z = pos.z;
            entry.buildingName = buildingData.buildingName;

            entry.tileName = (buildingData.buildingTile != null) ? buildingData.buildingTile.name : "";

            data.savedBuildings.Add(entry);
        }
    }

    public void LoadBuildingData(SaveData data, List<BuildingData> availableBuildings)
    {
        if (data == null || data.savedBuildings == null)
        { 
            return;
        }

        foreach (var entry in data.savedBuildings)
        {
            Vector3Int pos = new Vector3Int(entry.x, entry.y, entry.z);

            BuildingData foundData = availableBuildings.Find(b => b.buildingName == entry.buildingName);

            if (foundData != null)
            {
                PlaceBuildingSilently(pos, foundData);

                Debug.Log($"[로드 체크] 건물: {entry.buildingName}, 저장된 tileName: '{entry.tileName}'");

                if (buildingTilemap != null && !string.IsNullOrEmpty(entry.tileName))
                {
                    TileBase targetTile = null;
                    foreach (var b in availableBuildings)
                    {
                        if (b.buildingTile != null && b.buildingTile.name == entry.tileName)
                        {
                            targetTile = b.buildingTile;
                            break;
                        }
                    }

                    //못 찾으면 직접 찾기 
                    if (targetTile != null)
                    {
                        buildingTilemap.SetTile(pos, targetTile);
                        Debug.Log($"[타일 꽂기 성공] 좌표 {pos}에 타일 [{targetTile.name}] 꽂음!");
                    }
                    else
                    {
                        // 🌟 2. 목록에서 타일 이름을 못 찾았을 때 뜨는 경고
                        Debug.LogWarning($"[타일 찾기 실패] availableBuildings 목록에서 '{entry.tileName}' 이름을 가진 타일을 못 찾았습니다!");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[Load] 저장된 건물의 ScriptableObject = Null: {entry.buildingName}");
            }
        }
        Debug.Log("[BuildingManager] 건물 데이터 복원 완료");
    }

    private void TriggerSave()
    {
        if (!isInitialized)
        {
            return;
        }

        Debug.Log("[디버그] TriggerSave 함수 진입 성공");
        BuildingManager buildingMgr = this;

        SaveLoadManager.SaveGame(timeMgr, currencyMgr, buildingMgr, weatherMgr, unlockMgr);
        Debug.Log("[BuildingManager] 건물 변경으로 인한 자동 저장 완료");
    }
}