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

    private void HandleSpawnFirepit(Vector3Int cellPos, FirepitData firepitData)
    {
        if (spawnedFirepits.ContainsKey(cellPos) && spawnedFirepits[cellPos] != null)
        {
            return;
        }
        if (firepitData.firepitPrefab == null) 
        {
            return;
        }

        Vector3 worldPos = buildingTilemap.GetCellCenterWorld(cellPos);

        GameObject spawnedObj = Instantiate(firepitData.firepitPrefab, worldPos, Quaternion.identity);

        spawnedFirepits[cellPos] = spawnedObj;

        Firepit controller = spawnedObj.GetComponent<Firepit>();
        if (controller != null)
        {
            controller.Initialize(firepitData, cellPos);
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