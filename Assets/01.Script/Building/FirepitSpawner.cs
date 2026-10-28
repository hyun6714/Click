using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class FirepitSpawner : MonoBehaviour
{
    [Header("월드 좌표 변환용 타일맵")]
    public Tilemap buildingTilemap;

    private Dictionary<Vector3Int, GameObject> spawnedFirepits = new Dictionary<Vector3Int, GameObject>();

    private void OnEnable()
    {
        EventManager.OnRequestSpawnFirepit += HandleSpawnFirepit;
        EventManager.OnBuildingUnregistered += HandleBuildingUnregistered;
    }

    private void OnDisable()
    {
        EventManager.OnRequestSpawnFirepit -= HandleSpawnFirepit;
        EventManager.OnBuildingUnregistered -= HandleBuildingUnregistered;
    }

    private void Start()
    {
        EventManager.TriggerRequestSyncBuildings();
    }

    private void HandleSpawnFirepit(Vector3Int cellPos, FirepitData firepitData)
    {
        if (spawnedFirepits.ContainsKey(cellPos) && spawnedFirepits[cellPos] != null)
        {
            return;
        }
        if (firepitData.firepitPrefab == null) 
        {
            Debug.LogWarning("[화로 스폰 실패] 화로 프리팹이 지정되지 않았습니다.");
            return;
        }

        Vector3 worldPos = buildingTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);

        GameObject spawnedObj = Instantiate(firepitData.firepitPrefab, worldPos, Quaternion.identity);

        spawnedFirepits[cellPos] = spawnedObj;

        Firepit controller = spawnedObj.GetComponent<Firepit>();
        if (controller != null)
        {
            controller.Initialize(firepitData, cellPos);
            Debug.Log($"[화로 생성 완료] 위치: {cellPos}");
        }
    }

    private void HandleBuildingUnregistered(Vector3Int cellPos)
    {
        if (spawnedFirepits.ContainsKey(cellPos))
        {
            if (spawnedFirepits[cellPos] != null)
            {
                Destroy(spawnedFirepits[cellPos]);
            }
            spawnedFirepits.Remove(cellPos);
        }
    }
}