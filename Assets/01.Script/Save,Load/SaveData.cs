using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int day = 1;
    public float hour = 7f;
    public float minute = 0f;
    public float second = 0f;

    public string currentWeather;

    public int clickCurrency;
    public int treeCurrency;
    public int rockCurrency;
    public int goldCurrency;


    public int savedMaxClickLimit = 40;
    public int savedDailyProductionBonus;

    public float sfxVolume = 1f;
    public float bgmVolume = 1f;

    public int screenWidth = 1920;
    public int screenHeight = 1080;
    public bool isFullScreen = true;

    [System.Serializable]
    public struct BuildingSaveEntry
    {
        public int x;
        public int y;
        public int z;
        public string buildingName; // 건물 구분 고유 키
        public string tileName;
    }

    [System.Serializable]
    public struct SerializableVector3Int
    {
        public int x;
        public int y;
        public int z;

        public SerializableVector3Int(Vector3Int v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3Int ToVector3Int() => new Vector3Int(x, y, z);
    }

    public List<BuildingSaveEntry> savedBuildings = new List<BuildingSaveEntry>();

    public bool waveCheckpointActive;
    public List<BuildingSaveEntry> waveStartBuildings = new List<BuildingSaveEntry>();

    public List<string> unlockedBuildingNodes = new List<string>();

    public List<SerializableVector3Int> revealedFogList = new List<SerializableVector3Int>();

    public void InitWithDatabase(CurrencyDatabase db)
    {
        if (db == null || db.currencies == null)
        {
            return;
        }

        foreach (var data in db.currencies)
        {
            switch (data.type)
            {
                case CurrencyType.Click:
                    clickCurrency = data.initialAmount;
                    break;
                case CurrencyType.Tree:
                    treeCurrency = data.initialAmount;
                    break;
                case CurrencyType.Rock:
                    rockCurrency = data.initialAmount;
                    break;
                case CurrencyType.Gold:
                    goldCurrency = data.initialAmount;
                    break;
            }
        }
    }

}