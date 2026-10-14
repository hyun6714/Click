using UnityEngine;
using UnityEngine.Tilemaps;

public class AttackBuildingSpawner : MonoBehaviour
{
    [Header("월드 좌표 변환용 타일맵")]
    public Tilemap buildingTilemap;

    private void OnEnable()
    {
        EventManager.OnRequestSpawnTower += HandleSpawnTower;
    }

    private void OnDisable()
    {
        EventManager.OnRequestSpawnTower -= HandleSpawnTower;
    }

    private void HandleSpawnTower(Vector3Int cellPos, AttackBuildingData attackData)
    {
        if (attackData.towerPrefab == null)
        {
            return;
        }

        Vector3 worldPos = buildingTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);

        GameObject spawnedObj = Instantiate(attackData.towerPrefab, worldPos, Quaternion.identity);

        AttackBuilding controller = spawnedObj.GetComponent<AttackBuilding>();
        if (controller != null)
        {
            controller.Initialize(attackData, cellPos);
            Debug.Log($"[공격 타워 생성 완료] 위치: {cellPos}");
        }
    }
}