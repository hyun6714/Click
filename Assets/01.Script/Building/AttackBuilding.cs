using UnityEngine;

public class AttackBuilding : MonoBehaviour, IDamageable
{
    private WeatherType currentWeather = WeatherType.Sunny;
    private AttackBuildingData towerData;
    private Vector3Int cellPosition;
    private int currentHealth;
    private float cooldown = 0f;

    private bool isInitialized = false;

    private float baseAttackSpeed;
    private float baseAttackDamage;

    public AttackBuildingData TowerData => towerData;
    public int CurrentHealth => currentHealth;
    public Vector3Int CellPosition => cellPosition;

    private void OnEnable()
    {
        EventManager.OnWeatherChanged += HandleWeatherChanged;
    }

    private void OnDisable()
    {
        EventManager.OnWeatherChanged -= HandleWeatherChanged;
    }

    public void Initialize(AttackBuildingData data, Vector3Int pos)
    {
        towerData = data;
        cellPosition = pos;
        currentHealth = towerData.maxBuildingHp;

        baseAttackSpeed = towerData.attackSpeed;
        baseAttackDamage = towerData.attackDamage;

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized || towerData == null)
        {
            return;
        }
        if (towerData == null)
        {
            return;
        }

        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        Transform target = FindNearestEnemy(towerData.attackRange);
        if (target != null)
        {
            Attack(target);
            cooldown = 1f / GetCurrentAttackSpeed();
        }
    }

    Transform FindNearestEnemy(float range)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, LayerMask.GetMask("Enemy"));
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }
        return nearest;
    }

    void Attack(Transform target)
    {
        float finalDamage = GetCurrentAttackDamage();
        Debug.Log($"{towerData.buildingName} 공격! 데미지: {finalDamage}");

        if (target.TryGetComponent<IDamageable>(out var damageableTarget))
        {
            damageableTarget.TakeDamage(Mathf.RoundToInt(finalDamage));
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[타워 피격] {towerData.buildingName} 남은 체력: {currentHealth} / {towerData.maxBuildingHp}");

        if (currentHealth <= 0)
        {
            DestroyTower();
        }
    }

    private void DestroyTower()
    {
        Debug.Log($"[타워 파괴] {towerData.buildingName} 파괴 및 타일 제거");

        EventManager.TriggerRequestDemolish(cellPosition);

        Destroy(gameObject);
    }

    private float GetCurrentAttackSpeed()
    {
        bool isInFirepit = Firepit.IsPositionInAnyFirepitRange(transform.position);
        float multiplier = 1.0f;

        if (currentWeather == WeatherType.Snow)
        {
            multiplier = isInFirepit ? 1.0f : 0.8f; // 화로가 있으면 1.0, 없으면 0.8
        }

        return baseAttackSpeed * multiplier;
    }

    private float GetCurrentAttackDamage()
    {
        bool isInFirepit = Firepit.IsPositionInAnyFirepitRange(transform.position);

        float multiplier = 1.0f;
        if (currentWeather == WeatherType.Snow)
        {
            multiplier = isInFirepit ? 1.0f : 0.8f;
        }

        return baseAttackDamage * multiplier;
    }

    private void HandleWeatherChanged(WeatherType newWeather)
    {
        currentWeather = newWeather;
    }

    void OnDrawGizmosSelected()
    {
        if (towerData != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, towerData.attackRange);
        }
    }
}