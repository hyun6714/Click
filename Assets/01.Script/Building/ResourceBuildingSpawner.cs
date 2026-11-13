using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceBuildingSpawner : MonoBehaviour
{
    [Header("월드 좌표 변환용 타일맵")]
    public Tilemap buildingTilemap;

    [Header("자원타일맵 참조")]
    public Tilemap treeTilemap;
    public Tilemap stoneTilemap;

    private Dictionary<Vector3Int, GameObject> spawnedResourceBuildings = new Dictionary<Vector3Int, GameObject>();

    private void OnEnable()
    {
        EventManager.OnRequestSpawnResourceBuilding += HandleSpawnResourceBuilding;
        EventManager.OnBuildingUnregistered += HandleBuildingUnregistered;
    }

    private void OnDisable()
    {
        EventManager.OnRequestSpawnResourceBuilding -= HandleSpawnResourceBuilding;
        EventManager.OnBuildingUnregistered -= HandleBuildingUnregistered;
    }


    private void HandleSpawnResourceBuilding(Vector3Int cellPos, ProductionBuildingData prodData)
    {
        if (prodData == null)
        {
            return;
        }
        if (prodData.productionPrefab == null)
        {
            return;
        }
        if (spawnedResourceBuildings.TryGetValue(cellPos, out GameObject existingObj)&& existingObj != null)
        {
            return;
        }
        spawnedResourceBuildings.Remove(cellPos);

        if (buildingTilemap == null)
        {
            return;
        }

        Vector3 worldPos =
            buildingTilemap.GetCellCenterWorld(cellPos);

        GameObject spawnedObj = Instantiate(
            prodData.productionPrefab,
            worldPos,
            Quaternion.identity
        );

        ResourceBuilding controller = spawnedObj.GetComponent<ResourceBuilding>();

        if (controller == null)
        {
            Destroy(spawnedObj);
            return;
        }

        controller.Initialize(
            prodData,
            cellPos,
            treeTilemap,
            stoneTilemap
        );

        spawnedResourceBuildings[cellPos] = spawnedObj;
    }

    private void HandleBuildingUnregistered(Vector3Int cellPos)
    {
        if (!spawnedResourceBuildings.TryGetValue(cellPos, out GameObject spawnedObj))
        {
            return;
        }

        spawnedResourceBuildings.Remove(cellPos);

        if (spawnedObj != null)
        {
            Destroy(spawnedObj);
        }
    }
}