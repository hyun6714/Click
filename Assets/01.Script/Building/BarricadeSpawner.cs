using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BarricadeSpawner : MonoBehaviour
{
    [Header("¿ùµå ÁÂÇ¥ º¯È¯¿ë Å¸ÀÏ¸Ê")]
    public Tilemap buildingTilemap;

    private Dictionary<Vector3Int, GameObject> spawnedBarricades = new Dictionary<Vector3Int, GameObject>();

    private void OnEnable()
    {
        EventManager.OnRequestSpawnBuilding += HandleSpawnBuilding;
        EventManager.OnBuildingUnregistered += HandleBuildingUnregistered;
        EventManager.OnBuildingManagerReady += HandleBuildingManagerReady;
    }

    private void OnDisable()
    {
        EventManager.OnRequestSpawnBuilding -= HandleSpawnBuilding;
        EventManager.OnBuildingUnregistered -= HandleBuildingUnregistered;
        EventManager.OnBuildingManagerReady -= HandleBuildingManagerReady;
    }

    private void HandleBuildingManagerReady(Tilemap tilemap)
    {
        buildingTilemap = tilemap;
    }

    public void HandleSpawnBuilding(Vector3Int cellPos, BuildingData buildingData)
    {
        if (spawnedBarricades.ContainsKey(cellPos) && spawnedBarricades[cellPos] != null)
        {
            return;
        }

        if (buildingData == null)
        {
            return;
        }

        if (buildingData is BarricadeBuildingData barricadeData)
        {
            GameObject targetPrefab = barricadeData.barriPrefab;

            if (targetPrefab == null)
            {
                return;
            }

            Vector3 worldPos = buildingTilemap.GetCellCenterWorld(cellPos);
            GameObject spawnedObj = Instantiate(targetPrefab, worldPos, Quaternion.identity);
            spawnedBarricades[cellPos] = spawnedObj;

            BarricadeBuilding barricadeBuilding = spawnedObj.GetComponent<BarricadeBuilding>();
            if (barricadeBuilding != null)
            {
                barricadeBuilding.Initialize(barricadeData, cellPos);
            }
        }
    }

    private void HandleBuildingUnregistered(Vector3Int cellPos)
    {
        if (spawnedBarricades.ContainsKey(cellPos))
        {
            if (spawnedBarricades[cellPos] != null)
            {
                Destroy(spawnedBarricades[cellPos]);
            }
            spawnedBarricades.Remove(cellPos);
        }
    }
}