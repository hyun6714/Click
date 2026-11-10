using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static SaveData;

public class ResourceGatherer : MonoBehaviour
{
    private GameMode currentMode = GameMode.Normal;
    private WeatherType currentWeather = WeatherType.Sunny;

    [Header("Tilemaps")]
    public Tilemap referenceTilemap; // ±âÁØÀÌ µÇ´Â ¹Ù´Ú Å¸ÀÏ¸Ê
    public Tilemap treeTilemap; // ³ª¹« Å¸ÀÏ¸Ê
    public Tilemap stoneTilemap; // µ¹ Å¸ÀÏ¸Ê
    public Tilemap fogTilemap; //¾È°³ Å¸ÀÏ¸Ê

    [Header("¾È°³ ¼³Á¤")]
    [SerializeField] private int fogRevealRadius = 2; // Å¬¸¯ ½Ã °ÈÈú ¹üÀ§
    [SerializeField] private int fogClickCost = 1; // ¼Ò¸ð·®
    [SerializeField] private float fogFadeDuration = 0.12f;

    [Header("Ã¤Áý ½Ã È¹µæ·®")]
    [SerializeField] private int harvestAmount = 1;

    [Header("Å¬¸¯ Â÷°¨")]
    [SerializeField] private int clickCost = 1;

    [Header("FX¿ë ÂüÁ¶")]
    [SerializeField] private ResourceGainPopup gainPopupPrefab;
    [SerializeField] private Sprite treeIcon;
    [SerializeField] private Sprite rockIcon;

    private HashSet<Vector3Int> revealedFogPositions = new HashSet<Vector3Int>();

    private void OnEnable()
    {
        EventManager.OnGameModeChanged += HandleGameModeChanged;
        EventManager.OnWeatherChanged += HandleWeatherChanged;
        EventManager.OnRequestRevealedFogList += GetRevealedFogList;
    }

    private void OnDisable()
    {
        EventManager.OnGameModeChanged -= HandleGameModeChanged;
        EventManager.OnWeatherChanged -= HandleWeatherChanged;
        EventManager.OnRequestRevealedFogList -= GetRevealedFogList;
    }

    private void HandleGameModeChanged(GameMode newMode)
    {
        currentMode = newMode;
    }

    private void HandleWeatherChanged(WeatherType newWeather)
    {
        currentWeather = newWeather;
    }

    private void Start()
    {
        if (EventManager.OnRequestCurrentWeather != null)
        {
            currentWeather = EventManager.OnRequestCurrentWeather.Invoke();
        }
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
            EventManager.PlaySound(SoundCue.ResourceInsufficient);
            Debug.Log("Å¬¸¯ È½¼ö ºÎÁ·");
            return;
        }

        WeatherType liveWeather = currentWeather;
        if (EventManager.OnRequestCurrentWeather != null)
        {
            liveWeather = EventManager.OnRequestCurrentWeather.Invoke();
        }

        int finalHarvestAmount = (liveWeather == WeatherType.Rain) ? harvestAmount * 2 : harvestAmount;
        SaveData currentSaveData = SaveLoadManager.LoadGame() ?? new SaveData();

        if (treeTilemap.HasTile(cellPos))
        {
            EventManager.CurrencyAdded(CurrencyType.Tree, finalHarvestAmount);
            EventManager.PlaySound(SoundCue.ResourceGather);
            Debug.Log("³ª¹« È¹µæ");

            if (gainPopupPrefab != null)
            {
                Vector3 popupPosition = treeTilemap.GetCellCenterWorld(cellPos);
                ResourceGainPopup popup = Instantiate(gainPopupPrefab, popupPosition, Quaternion.identity);
                popup.Initialize(finalHarvestAmount, treeIcon);
            }

            treeTilemap.SetTile(cellPos, null);

            if (currentSaveData.removedTrees == null) currentSaveData.removedTrees = new List<SerializableVector3Int>();
            SerializableVector3Int wrappedPos = new SerializableVector3Int(cellPos);

            if (!currentSaveData.removedTrees.Contains(wrappedPos))
            {
                currentSaveData.removedTrees.Add(wrappedPos);
                SaveLoadManager.SaveRawData(currentSaveData); // Áï½Ã ÀúÀå
            }
            return;
        }

