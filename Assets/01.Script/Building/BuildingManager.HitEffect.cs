using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class BuildingManager
{
    private readonly Dictionary<Vector3Int, TileFlags> originalTileFlags = new();
    private readonly Dictionary<Vector3Int, Coroutine> hitEffects = new();
    private readonly Dictionary<Vector3Int, Color> originalTileColors = new();

    private void PlayTileHitEffect(Vector3Int cellPosition)
    {
        if (buildingTilemap == null || !buildingTilemap.HasTile(cellPosition))
            return;

        if (!hitEffects.ContainsKey(cellPosition))
        {
            originalTileColors[cellPosition] = buildingTilemap.GetColor(cellPosition);
            originalTileFlags[cellPosition] = buildingTilemap.GetTileFlags(cellPosition);
        }
        else
        {
            StopCoroutine(hitEffects[cellPosition]);
            hitEffects.Remove(cellPosition);
        }

        buildingTilemap.SetTileFlags(cellPosition, TileFlags.None);
        hitEffects[cellPosition] = StartCoroutine(TileHitEffectRoutine(cellPosition));
    }

    private IEnumerator TileHitEffectRoutine(Vector3Int cellPosition)
    {
        buildingTilemap.SetColor(cellPosition, Color.red);
        yield return new WaitForSeconds(0.12f);

        if (buildingTilemap != null && buildingTilemap.HasTile(cellPosition))
        {
            buildingTilemap.SetColor(cellPosition, originalTileColors[cellPosition]);
            buildingTilemap.SetTileFlags(cellPosition, originalTileFlags[cellPosition]);
        }

        hitEffects.Remove(cellPosition);
        originalTileColors.Remove(cellPosition);
        originalTileFlags.Remove(cellPosition);
    }
}