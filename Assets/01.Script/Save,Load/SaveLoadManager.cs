using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    // 게임 저장
    public static void SaveGame(
        GameTimeManager timeManager,
        CurrencyManager currencyManager,
        BuildingManager buildingManager,
        WeatherManager weatherManager,
        BuildingUnlockManager unlockManager)
    {
        SaveData previousData = LoadGame();
        SaveData data = new SaveData();

        if (previousData != null)
        {
            data.mapSeed = previousData.mapSeed;
            data.removedTrees = previousData.removedTrees ?? new System.Collections.Generic.List<SaveData.SerializableVector3Int>();
            data.removedStones = previousData.removedStones ?? new System.Collections.Generic.List<SaveData.SerializableVector3Int>();

            data.waveCheckpointActive = previousData.waveCheckpointActive;
            data.waveStartBuildings = previousData.waveStartBuildings ?? new System.Collections.Generic.List<SaveData.BuildingSaveEntry>();

            data.sfxVolume = previousData.sfxVolume;
            data.bgmVolume = previousData.bgmVolume;
            
            data.screenWidth = previousData.screenWidth;
            data.screenHeight = previousData.screenHeight;
            data.isFullScreen = previousData.isFullScreen;
        }

        if (timeManager != null)
        {
            data.day = timeManager.CurrentDay;
            data.hour = timeManager.CurrentHour;
            data.minute = timeManager.CurrentMinute;
            data.second = timeManager.CurrentSecond;
        }

        if (currencyManager != null)
        {
            currencyManager.SaveCurrencyData(data);
        }

        if (buildingManager != null)
        {
            buildingManager.SaveBuildingData(data);
        }

        if (weatherManager != null)
        {
            data.currentWeather = weatherManager.CurrentWeather.ToString();
        }

        if (unlockManager != null)
        {
            unlockManager.SaveUnlockData(data);
        }

        data.revealedFogList = EventManager.RequestRevealedFogList();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"[SaveLoadManager] 게임 저장 완료: {SavePath}");
    }

    // 게임 불러오기
    public static SaveData LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            return null;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        Debug.Log("[SaveLoadManager] 게임 로드 성공!");
        return data;
    }

    public static void SaveRawData(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"[SaveLoadManager] 체크포인트 저장 완료: {SavePath}");
    }

    public static void DeleteSaveData()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("[SaveLoadManager] 저장 파일 삭제 완료");
        }
    }
}