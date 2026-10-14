using UnityEngine;

public class TrapBuilding : MonoBehaviour
{
    private TrapBuildingData trapData;
    private Vector3Int cellPosition;

    [Header("감지 설정")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("지속형 함정 설정")]
    [SerializeField] private float damageInterval = 1.0f; //공격 속도
    private float nextDamageTime;

    public void Initialize(TrapBuildingData data, Vector3Int pos)
    {
        trapData = data;
        cellPosition = pos;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((enemyLayer & (1 << collision.gameObject.layer)) != 0)
        {
            if (Time.time >= nextDamageTime)
            {
                nextDamageTime = Time.time + damageInterval;

                if (collision.TryGetComponent<IDamageable>(out var target))
                {
                    target.TakeDamage(trapData.damage);
                    Debug.Log($"[가시 발동] {trapData.buildingName}이(가) 적을 공격 데미지: {trapData.damage}");
                }
            }
        }
    }

    public void DestroyTrap()
    {
        Debug.Log($"[함정 철거] {trapData.buildingName} 회수 및 타일 제거");

        EventManager.TriggerRequestDemolish(cellPosition);

        Destroy(gameObject);
    }
}