        if (stoneTilemap.HasTile(cellPos))
        {
            EventManager.CurrencyAdded(CurrencyType.Rock, finalHarvestAmount);
            EventManager.PlaySound(SoundCue.ResourceGather);
            Debug.Log("µ¹ È¹µæ");

            if (gainPopupPrefab != null)
            {
                Vector3 popupPosition = stoneTilemap.GetCellCenterWorld(cellPos);
                ResourceGainPopup popup = Instantiate(gainPopupPrefab, popupPosition, Quaternion.identity);
                popup.Initialize(finalHarvestAmount, rockIcon);
            }

            stoneTilemap.SetTile(cellPos, null);

            if (currentSaveData.removedStones == null) currentSaveData.removedStones = new List<SerializableVector3Int>();
            SerializableVector3Int wrappedPos = new SerializableVector3Int(cellPos);

            if (!currentSaveData.removedStones.Contains(wrappedPos))
            {
                currentSaveData.removedStones.Add(wrappedPos);
                SaveLoadManager.SaveRawData(currentSaveData); // Áï½Ã ÀúÀå
            }

            return;
        }
    }

    private void TryClearFog(Vector3Int centerCell)
    {
        if (fogTilemap == null || revealedFogPositions.Contains(centerCell) ||
            !fogTilemap.HasTile(centerCell) || !HasClearCardinalNeighbor(centerCell))
        {
            return;
        }

        if (fogClickCost > 0)
        {
            bool canUseClick = EventManager.RequestUseCurrency(CurrencyType.Click, fogClickCost);
            if (!canUseClick)
            {
                Debug.Log("¾È°³¸¦ °È¾î³¾ Å¬¸¯ È½¼ö ºÎÁ·");
                return;
            }

            EventManager.TriggerRequestSaveGame();
        }

        StartCoroutine(ClearFogPlusThenDiagonals(centerCell));

        Debug.Log("¾È°³°¡ °ÈÇû½À´Ï´Ù!");
    }

    private IEnumerator ClearFogPlusThenDiagonals(Vector3Int centerCell)
    {
        List<Vector3Int> plusCells = new List<Vector3Int>();

        Vector3Int[] plusOffsets = {Vector3Int.zero, Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right,
                                   new Vector3Int(0, 2, 0), new Vector3Int(0, -2, 0), new Vector3Int(-2, 0, 0), new Vector3Int(2, 0, 0) };
       
        foreach (Vector3Int offset in plusOffsets)
        {
            Vector3Int cell = centerCell + offset;

            if (fogTilemap.HasTile(cell) && revealedFogPositions.Add(cell))
            {
                plusCells.Add(cell);
                StartCoroutine(FadeOutFogTile(cell, 0f));


            }
        }

        bool plusTilesRemain;

        do
        {
            plusTilesRemain = false;

            foreach (Vector3Int cell in plusCells)
            {
                if (fogTilemap.HasTile(cell))
                {
                    plusTilesRemain = true;
                    break;
                }
            }

            if (plusTilesRemain)
            {
                yield return null;
            }
        }

        while (plusTilesRemain);

        List<Vector3Int> remainingCells = new List<Vector3Int>();

        for (int x = -fogRevealRadius; x <= fogRevealRadius; x++)
        {
            for (int y = -fogRevealRadius; y <= fogRevealRadius; y++)
            {
                Vector3Int cell = centerCell + new Vector3Int(x, y, 0);

                if (fogTilemap.HasTile(cell) && revealedFogPositions.Add(cell))
                {
                    remainingCells.Add(cell);
                }
            }
        }

        remainingCells.Sort((a, b) =>
        {
            int distanceA = Mathf.Abs(a.x - centerCell.x) + Mathf.Abs(a.y - centerCell.y);
            int distanceB = Mathf.Abs(b.x - centerCell.x) + Mathf.Abs(b.y - centerCell.y);
            return distanceA.CompareTo(distanceB);
        });

        foreach (Vector3Int cell in remainingCells)
        {
            if (fogTilemap.HasTile(cell))
            {
                yield return StartCoroutine(FadeOutFogTile(cell, 0f));

            }
        }
    }


    private bool HasClearCardinalNeighbor(Vector3Int cell)
    {
        Vector3Int[] directions = {Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right};

        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighbor = cell + direction;

            bool isValidMapCell = referenceTilemap == null || referenceTilemap.HasTile(neighbor);

            if (isValidMapCell && !fogTilemap.HasTile(neighbor))
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator FadeOutFogTile(Vector3Int cellPos, float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        if (fogTilemap == null || !fogTilemap.HasTile(cellPos))
        {
            yield break;
        }

        Color originalColor = fogTilemap.GetColor(cellPos);
        float duration = Mathf.Max(0.01f, fogFadeDuration);
        float elapsed = 0f;

        while (elapsed < duration && fogTilemap.HasTile(cellPos))
        {
            elapsed += Time.deltaTime;

            Color color = originalColor;
            color.a = Mathf.Lerp(originalColor.a, 0f, elapsed / duration);
            fogTilemap.SetColor(cellPos, color);

            yield return null;
        }

        fogTilemap.SetTile(cellPos, null);
        fogTilemap.SetColor(cellPos, Color.white);
    }

    public List<SerializableVector3Int> GetRevealedFogList()
    {
        List<SerializableVector3Int> list = new List<SerializableVector3Int>();
        foreach (var pos in revealedFogPositions)
        {
            list.Add(new SerializableVector3Int(pos));
        }
        return list;
    }
}
