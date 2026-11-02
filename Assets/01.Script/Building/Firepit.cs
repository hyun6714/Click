using System.Collections.Generic;
using UnityEngine;

public class Firepit : MonoBehaviour, IDamageable
{
    private FirepitData firepitData;
    private Vector3Int cellPosition;
    private int currentHealth;

    public FirepitData FirepitData => firepitData;
    public int CurrentHealth => currentHealth;
    public Vector3Int CellPosition => cellPosition;

    public static List<Firepit> activeFirepits = new List<Firepit>();

    private void OnEnable()
    {
        if (!activeFirepits.Contains(this))
        {
            activeFirepits.Add(this);
        }
    }

    private void OnDisable()
    {
        if (activeFirepits.Contains(this))
        {
            activeFirepits.Remove(this);
        }
    }

    public void Initialize(FirepitData data, Vector3Int pos)
    {
        firepitData = data;
        cellPosition = pos;
        currentHealth = firepitData.maxBuildingHp;
    }

    public static bool IsPositionInAnyFirepitRange(Vector3 worldPosition)
    {
        foreach (var firepit in activeFirepits)
        {
            if (firepit == null || firepit.firepitData == null)
            {
                continue;
            }

            float distance = Vector2.Distance(firepit.transform.position, worldPosition);
            if (distance <= firepit.firepitData.effectRadius)
            {
                return true; // 온기 범위 안
            }
        }
        return false;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        EventManager.TriggerRequestDamageBuilding(cellPosition, damage);
    }

    private void OnDrawGizmosSelected()
    {
        if (firepitData != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f); // 주황색 반투명
            Gizmos.DrawWireSphere(transform.position, firepitData.effectRadius);
        }
    }
}