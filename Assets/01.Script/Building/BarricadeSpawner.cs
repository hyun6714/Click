using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BarricadeSpawner : MonoBehaviour
{
    [Header("월드 좌표 변환용 타일맵")]
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
                Debug.LogWarning($"[BarricadeSpawner] {barricadeData.barriPrefab} 프리팹이 연결되지 않음");
                return;
            }

            Vector3 worldPos = buildingTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);
            GameObject spawnedObj = Instantiate(targetPrefab, worldPos, Quaternion.identity);
            spawnedBarricades[cellPos] = spawnedObj;

            BarricadeBuilding barricadeBuilding = spawnedObj.GetComponent<BarricadeBuilding>();
            if (barricadeBuilding != null)
            {
                barricadeBuilding.Initialize(barricadeData, cellPos);
                Debug.Log($"[바리케이트 생성 완료] 위치: {cellPos}");
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