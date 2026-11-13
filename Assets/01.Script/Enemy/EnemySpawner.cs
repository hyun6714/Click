using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("참조 설정")]
    [SerializeField] private Transform mainCastleTarget; // 최종 목적지인 메인 성
    [SerializeField] private EnemyObjectPool enemyPool;  // 오브젝트 풀 매니저 참조

    [Header("웨이브 및 스폰 설정")]
    [SerializeField] private List<WaveData> waveDataList; // 웨이브 목록
    [SerializeField] private Transform[] spawnPoints;

    private int remainingEnemyCount = 0;
    private WaveData currentActiveWave;
    private bool isWaveInProgress = false;

    [SerializeField] private BuildingManager buildingManager;

    private void Awake()
    {
        ForceResetWaveState();
    }

    private void OnEnable()
    {
        EventManager.OnDayChanged -= HandleDayChanged;
        EventManager.OnDayChanged += HandleDayChanged;

        EventManager.OnRequestReturnPool -= HandleEnemyReturnPool;
        EventManager.OnRequestReturnPool += HandleEnemyReturnPool;

        ForceResetWaveState();
    }

    private void OnDisable()
    {
        EventManager.OnDayChanged -= HandleDayChanged;
        EventManager.OnRequestReturnPool -= HandleEnemyReturnPool;
    }

    private void HandleDayChanged(int day)
    {
        if (waveDataList == null)
        {
            return;
        }

        WaveData matchingWave = waveDataList.Find(wave => wave != null && wave.targetDay == day);
        if (matchingWave == null)
        {
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        StartCoroutine(SpawnSpecificWaveRoutine(matchingWave));
    }

    private IEnumerator SpawnSpecificWaveRoutine(WaveData wave)
    {
        if (isWaveInProgress)
        {
            yield break;
        }

        if (buildingManager == null)
        {
            yield break;
        }

        isWaveInProgress = true;
        currentActiveWave = wave;
        remainingEnemyCount = 0;

        EventManager.TriggerWaveStarted(wave);

        while (!buildingManager.IsInitialized)
        {
            yield return null;
        }

        buildingManager.BeginWaveBuildingCheckpoint();

        if (wave.spawnInfoList != null)
        {
            foreach (var spawnInfo in wave.spawnInfoList)
            {
                if (spawnInfo.enemyData == null || spawnInfo.spawnCount <= 0)
                {
                    continue;
                }

                for (int i = 0; i < spawnInfo.spawnCount; i++)
                {
                    int index = Random.Range(0, spawnPoints.Length);
                    Vector3 spawnPos = spawnPoints[index].position;

                    GameObject enemyObj = enemyPool.GetEnemy(
                        spawnInfo.enemyData, spawnPos, mainCastleTarget
                    );

                    if (enemyObj != null &&
                        enemyObj.TryGetComponent<EnemyBase>(out var enemy))
                    {
                        enemy.Initialize(spawnInfo.enemyData, spawnPos, mainCastleTarget);
                        remainingEnemyCount++;
                    }

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        if (wave.isBossWave && wave.bossEnemy != null)
        {
            yield return new WaitForSeconds(1.5f);

            int index = Random.Range(0, spawnPoints.Length);
            Vector3 spawnPos = spawnPoints[index].position;
            GameObject bossObj = enemyPool.GetEnemy(wave.bossEnemy, spawnPos, mainCastleTarget);

            if (bossObj != null)
            {
                if (bossObj.TryGetComponent<BossEnemy>(out var boss))
                {
                    boss.InitializeBoss(wave.bossEnemy, spawnPos, mainCastleTarget);
                    remainingEnemyCount++;
                }
                else if (bossObj.TryGetComponent<EnemyBase>(out var enemy))
                {
                    enemy.Initialize(wave.bossEnemy, spawnPos, mainCastleTarget);
                    remainingEnemyCount++;
                }
            }
        }

        // 스폰된 적이 없으면 웨이브를 즉시 완료
        if (remainingEnemyCount == 0)
        { 
            CompleteCurrentWave();
        }
    }

    private void HandleEnemyReturnPool(EnemyType enemyType, GameObject enemyObj)
    {
        if (!isWaveInProgress)
        {
            return;
        }

        remainingEnemyCount--;

        if (remainingEnemyCount <= 0)
        {
            CompleteCurrentWave();
        }
    }

    private void CompleteCurrentWave()
    {
        isWaveInProgress = false;
        EventManager.TriggerWaveEnded(currentActiveWave);

        if (currentActiveWave.rewardGold > 0)
        {
            EventManager.CurrencyAdded(CurrencyType.Gold, currentActiveWave.rewardGold);
        }

        EventManager.OnWaveCleared?.Invoke(currentActiveWave);
        buildingManager.EndWaveBuildingCheckpoint();

        if (currentActiveWave.isClearWave)
        {
            EventManager.TriggerGameCleared();
        }
    }

    public void SetMainCastleTarget(Transform castleTransform)
    {
        mainCastleTarget = castleTransform;
    }


    //패배 후 or 재시작 시 초기화 함수
    public void ForceResetWaveState()
    {
        StopAllCoroutines();
        isWaveInProgress = false;
        remainingEnemyCount = 0;
        currentActiveWave = null;
    }

    public void CheckAndResumeWaveOnLoad(int currentDay)
    {
        if (isWaveInProgress)
        {
            return;
        }

        if (waveDataList == null)
        {
            return;
        }

        WaveData matchingWave = waveDataList.Find(wave => wave != null && wave.targetDay == currentDay);
        if (matchingWave != null)
        {
            StartCoroutine(SpawnSpecificWaveRoutine(matchingWave));
        }
    }
}