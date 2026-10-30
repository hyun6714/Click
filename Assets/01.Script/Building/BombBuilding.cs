using UnityEngine;

public class BombBuilding : MonoBehaviour
{
    private TrapBuildingData trapData;
    private Vector3Int cellPosition;

    [Header("감지 및 폭발 설정")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float explosionRadius = 1.5f; // 폭발 범위


    public TrapBuildingData TrapData => trapData;
    public Vector3Int CellPosition => cellPosition;
    public int CurrentHealth => 1;

    public void Initialize(TrapBuildingData data, Vector3Int pos)
    {
        trapData = data;
        cellPosition = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((enemyLayer & (1 << collision.gameObject.layer)) != 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Debug.Log($"[폭발!] {trapData.buildingName}이(가) 발동 데미지: {trapData.damage}");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);

        foreach (var enemyCollider in hitEnemies)
        {
            if (enemyCollider.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(trapData.damage);
            }
        }

        EventManager.TriggerRequestDemolish(cellPosition);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}