using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class BuildingManager
{
    private GameTimeManager timeMgr;
    private CurrencyManager currencyMgr;
    private WeatherManager weatherMgr;
    private BuildingUnlockManager unlockMgr;
    private BuildingPlacer buildingPlacer;

    private void Start()
    {
        InitializeGameAsync();
    }

    private async void InitializeGameAsync()
    {
        await Task.Yield();


        SaveData loadedData = SaveLoadManager.LoadGame();

        if (loadedData != null)
        {
            if (loadedData.waveCheckpointActive && loadedData.waveStartBuildings != null &&
                loadedData.waveStartBuildings.Count >= loadedData.savedBuildings.Count && loadedData.waveStartBuildings.Count > 0)
            {
                loadedData.savedBuildings = new List<SaveData.BuildingSaveEntry>(loadedData.waveStartBuildings);

                Debug.Log("[BuildingManager] 웨이브 시작 당시 건물 상태로 복원");
            }

            if (buildingPlacer != null)
            {
                LoadBuildingData(loadedData, buildingPlacer.availableBuildings);
            }
        }

        EventManager.TriggerBuildingManagerReady(buildingTilemap);
        EventManager.TriggerRequestSyncBuildings();

        isInitialized = true;
        Debug.Log("[BuildingManager] 비동기 지연 로드 및 초기화 완료");
    }

    public void SaveBuildingData(SaveData data)
    {
        if (data == null)
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

    public void BeginWaveBuildingCheckpoint()
    {
        SaveData data = SaveLoadManager.LoadGame() ?? new SaveData();

        SaveData snapshot = new SaveData();
        SaveBuildingData(snapshot);

        data.waveStartBuildings = new List<SaveData.BuildingSaveEntry>(snapshot.savedBuildings);
        Debug.Log($"[웨이브 체크포인트 저장] 건물 수: {data.waveStartBuildings.Count}");

        data.waveCheckpointActive = true;

        SaveLoadManager.SaveRawData(data);
    }

    public void EndWaveBuildingCheckpoint()
    {
        SaveData data = SaveLoadManager.LoadGame();

        if (data == null)
            return;

        data.waveCheckpointActive = false;
        data.waveStartBuildings.Clear();

        SaveLoadManager.SaveRawData(data);
        TriggerSave();
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
