using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceBuilding : MonoBehaviour, IDamageable
{
    [Header("FX용 참조")]
    [SerializeField] private ResourceGainPopup gainPopupPrefab;
    [SerializeField] private Sprite treeIcon;
    [SerializeField] private Sprite rockIcon;

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

        if (EventManager.OnRequestCurrentWeather != null)
        {
            currentWeather = EventManager.OnRequestCurrentWeather.Invoke();
        }

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
        if (damage <= 0)
        {
            return;
        }

        EventManager.TriggerRequestDamageBuilding(cellPosition, damage);
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
                        WeatherType liveWeather = currentWeather;
                        if (EventManager.OnRequestCurrentWeather != null)
                        {
                            liveWeather = EventManager.OnRequestCurrentWeather.Invoke();
                        }

                        int rewardAmount = (liveWeather == WeatherType.Rain) ? 2 : 1;

                        Sprite icon = rewardType == CurrencyType.Tree ? treeIcon : rockIcon;

                        if (gainPopupPrefab != null)
                        {
                            Vector3 popupPosition = targetCenterWorld;

                            ResourceGainPopup popup = Instantiate( gainPopupPrefab, popupPosition, Quaternion.identity);

                            popup.Initialize(rewardAmount, icon);
                        }

                        targetTilemap.SetTile(targetPos, null);
                        EventManager.CurrencyAdded(rewardType, rewardAmount);
                        EventManager.PlaySound(SoundCue.ResourceBuildingGather);

                        Debug.Log($"[{buildingData.buildingName}] 채집 성공 좌표: {targetPos} ({rewardType})");
                        return;
                    }
                }
            }
        }
    }

    private void HandleWeatherChanged(WeatherType newWeather)
    {
        currentWeather = newWeather;
    }
}