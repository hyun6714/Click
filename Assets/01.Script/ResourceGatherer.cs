using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class ResourceGatherer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap referenceTilemap; // ±âÁØÀÌ µÇ´Â ¹Ù´Ú Å¸ÀÏ¸Ê
    public Tilemap treeTilemap;      // ³ª¹« Å¸ÀÏ¸Ê
    public Tilemap stoneTilemap;     // µ¹ Å¸ÀÏ¸Ê


    [Header("Ã¤Áý ½Ã È¹µæ·®")]
    [SerializeField] private int harvestAmount = 1;

    [Header("Å¬¸¯ Â÷°¨")]
    [SerializeField] private int clickCost = 1;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryGatherResource();
        }
    }

    private void TryGatherResource()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector3Int cellPos = referenceTilemap.WorldToCell(mouseWorldPos);
        cellPos.z = 0;

        bool hasTree = treeTilemap.HasTile(cellPos);
        bool hasStone = stoneTilemap.HasTile(cellPos);

        if (!hasTree && !hasStone) return;

        bool canUseClick = EventManager.RequestUseCurrency(CurrencyType.Click, clickCost);

        if (!canUseClick)
        {
            Debug.Log("Å¬¸¯ È½¼ö ºÎÁ·");
            return;
        }

        if (treeTilemap.HasTile(cellPos))
        {
            EventManager.CurrencyAdded(CurrencyType.Tree, harvestAmount);
            Debug.Log("³ª¹« È¹µæ");

            treeTilemap.SetTile(cellPos, null);
            return;
        }

        if (stoneTilemap.HasTile(cellPos))
        {
            EventManager.CurrencyAdded(CurrencyType.Rock, harvestAmount);
            Debug.Log("µ¹ È¹µæ");

            stoneTilemap.SetTile(cellPos, null);
            return;
        }
    }
}