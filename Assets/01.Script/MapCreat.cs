using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapCreat : MonoBehaviour
{
    [Header("타일맵 연결")]
    public Tilemap groundTilemap;// 바닥 타일맵 연결
    public Tilemap stoneTilemap; // 돌 타일맵 연결
    public Tilemap treeTilemap; // 나무 타일맵 연결
    public Tilemap buildingTilemap;
    public Tile transparentBlockingTile;

    [Header("사용할 타일 asset")]
    public TileBase groundTile; // 사용할 잔디 바닥 타일
    public TileBase stoneTile; // 사용할 돌 타일
    public TileBase treeTile;
    public TileBase mainBuildingTile;


    [Header("메인 성 설정")]
    public Vector3Int mainBuildingCellPos = Vector3Int.zero;

    [Header("건물 차단 영역 오프셋 (비율 조절)")]
    public int rangeLeft = 2;
    public int rangeRight = 2; 
    public int rangeUp = 2; 
    public int rangeDown = 4;

    [Header("맵 크기 (중앙 0,0 기준)")]
    public int width = 150; // 맵 가로 크기
    public int height = 150; // 맵 세로 크기

    [Header("중앙 비우기")]
    public int centerSafeRadius = 10;

    [Header("나무 생성 여백")]
    public int treeMargin = 2;

    [Header("돌 무리")]
    public int stoneClusterCount = 30;
    public int stoneClearance = 1;

    [Range(0f, 1f)]
    public float treeDensity = 0.0f;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // 기존에 그려진 것 초기화
        groundTilemap.ClearAllTiles();
        stoneTilemap.ClearAllTiles();
        treeTilemap.ClearAllTiles();
        if (buildingTilemap != null) buildingTilemap.ClearAllTiles();

        int halfW = width / 2;
        int halfH = height / 2;

        //바닥 깔기
        int totalGroundCount = (width + 1) * (height + 1);
        Vector3Int[] groundPositions = new Vector3Int[totalGroundCount];
        TileBase[] groundTiles = new TileBase[totalGroundCount];
        int gIndex = 0;

        for (int x = -halfW; x <= halfW; x++)
        {
            for (int y = -halfH; y <= halfH; y++)
            {
                groundPositions[gIndex] = new Vector3Int(x, y, 0);
                groundTiles[gIndex] = groundTile;
                gIndex++;
            }
        }
        groundTilemap.SetTiles(groundPositions, groundTiles);


        HashSet<Vector3Int> forbiddenForTrees = new HashSet<Vector3Int>();

        if (buildingTilemap != null && mainBuildingTile != null)
        {
            buildingTilemap.SetTile(mainBuildingCellPos, mainBuildingTile);
        }

        if (buildingTilemap != null)
        {
            for (int dx = -rangeLeft; dx <= rangeRight; dx++)
            {
                for (int dy = -rangeDown; dy <= rangeUp; dy++)
                {
                    Vector3Int targetPos = new Vector3Int(mainBuildingCellPos.x + dx, mainBuildingCellPos.y + dy, 0);

                    forbiddenForTrees.Add(targetPos);

                    if (dx == 0 && dy == 0)
                    {
                        if (mainBuildingTile != null)
                        {
                            buildingTilemap.SetTile(targetPos, mainBuildingTile);
                        }
                    }
                    else
                    {
                        if (transparentBlockingTile != null)
                        {
                            buildingTilemap.SetTile(targetPos, transparentBlockingTile);
                        }
                    }
                }
            }
        }

        //돌 심기
        HashSet<Vector3Int> stonePositions = new HashSet<Vector3Int>(); //돌 설치 구간 저장
        int spawnedClusters = 0;
        int safetyCount = 0;

        while (spawnedClusters < stoneClusterCount && safetyCount < 1000)
        {
            safetyCount++;

            int cx = Random.Range(-halfW + 10, halfW - 10);
            int cy = Random.Range(-halfH + 10, halfH - 10);

            if (Mathf.Abs(cx) <= centerSafeRadius + 10 && Mathf.Abs(cy) <= centerSafeRadius + 10)
            {
                continue;
            }

            bool tooClose = false;
            foreach (var pos in stonePositions)
            {
                if (Mathf.Abs(pos.x - cx) < 10 && Mathf.Abs(pos.y - cy) < 10)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose && safetyCount < 500)
            {
                continue;
            }

            int clusterSize = Random.Range(3, 5);
            bool clusterCreated = false;

            for (int j = 0; j < clusterSize; j++)
            {
                int offsetX = Random.Range(-2, 3);
                int offsetY = Random.Range(-2, 3);
                int nx = cx + offsetX;
                int ny = cy + offsetY;

                if (nx >= -halfW + 5 && nx <= halfW - 5 && ny >= -halfH + 5 && ny <= halfH - 5)
                {
                    if (Mathf.Abs(nx) <= centerSafeRadius && Mathf.Abs(ny) <= centerSafeRadius)
                    {
                        continue;
                    }

                    Vector3Int stonePos = new Vector3Int(nx, ny, 0);
                    stonePositions.Add(stonePos);

                    for (int dx = -stoneClearance; dx <= stoneClearance; dx++)
                    {
                        for (int dy = -stoneClearance; dy <= stoneClearance; dy++)
                        {
                            forbiddenForTrees.Add(new Vector3Int(stonePos.x + dx, stonePos.y + dy, 0));
                        }
                    }

                    clusterCreated = true;
                }
            }

            if (clusterCreated)
            {
                spawnedClusters++;
            }
        }

        // 실제 돌 타일맵에 한 번에 반영
        foreach (var pos in stonePositions)
        {
            stoneTilemap.SetTile(pos, stoneTile);
        }


        //나무 및 최종 타일 배치
        List<Vector3Int> treePositionsList = new List<Vector3Int>();
        List<TileBase> treeTilesList = new List<TileBase>();

        for (int x = -halfW + treeMargin; x <= halfW - treeMargin; x++)
        {
            for (int y = -halfH + treeMargin; y <= halfH - treeMargin; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                if (Mathf.Abs(x) <= centerSafeRadius && Mathf.Abs(y) <= centerSafeRadius)
                {
                    continue;
                }

                if (forbiddenForTrees.Contains(cellPos))
                {
                    continue;
                }

                if (Random.value < treeDensity)
                {
                    treePositionsList.Add(cellPos);
                    treeTilesList.Add(treeTile);
                }
            }
        }

        // 나무 타일도 일괄 적용
        if (treePositionsList.Count > 0)
        {
            treeTilemap.SetTiles(treePositionsList.ToArray(), treeTilesList.ToArray());
        }
    }
        
    
}
