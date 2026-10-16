using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [Header("참조 설정")]
    [SerializeField] private Transform mainCastleTarget; // 최종 목적지인 메인 성
    [SerializeField] private EnemyObjectPool enemyPool;  // 오브젝트 풀 매니저 참조

    [Header("웨이브 및 스폰 설정")]
    [SerializeField] private List<WaveData> waveDataList; // 웨이브 목록
    [SerializeField] private Transform[] spawnPoints;

    private void OnEnable()
    {
        EventManager.OnDayChanged += HandleDayChanged;
    }

    private void OnDisable()
    {
        EventManager.OnDayChanged -= HandleDayChanged;
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
            if (bossObj.TryGetComponent<EnemyBase>(out var enemyScript))
            {
                enemyScript.Initialize(wave.bossEnemy, spawnPos, mainCastleTarget);
                Debug.Log($"[스포너] 보스 [{wave.bossEnemy.enemyName}] 출현");
            }
        }
    }

    public void SetMainCastleTarget(Transform castleTransform)
    {
        mainCastleTarget = castleTransform;
    }
}