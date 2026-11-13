using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class AttackBuildingSpawner : MonoBehaviour
{
    [Header("¿ùµå ÁÂÇ¥ º¯È¯¿ë Å¸ÀÏ¸Ê")]
    public Tilemap buildingTilemap;

    private Dictionary<Vector3Int, GameObject> spawnedTowers = new Dictionary<Vector3Int, GameObject>();

    private void OnEnable()
    {
        EventManager.OnRequestSpawnTower += HandleSpawnTower;
        EventManager.OnBuildingUnregistered += HandleBuildingUnregistered;
        EventManager.OnBuildingManagerReady += HandleBuildingManagerReady;
    }

    private void OnDisable()
    {
        EventManager.OnRequestSpawnTower -= HandleSpawnTower;
        EventManager.OnBuildingUnregistered -= HandleBuildingUnregistered;
        EventManager.OnBuildingManagerReady -= HandleBuildingManagerReady;
    }

    private void HandleBuildingManagerReady(Tilemap tilemap)
    {
        buildingTilemap = tilemap;
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

        Vector3 worldPos = buildingTilemap.GetCellCenterWorld(cellPos);

        GameObject spawnedObj = Instantiate(attackData.towerPrefab, worldPos, Quaternion.identity);

        spawnedTowers[cellPos] = spawnedObj;

        AttackBuilding controller = spawnedObj.GetComponent<AttackBuilding>();
        if (controller != null)
        {
            controller.Initialize(attackData, cellPos);
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