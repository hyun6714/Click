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
        if (prodData.productionPrefab == null)
        {
            return;
        }

        if (spawnedResourceBuildings.ContainsKey(cellPos) && spawnedResourceBuildings[cellPos] != null)
        {
            return;
        }

        // 월드 좌표로 변환
        Vector3 worldPos = buildingTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);

        // 생성
        GameObject spawnedObj = Instantiate(prodData.productionPrefab, worldPos, Quaternion.identity);

        // 데이터 주입
        ResourceBuilding controller = spawnedObj.GetComponent<ResourceBuilding>();
        if (controller != null)
        {
            controller.Initialize(prodData, cellPos, treeTilemap, stoneTilemap);
        }
    }

    private void HandleBuildingUnregistered(Vector3Int cellPos)
    {
        if (spawnedResourceBuildings.ContainsKey(cellPos))
        {
            if (spawnedResourceBuildings[cellPos] != null)
            {
                Destroy(spawnedResourceBuildings[cellPos]);
            }
            spawnedResourceBuildings.Remove(cellPos);
        }
    }
}