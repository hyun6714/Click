using UnityEngine;
using UnityEngine.Tilemaps;

public class MainCastleSpawner : MonoBehaviour
{
    [Header("월드 좌표 변환용 타일맵")]
    public Tilemap buildingTilemap;

    [Header("메인 성 프리팹 연결")]
    public GameObject mainCastlePrefab;

    private void OnEnable()
    {
        EventManager.OnRequestPlaceBuilding += HandleSpawnMainCastle;
    }

    private void OnDisable()
    {
        EventManager.OnRequestPlaceBuilding -= HandleSpawnMainCastle;
    }

    private void HandleSpawnMainCastle(Vector3Int cellPos, BuildingData data)
    {
        if (data == null || data.category != BuildingCategory.Main)
        {
            return;
        }

        if (mainCastlePrefab == null)
        {
            return;
        }

        Vector3 worldPos = buildingTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);

        GameObject spawnedObj = Instantiate(mainCastlePrefab, worldPos, Quaternion.identity);

        MainCastle controller = spawnedObj.GetComponent<MainCastle>();
        if (controller != null)
        {
            controller.Initialize(data, cellPos);
            Debug.Log($"[메인 성 생성 완료] 위치: {cellPos}");
        }
    }
}