using UnityEngine;

public class ClickBuilding : MonoBehaviour, IDamageable
{
    private ClickBuildingData clickData;
    private Vector3Int cellPosition;
    private int currentHealth;

    private bool effectApplied;

    public ClickBuildingData ClickData => clickData;
    public int CurrentHealth => currentHealth;
    public Vector3Int CellPosition => cellPosition;

    public void Initialize(ClickBuildingData data, Vector3Int pos)
    { 
        clickData = data;
        cellPosition = pos;
        currentHealth = clickData.maxBuildingHp;

        ApplyBuildingEffect();
    }

    private void OnEnable()
    {
        EventManager.OnBuildingUnregistered += HandleBuildingUnregistered;
    }

    private void OnDisable()
    {
        EventManager.OnBuildingUnregistered -= HandleBuildingUnregistered;
    }

    private void HandleBuildingUnregistered(Vector3Int unregisteredCell)
    {
        if (unregisteredCell == cellPosition)
        {
            RemoveBuildingEffect();
        }
    }

    private void ApplyBuildingEffect()
    {
        if (clickData == null || effectApplied)
        {
            return;
        }

        EventManager.TriggerAddMaxClick(clickData.maxClickBonus);
        effectApplied = true;
    }

    private void RemoveBuildingEffect()
    {
        if (clickData == null || !effectApplied)
        {
            return;
        }

        EventManager.TriggerAddMaxClick(-clickData.maxClickBonus);
        effectApplied = false;
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