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

            }

            if (buildingPlacer != null)
            {
                LoadBuildingData(loadedData, buildingPlacer.availableBuildings);
            }
        }

        EventManager.TriggerBuildingManagerReady(buildingTilemap);
        EventManager.TriggerRequestSyncBuildings();

        isInitialized = true;
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
                    }
                }
            }

        }
    }

    public void BeginWaveBuildingCheckpoint()
    {
        SaveData data = SaveLoadManager.LoadGame() ?? new SaveData();

        SaveData snapshot = new SaveData();
        SaveBuildingData(snapshot);

        data.waveStartBuildings = new List<SaveData.BuildingSaveEntry>(snapshot.savedBuildings);

        data.waveCheckpointActive = true;

        SaveLoadManager.SaveRawData(data);
    }

    public void EndWaveBuildingCheckpoint()
    {
        SaveData data = SaveLoadManager.LoadGame();

        if (data == null)
        {
            return;
        }

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

        BuildingManager buildingMgr = this;

        SaveLoadManager.SaveGame(timeMgr, currencyMgr, buildingMgr, weatherMgr, unlockMgr);
    }

}
