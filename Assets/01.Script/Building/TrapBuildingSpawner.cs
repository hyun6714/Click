using UnityEngine;
using UnityEngine.Tilemaps;

public class TrapBuildingSpawner : MonoBehaviour
{
    [Header("¿ùµå ÁÂÇ¥ º¯È¯¿ë Å¸ÀÏ¸Ê")]
    public Tilemap buildingTilemap;

    private void OnEnable()
    {
        EventManager.OnRequestSpawnTrapBuilding += HandleSpawnTrapBuilding;
    }

    private void OnDisable()
    {
        EventManager.OnRequestSpawnTrapBuilding -= HandleSpawnTrapBuilding;
    }

    private void HandleSpawnTrapBuilding(Vector3Int cellPos, TrapBuildingData trapData)
    {
        if (trapData.trapPrefab == null)
        {
            return;
        }

        Vector3 worldPos = buildingTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);

        GameObject spawnedObj = Instantiate(trapData.trapPrefab, worldPos, Quaternion.identity);

        TrapBuilding controller = spawnedObj.GetComponent<TrapBuilding>();
        if (controller != null)
        {
            controller.Initialize(trapData, cellPos);
        }
    }
}