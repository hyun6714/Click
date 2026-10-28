using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceBuilding : MonoBehaviour, IDamageable
{
    private ProductionBuildingData buildingData;
    private Vector3Int cellPosition;
    private Tilemap treeTilemap;
    private Tilemap stoneTilemap;

    private int currentHealth;

    private WeatherType currentWeather = WeatherType.Sunny;

    public ProductionBuildingData BuildingData => buildingData;
    public int CurrentHealth => currentHealth;
    public Vector3Int CellPosition => cellPosition;

    public void Initialize(ProductionBuildingData data, Vector3Int pos, Tilemap treeMap, Tilemap stoneMap)
    {
        buildingData = data;
        cellPosition = pos;
        currentHealth = buildingData.maxBuildingHp;

        treeTilemap = treeMap;
        stoneTilemap = stoneMap;

        // 채집 루프 시작
        StartCoroutine(GatherRoutine());
    
    }
    private void OnEnable()
    {
        EventManager.OnWeatherChanged += HandleWeatherChanged;
    }

    private void OnDisable()
    {
        EventManager.OnWeatherChanged -= HandleWeatherChanged;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"[채집 건물 피격] {buildingData.buildingName} 남은 체력: {currentHealth} / {buildingData.maxBuildingHp}");

        if (currentHealth <= 0)
        {
            DestroyBuilding();
        }
    }

    private IEnumerator GatherRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(buildingData.gatherInterval);

        while (true)
        {
            yield return waitTime;

            if (buildingData == null)
            {
                yield break;
            }

            TryGatherResource();
        }
    }

    private void TryGatherResource()
    {
        Debug.Log($"[채집 시도] 건물 이름: {buildingData?.buildingName}, 타겟 자원 타입: {buildingData?.targetResourceType}");

        int range = buildingData.gatherRange;
        Tilemap targetTilemap = null;
        CurrencyType rewardType = CurrencyType.Tree;

        if (buildingData.targetResourceType == TargetResourceType.Tree)
        {
            targetTilemap = treeTilemap;
            rewardType = CurrencyType.Tree;
        }

        else if (buildingData.targetResourceType == TargetResourceType.Rock)
        {
            targetTilemap = stoneTilemap;
            rewardType = CurrencyType.Rock;
        }

        if (targetTilemap == null)
        {
            Debug.LogWarning("채집할 타일맵이 없음");
            return;
        }

        Vector3 buildingCenterWorld = targetTilemap.GetCellCenterWorld(cellPosition);
        int searchRange = Mathf.CeilToInt(range * 2f);

        for (int x = -searchRange; x <= searchRange; x++)
        {
            for (int y = -searchRange; y <= searchRange; y++)
            {
                Vector3Int targetPos = cellPosition + new Vector3Int(x, y, 0);

                if (targetTilemap.HasTile(targetPos))
                {
                    Vector3 targetCenterWorld = targetTilemap.GetCellCenterWorld(targetPos);

                    float dx = targetCenterWorld.x - buildingCenterWorld.x;
                    float dy = targetCenterWorld.y - buildingCenterWorld.y;

                    float ellipticalValue = (dx * dx) / (range * range) + (dy * dy) / ((range * 0.5f) * (range * 0.5f));

                    if (ellipticalValue <= 1.0f)
                    {
                        targetTilemap.SetTile(targetPos, null);

                        int rewardAmount = (currentWeather == WeatherType.Rain) ? 2 : 1;

                        EventManager.CurrencyAdded(rewardType, 1);
                        Debug.Log($"[{buildingData.buildingName}] 채집 성공 좌표: {targetPos} (+1 {rewardType})");

                        return;
                    }
                }
            }
        }
    }

    private void DestroyBuilding()
    {
        Debug.Log($"[채집 건물 파괴] {buildingData.buildingName} 파괴 및 타일 제거");

        EventManager.TriggerRequestDemolish(cellPosition);

        Destroy(gameObject);
    }

    private void HandleWeatherChanged(WeatherType newWeather)
    {
        currentWeather = newWeather;
    }
}