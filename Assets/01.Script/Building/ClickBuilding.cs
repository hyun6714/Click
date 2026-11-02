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
        Debug.Log($"[디버그] Initialize 실행됨! 보너스 수치: {data.maxClickBonus}");
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

        Debug.Log($"[건물 효과 적용] {clickData.buildingName} 건설 완료 최대 클릭 수 {clickData.maxClickBonus} 증가");

        EventManager.TriggerAddMaxClick(clickData.maxClickBonus);
        effectApplied = true;
    }

    private void RemoveBuildingEffect()
    {
        if (clickData == null || !effectApplied)
        {
            return;
        }

        Debug.Log($"[건물 효과 해제] {clickData.buildingName} 파괴됨 최대 클릭 수 감소");

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