using UnityEngine;

public abstract class AttackBuilding : MonoBehaviour, IDamageable
{
    [SerializeField] private Transform firePoint;

    private WeatherType currentWeather = WeatherType.Sunny;
    private AttackBuildingData towerData;
    private Vector3Int cellPosition;
    private int currentHealth;
    private float cooldown = 0f;

    private bool isInitialized = false;

    private float baseAttackSpeed;
    private float baseAttackDamage;

    [SerializeField] private Animator animator;

    public AttackBuildingData TowerData => towerData;
    public int CurrentHealth => currentHealth;
    public Vector3Int CellPosition => cellPosition;
    protected Transform FirePoint => firePoint;
    protected Animator Animator => animator;

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

        Transform target = FindTarget(towerData.attackRange);
        if (target != null)
        {
            float attackSpeed = GetCurrentAttackSpeed();

            if (attackSpeed <= 0f)
            {
                return;
            }

            Attack(target);
            cooldown = 1f / attackSpeed;
        }
    }

    protected virtual Transform FindTarget(float range)
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

    protected virtual void Attack(Transform target)
    {
        if (target == null || towerData.projectilePrefab == null)
        {
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

        Vector2 direction = (target.position - spawnPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject projectileObject = Instantiate( towerData.projectilePrefab, spawnPosition, Quaternion.Euler(0f, 0f, angle));

        TowerProjectile projectile = projectileObject.GetComponent<TowerProjectile>();

        if (projectile == null)
        {
            Debug.LogError($"[{towerData.buildingName}] projectilePrefab에 TowerProjectile 스크립트가 없어 발사체를 제거합니다.");

            Destroy(projectileObject);
            return;
        }

        int damage = Mathf.RoundToInt(GetCurrentAttackDamage());

        projectile.Initialize(target, damage, towerData.projectileSpeed
        );

        Debug.Log($"[{towerData.buildingName}] 투사체 발사! 데미지: {damage}");
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        EventManager.TriggerRequestDamageBuilding(cellPosition, damage);
    }

    protected float GetCurrentAttackSpeed()
    {
        bool isInFirepit = Firepit.IsPositionInAnyFirepitRange(transform.position);
        float multiplier = 1.0f;

        if (currentWeather == WeatherType.Snow)
        {
            multiplier = isInFirepit ? 1.0f : 0.8f; // 화로가 있으면 1.0, 없으면 0.8
        }

        return baseAttackSpeed * multiplier;
    }

    protected float GetCurrentAttackDamage()
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