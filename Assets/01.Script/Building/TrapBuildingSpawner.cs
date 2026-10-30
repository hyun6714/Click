using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrapBuildingSpawner : MonoBehaviour
{
    [Header("월드 좌표 변환용 타일맵")]
    public Tilemap buildingTilemap;

    private Dictionary<Vector3Int, GameObject> spawnedTraps = new Dictionary<Vector3Int, GameObject>();

    private void OnEnable()
    {
        EventManager.OnRequestSpawnTrapBuilding += HandleSpawnBuilding;
        EventManager.OnBuildingUnregistered += HandleBuildingUnregistered;
        EventManager.OnBuildingManagerReady += HandleBuildingManagerReady;
    }

    private void OnDisable()
    {
        EventManager.OnRequestSpawnTrapBuilding -= HandleSpawnBuilding;
        EventManager.OnBuildingUnregistered -= HandleBuildingUnregistered;
        EventManager.OnBuildingManagerReady -= HandleBuildingManagerReady;
    }

    private void HandleBuildingManagerReady(Tilemap tilemap)
    {
        buildingTilemap = tilemap;
    }

    public void HandleSpawnBuilding(Vector3Int cellPos, TrapBuildingData trapData)
    {
        if (spawnedTraps.ContainsKey(cellPos) && spawnedTraps[cellPos] != null)
        {
            return;
        }

        if (trapData == null)
        {
            return;
        }

        GameObject targetPrefab = trapData.trapPrefab;

        if (targetPrefab == null)
        {
            Debug.LogWarning($"[TrapBuildingSpawner] {trapData.buildingName} 프리팹이 연결되지 않음");
            return;
        }

        Vector3 worldPos = buildingTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);
        GameObject spawnedObj = Instantiate(targetPrefab, worldPos, Quaternion.identity);
        spawnedTraps[cellPos] = spawnedObj;

        TrapBuilding trapBuilding = spawnedObj.GetComponent<TrapBuilding>();
        if (trapBuilding != null)
        {
            trapBuilding.Initialize(trapData, cellPos);
        }

        BombBuilding bombBuilding = spawnedObj.GetComponent<BombBuilding>();
        if (bombBuilding != null)
        {
            bombBuilding.Initialize(trapData, cellPos);
        }

        Debug.Log($"[함정/폭탄 생성 완료] 위치: {cellPos}");
    }

    private void HandleBuildingUnregistered(Vector3Int cellPos)
    {
        if (spawnedTraps.ContainsKey(cellPos))
        {
            if (spawnedTraps[cellPos] != null)
            {
                Destroy(spawnedTraps[cellPos]);
            }
            spawnedTraps.Remove(cellPos);
        }
    }
}