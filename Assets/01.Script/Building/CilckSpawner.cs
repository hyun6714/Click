using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CilckSpawner : MonoBehaviour
{
    [Header("월드 좌표 변환용 타일맵")]
    public Tilemap buildingTilemap;

    private Dictionary<Vector3Int, GameObject> spawnedBuildings = new Dictionary<Vector3Int, GameObject>();

    private void OnEnable()
    {
        EventManager.OnRequestSpawnBuilding += HandleSpawnBuilding;
        EventManager.OnBuildingUnregistered += HandleBuildingUnregistered;
        EventManager.OnBuildingManagerReady += HandleBuildingManagerReady;
    }

    private void OnDisable()
    {
        EventManager.OnRequestSpawnBuilding += HandleSpawnBuilding;
        EventManager.OnBuildingUnregistered -= HandleBuildingUnregistered;
        EventManager.OnBuildingManagerReady -= HandleBuildingManagerReady;
    }

    private void HandleBuildingManagerReady(Tilemap tilemap)
    {
        buildingTilemap = tilemap;
    }

    public void HandleSpawnBuilding(Vector3Int cellPos, BuildingData buildingData)
    {
        if (spawnedBuildings.ContainsKey(cellPos) && spawnedBuildings[cellPos] != null)
        {
            return;
        }

        if (buildingData == null)
        {
            return;
        }

        GameObject targetPrefab = null;
        Vector3 worldPos = buildingTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);

        if (buildingData is ClickBuildingData clickData)
        {
            targetPrefab = clickData.clickMaxPrefab;

            if (targetPrefab == null)
            {
                return;
            }

            GameObject spawnedObj = Instantiate(targetPrefab, worldPos, Quaternion.identity);
            spawnedBuildings[cellPos] = spawnedObj;

            ClickBuilding clickBuilding = spawnedObj.GetComponent<ClickBuilding>();
            if (clickBuilding != null)
            {
                clickBuilding.Initialize(clickData, cellPos);
                Debug.Log($"[클릭 건물 생성 완료] 위치: {cellPos}");
            }
        }
        else if (buildingData is ProductionBoostBuildingData boostData)
        {
            targetPrefab = boostData.ClickBoPrefab;

            if (targetPrefab == null)
            {
                return;
            }

            GameObject spawnedObj = Instantiate(targetPrefab, worldPos, Quaternion.identity);
            spawnedBuildings[cellPos] = spawnedObj;

            ProductionBoostBuilding boostBuilding = spawnedObj.GetComponent<ProductionBoostBuilding>();
            if (boostBuilding != null)
            {
                boostBuilding.Initialize(boostData, cellPos);
                Debug.Log($"[생산 보너스 건물 생성 완료] 위치: {cellPos}");
            }
        }
    }

    private void HandleBuildingUnregistered(Vector3Int cellPos)
    {
        if (spawnedBuildings.ContainsKey(cellPos))
        {
            if (spawnedBuildings[cellPos] != null)
            {
                Destroy(spawnedBuildings[cellPos]);
            }
            spawnedBuildings.Remove(cellPos);
        }
    }
}