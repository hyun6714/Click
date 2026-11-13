using UnityEngine;

public class ProductionBoostBuilding : MonoBehaviour, IDamageable
{
    private ProductionBoostBuildingData boostData;
    private Vector3Int cellPosition;
    private int currentHealth;

    private bool effectApplied;

    public ProductionBoostBuildingData BoostData => boostData;
    public int CurrentHealth => currentHealth;
    public Vector3Int CellPosition => cellPosition;

    private void OnEnable()
    {
        EventManager.OnBuildingUnregistered += HandleBuildingUnregistered;
    }

    private void OnDisable()
    {
        EventManager.OnBuildingUnregistered -= HandleBuildingUnregistered;
    }


    public void Initialize(ProductionBoostBuildingData data, Vector3Int pos)
    {
        boostData = data;
        cellPosition = pos;
        currentHealth = boostData.maxBuildingHp;

        ApplyBuildingEffect();
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
        if (boostData == null || effectApplied)
        {
            return;
        }

        EventManager.TriggerAddDailyProductionBonus(boostData.dailyProductionBonus);

        effectApplied = true;
    }

    private void RemoveBuildingEffect()
    {
        if (boostData == null || !effectApplied)
        {
            return;
        }

        EventManager.TriggerAddDailyProductionBonus(-boostData.dailyProductionBonus);

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