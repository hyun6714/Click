using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class AttackBuildingSpawner : MonoBehaviour
{
    [Header("월드 좌표 변환용 타일맵")]
    public Tilemap buildingTilemap;

    private Dictionary<Vector3Int, GameObject> spawnedTowers = new Dictionary<Vector3Int, GameObject>();

    private void OnEnable()
    {
        EventManager.OnRequestSpawnTower += HandleSpawnTower;
        EventManager.OnBuildingUnregistered += HandleBuildingUnregistered;
    }

    private void OnDisable()
    {
        EventManager.OnRequestSpawnTower -= HandleSpawnTower;
        EventManager.OnBuildingUnregistered -= HandleBuildingUnregistered;
    }

    private void Start()
    {
        EventManager.TriggerRequestSyncBuildings();
    }

    private void HandleSpawnTower(Vector3Int cellPos, AttackBuildingData attackData)
    {
        if (spawnedTowers.ContainsKey(cellPos) && spawnedTowers[cellPos] != null)
        {
            return;
        }
        if (attackData.towerPrefab == null)
        {
            return;
        }

        Vector3 worldPos = buildingTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);

        GameObject spawnedObj = Instantiate(attackData.towerPrefab, worldPos, Quaternion.identity);

        spawnedTowers[cellPos] = spawnedObj;

        AttackBuilding controller = spawnedObj.GetComponent<AttackBuilding>();
        if (controller != null)
        {
            controller.Initialize(attackData, cellPos);
            Debug.Log($"[공격 타워 생성 완료] 위치: {cellPos}");
        }
    }

    private void HandleBuildingUnregistered(Vector3Int cellPos)
    {
        if (spawnedTowers.ContainsKey(cellPos))
        {
            if (spawnedTowers[cellPos] != null)
            {
                Destroy(spawnedTowers[cellPos]);
            }
            spawnedTowers.Remove(cellPos);
        }
    }
}