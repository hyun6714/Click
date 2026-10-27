using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class ResourceGatherer : MonoBehaviour
{
    private GameMode currentMode = GameMode.Normal;

    [Header("Tilemaps")]
    public Tilemap referenceTilemap; // 기준이 되는 바닥 타일맵
    public Tilemap treeTilemap; // 나무 타일맵
    public Tilemap stoneTilemap; // 돌 타일맵
    public Tilemap fogTilemap; //안개 타일맵

    [Header("안개 설정")]
    [SerializeField] private int fogRevealRadius = 1; // 클릭 시 걷힐 범위
    [SerializeField] private int fogClickCost = 1; // 소모량

    [Header("채집 시 획득량")]
    [SerializeField] private int harvestAmount = 1;

    [Header("클릭 차감")]
    [SerializeField] private int clickCost = 1;

    private HashSet<Vector3Int> revealedFogPositions = new HashSet<Vector3Int>();

    private void OnEnable()
    {
        EventManager.OnGameModeChanged += HandleGameModeChanged;
    }

    private void OnDisable()
    {
        EventManager.OnGameModeChanged -= HandleGameModeChanged;
    }

    private void HandleGameModeChanged(GameMode newMode)
    {
        currentMode = newMode;
    }


    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (currentMode != GameMode.Normal)
            {
                return;
            }

            TryInteractWithWorld();
        }
    }

    private void TryInteractWithWorld()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector3Int cellPos = referenceTilemap.WorldToCell(mouseWorldPos);
        cellPos.z = 0;

        if (fogTilemap != null && fogTilemap.HasTile(cellPos))
        {
            TryClearFog(cellPos);
            return;
        }

        bool hasTree = treeTilemap.HasTile(cellPos);
        bool hasStone = stoneTilemap.HasTile(cellPos);

        if (!hasTree && !hasStone)
        {
            return;
        }

        bool canUseClick = EventManager.RequestUseCurrency(CurrencyType.Click, clickCost);

        if (!canUseClick)
        {
            Debug.Log("클릭 횟수 부족");
            return;
        }

        if (treeTilemap.HasTile(cellPos))
        {
            EventManager.CurrencyAdded(CurrencyType.Tree, harvestAmount);
            Debug.Log("나무 획득");

            treeTilemap.SetTile(cellPos, null);
            return;
        }

        if (stoneTilemap.HasTile(cellPos))
        {
            EventManager.CurrencyAdded(CurrencyType.Rock, harvestAmount);
            Debug.Log("돌 획득");

            stoneTilemap.SetTile(cellPos, null);
            return;
        }
    }

    private void TryClearFog(Vector3Int centerCell)
    {
        if (fogClickCost > 0)
        {
            bool canUseClick = EventManager.RequestUseCurrency(CurrencyType.Click, fogClickCost);
            if (!canUseClick)
            {
                Debug.Log("안개를 걷어낼 클릭 횟수 부족");
                return;
            }
        }

        for (int x = -fogRevealRadius; x <= fogRevealRadius; x++)
        {
            for (int y = -fogRevealRadius; y <= fogRevealRadius; y++)
            {
                Vector3Int targetPos = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);

                if (!revealedFogPositions.Contains(targetPos))
                {
                    revealedFogPositions.Add(targetPos);

                    if (fogTilemap.HasTile(targetPos))
                    {
                        fogTilemap.SetTile(targetPos, null);
                    }
                }
            }
        }

        Debug.Log("안개가 걷혔습니다!");
    }
}
