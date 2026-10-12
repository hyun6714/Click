using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceBuilding : MonoBehaviour
{
    private ProductionBuildingData buildingData;
    private Vector3Int cellPosition;
    private Tilemap treeTilemap;
    private Tilemap stoneTilemap;

    public void Initialize(ProductionBuildingData data, Vector3Int pos, Tilemap treeMap, Tilemap stoneMap)
    {
        buildingData = data;
        cellPosition = pos;

        treeTilemap = treeMap;
        stoneTilemap = stoneMap;

        // 채집 루프 시작
        StartCoroutine(GatherRoutine());
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
            Debug.LogWarning("채집할 타일맵을 찾지 못했습니다!");
            return;
        }

        // 주변 범위(range) 안을 탐색
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int targetPos = cellPosition + new Vector3Int(x, y, 0);

                // 해당 위치에 자원 타일이 존재하면 파괴하고 자원 획득
                if (targetTilemap.HasTile(targetPos))
                {
                    targetTilemap.SetTile(targetPos, null);

                    EventManager.CurrencyAdded(rewardType, 1);
                    Debug.Log($"[{buildingData.buildingName}] 채집 성공! 좌표: {targetPos} (+1 {rewardType})");

                    return;
                }
            }
        }
    }
}