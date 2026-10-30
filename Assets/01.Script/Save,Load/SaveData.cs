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

    public int clickCurrency = 0;
    public int treeCurrency = 0;
    public int rockCurrency = 0;
    public int goldCurrency = 0;

    public int savedMaxClickLimit;
    public int savedDailyProductionBonus;

    [System.Serializable]
    public struct BuildingSaveEntry
    {
        public int x;
        public int y;
        public int z;
        public string buildingName; // 건물 구분 고유 키
        public string tileName;
    }

    public List<BuildingSaveEntry> savedBuildings = new List<BuildingSaveEntry>();
    public List<string> unlockedBuildingNodes = new List<string>();

}