using UnityEngine;

public class MainCastle : MonoBehaviour, IDamageable
{
    private BuildingData castleData;
    private Vector3Int cellPosition;
    private int currentHealth;

    public void Initialize(BuildingData data, Vector3Int pos)
    {
        castleData = data;
        cellPosition = pos;
        currentHealth = castleData.maxBuildingHp;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[메인 성 피격] 남은 체력: {currentHealth} / {castleData.maxBuildingHp}");

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("[게임 오버] 메인 성이 파괴");
        Destroy(gameObject);
    }
}