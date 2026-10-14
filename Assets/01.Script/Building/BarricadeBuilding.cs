using UnityEngine;

public class BarricadeBuilding : MonoBehaviour, IDamageable
{
    private BarricadeBuildingData barricadeData;
    private Vector3Int cellPosition;
    private int currentHealth;

    public void Initialize(BarricadeBuildingData data, Vector3Int pos)
    {
        barricadeData = data;
        cellPosition = pos;
        currentHealth = barricadeData.maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"[바리케이트 피격] 남은 체력: {currentHealth} / {barricadeData.maxHealth}");

        if (currentHealth <= 0)
        {
            DestroyBarricade();
        }
    }

    private void DestroyBarricade()
    {
        Debug.Log($"[바리케이트 파괴] {barricadeData.buildingName} 파괴 및 타일 제거");

        EventManager.TriggerRequestDemolish(cellPosition);

        Destroy(gameObject);
    }
}