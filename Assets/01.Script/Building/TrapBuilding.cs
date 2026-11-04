using UnityEngine;
using System.Collections;

public class TrapBuilding : MonoBehaviour
{
    private TrapBuildingData trapData;
    private Vector3Int cellPosition;

    [Header("감지 설정")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("지속형 함정 설정")]
    [SerializeField] private float damageInterval = 1.0f; //공격 속도
    private float nextDamageTime;

    [Header("발동 이펙트")]
    [SerializeField] private GameObject activationEffectPrefab;

    public TrapBuildingData TrapData => trapData;
    public Vector3Int CellPosition => cellPosition;
    public int CurrentHealth => 1;

    public void Initialize(TrapBuildingData data, Vector3Int pos)
    {
        trapData = data;
        cellPosition = pos;
        nextDamageTime = 0f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((enemyLayer & (1 << collision.gameObject.layer)) == 0)
        {
            return;
        }

        if (Time.time < nextDamageTime)
        {
            return;
        }

        if (!collision.TryGetComponent<IDamageable>(out var target))
        {
            return;
        }

        nextDamageTime = Time.time + damageInterval;

        if (activationEffectPrefab != null)
        {
            Instantiate(activationEffectPrefab, transform.position, Quaternion.identity);
        }

        EventManager.PlaySound(SoundCue.TrapActivate);
        target.TakeDamage(trapData.damage);

        Debug.Log($"[함정 발동] {trapData.buildingName} 피해량: {trapData.damage}");
    }

    public void DestroyTrap()
    {
        Debug.Log($"[함정 철거] {trapData.buildingName} 회수 및 타일 제거");

        EventManager.TriggerRequestDemolish(cellPosition);

        Destroy(gameObject);
    }
}