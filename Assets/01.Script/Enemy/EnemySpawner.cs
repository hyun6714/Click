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

    private void OnEnable()
    {
        EventManager.OnDayChanged += HandleDayChanged;
        EventManager.OnRequestReturnPool += HandleEnemyReturnPool;
    }

    private void OnDisable()
    {
        EventManager.OnDayChanged -= HandleDayChanged;
        EventManager.OnRequestReturnPool -= HandleEnemyReturnPool;
    }

    private void HandleDayChanged(int day)
    {
        Debug.Log($"[스포너] {day}일차 도달 - 웨이브 체크");

        WaveData matchingWave = waveDataList.Find(wave => wave.targetDay == day);

        if (matchingWave != null)
        {
            StartCoroutine(SpawnSpecificWaveRoutine(matchingWave));
        }
        else
        {
            Debug.Log($"[스포너] {day}일차 Wave 없음");
        }
    }

    private IEnumerator SpawnSpecificWaveRoutine(WaveData wave)
    {
        isWaveInProgress = true;
        currentActiveWave = wave;

        int totalSpawnCount = 0;
        if (wave.spawnInfoList != null)
        {
            foreach (var info in wave.spawnInfoList)
            {
                totalSpawnCount += info.spawnCount;
            }
        }

        if (wave.isBossWave && wave.bossEnemy != null)
        {
            totalSpawnCount += 1;
        }

        remainingEnemyCount = totalSpawnCount;

        Debug.Log($"[스포너] {wave.targetDay}일차 웨이브 시작");

        //일반 
        if (wave.spawnInfoList != null)
        {
            foreach (var spawnInfo in wave.spawnInfoList)
            {
                if (spawnInfo.enemyData == null || spawnPoints.Length == 0)
                {
                    continue;
                }

                for (int i = 0; i < spawnInfo.spawnCount; i++)
                {
                    int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
                    Vector3 spawnPos = spawnPoints[randomSpawnIndex].position;

                    GameObject enemyObj = enemyPool.GetEnemy(spawnInfo.enemyData, spawnPos, mainCastleTarget);
                    if (enemyObj.TryGetComponent<EnemyBase>(out var enemyScript))
                    {
                        enemyScript.Initialize(spawnInfo.enemyData, spawnPos, mainCastleTarget);
                    }

                    yield return new WaitForSeconds(1f);
                }
            }
        }

        //보스
        if (wave.isBossWave && wave.bossEnemy != null)
        {
            yield return new WaitForSeconds(1.5f);

            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            Vector3 spawnPos = spawnPoints[randomSpawnIndex].position;

            GameObject bossObj = enemyPool.GetEnemy(wave.bossEnemy, spawnPos, mainCastleTarget);
            if (bossObj.TryGetComponent<BossEnemy>(out var bossScript))
            {
                bossScript.InitializeBoss(wave.bossEnemy, spawnPos, mainCastleTarget);
                Debug.Log($"[스포너] 보스 [{wave.bossEnemy.enemyName}] 출현 (클리어 보스 여부: {wave.isClearWave})");
            }
            else if (bossObj.TryGetComponent<EnemyBase>(out var enemyScript))
            {
                enemyScript.Initialize(wave.bossEnemy, spawnPos, mainCastleTarget);
                Debug.Log($"[스포너] 보스 [{wave.bossEnemy.enemyName}] 출현");
            }
        }
    }

    private void HandleEnemyReturnPool(EnemyType enemyType, GameObject enemyObj)
    {
        if (!isWaveInProgress)
        {
            return;
        }

        remainingEnemyCount--;
        Debug.Log($"[스포너] 적 처치(풀 반환)! 남은 적: {remainingEnemyCount}마리");

        if (remainingEnemyCount <= 0)
        {
            CompleteCurrentWave();
        }
    }

    private void CompleteCurrentWave()
    {
        isWaveInProgress = false;
        Debug.Log($"[스포너] {currentActiveWave.targetDay}일차 웨이브 클리어 보상 골드: {currentActiveWave.rewardGold}");

        EventManager.OnWaveCleared?.Invoke(currentActiveWave);

        if (currentActiveWave.isClearWave)
        {
            Debug.Log("[스포너] 최종 클리어 웨이브 달성");
            EventManager.TriggerGameCleared();
        }
    }

    public void SetMainCastleTarget(Transform castleTransform)
    {
        mainCastleTarget = castleTransform;
    }
}