using UnityEngine;

public class ProductionBoostBuilding : MonoBehaviour, IDamageable
{
    private ProductionBoostBuildingData boostData;
    private Vector3Int cellPosition;
    private int currentHealth;

    public ProductionBoostBuildingData BoostData => boostData;
    public int CurrentHealth => currentHealth;
    public Vector3Int CellPosition => cellPosition;

    public void Initialize(ProductionBoostBuildingData data, Vector3Int pos)
    {
        boostData = data;
        cellPosition = pos;
        currentHealth = boostData.maxBuildingHp;

        ApplyBuildingEffect();
    }

    private void ApplyBuildingEffect()
    {
        if (boostData == null)
        {
            return;
        }

        Debug.Log($"[건물 효과 적용] {boostData.buildingName} 건설 일일 생산량 보너스 {boostData.dailyProductionBonus} 증가");
        EventManager.TriggerAddDailyProductionBonus(boostData.dailyProductionBonus);
    }

    private void RemoveBuildingEffect()
    {
        if (boostData == null)
        {
            return;
        }

        Debug.Log($"[건물 효과 해제] {boostData.buildingName} 파괴 일일 생산량 보너스 감소");
        EventManager.TriggerAddDailyProductionBonus(-boostData.dailyProductionBonus);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[생산 건물 피격] {boostData.buildingName} 남은 체력: {currentHealth} / {boostData.maxBuildingHp}");

        if (currentHealth <= 0)
        {
            DestroyBuilding();
        }
    }

    private void DestroyBuilding()
    {
        Debug.Log($"[생산 건물 파괴] {boostData.buildingName} 파괴 및 타일 제거 요청");

        RemoveBuildingEffect();
        EventManager.TriggerRequestDemolish(cellPosition);
        Destroy(gameObject);
    }
}