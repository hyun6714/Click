using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemySpawnInfo
{
    public EnemyData enemyData;
    public int spawnCount;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "WaveData")]
public class WaveData : ScriptableObject
{
    public int targetDay;

    public List<EnemySpawnInfo> spawnInfoList;

    [Header("보스 설정")]
    public bool isBossWave;
    public EnemyData bossEnemy;
}