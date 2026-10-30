using UnityEngine;

public class ClickBuilding : MonoBehaviour, IDamageable
{
    private ClickBuildingData clickData;
    private Vector3Int cellPosition;
    private int currentHealth;

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

    private void OnMouseDown()
    {

        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        { 
            return;
        }
    }

    private void ApplyBuildingEffect()
    {
        if (clickData == null)
        {
            return;
        }

        Debug.Log($"[건물 효과 적용] {clickData.buildingName} 건설 완료 최대 클릭 수 {clickData.maxClickBonus} 증가");

        EventManager.TriggerAddMaxClick(clickData.maxClickBonus);
    }

    private void RemoveBuildingEffect()
    {
        if (clickData == null)
        {
            return;
        }

        Debug.Log($"[건물 효과 해제] {clickData.buildingName} 파괴됨 최대 클릭 수 감소");

        EventManager.TriggerAddMaxClick(-clickData.maxClickBonus);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[클릭 건물 피격] {clickData.buildingName} 남은 체력: {currentHealth} / {clickData.maxBuildingHp}");

        if (currentHealth <= 0)
        {
            DestroyBuilding();
        }
    }

    private void DestroyBuilding()
    {
        Debug.Log($"[클릭 건물 파괴] {clickData.buildingName} 파괴 및 타일 제거 요청");

        RemoveBuildingEffect();

        EventManager.TriggerRequestDemolish(cellPosition);

        Destroy(gameObject);
    }
}