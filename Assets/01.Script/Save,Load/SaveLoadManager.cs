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
        SaveData data = new SaveData();

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

    // 저장 파일 삭제 (테스트용)
    public static void DeleteSaveData()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("[SaveLoadManager] 저장 파일 삭제 완료");
        }
    }
}