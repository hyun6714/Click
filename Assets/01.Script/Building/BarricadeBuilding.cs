using UnityEngine;

public class BarricadeBuilding : MonoBehaviour, IDamageable
{
    private BarricadeBuildingData barricadeData;
    private Vector3Int cellPosition;
    private int currentHealth;

    public BarricadeBuildingData BarricadeData => barricadeData;
    public Vector3Int CellPosition => cellPosition;
    public int CurrentHealth => currentHealth;

    public void Initialize(BarricadeBuildingData data, Vector3Int pos)
    {
        barricadeData = data;
        cellPosition = pos;
        currentHealth = barricadeData.maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        EventManager.TriggerRequestDamageBuilding(cellPosition, damage);
    }
}