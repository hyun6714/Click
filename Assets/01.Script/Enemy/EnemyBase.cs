using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBase : MonoBehaviour, IDamageable
{
    protected WeatherType currentWeather = WeatherType.Sunny;

    protected EnemyData enemyData;
    protected int currentHp;
    protected Transform target;
    protected float attackCooldown;

    [Header("타겟팅 설정")]
    [SerializeField] protected float searchRadius = 5f;

    protected static HashSet<Vector3Int> activeBuildingPositions = new HashSet<Vector3Int>();
    protected Vector3Int currentBuildingCell;
    protected bool hasBuildingTarget = false;
    protected float baseMoveSpeed;
    protected float baseAttackSpeed;
    protected int baseAttackDamage;

    protected static Tilemap sharedBuildingTilemap;

    private void OnEnable()
    {
        EventManager.OnBuildingRegistered += HandleBuildingRegistered;
        EventManager.OnBuildingUnregistered += HandleBuildingUnregistered;
        EventManager.OnBuildingManagerReady += HandleBuildingManagerReady;
        EventManager.OnWeatherChanged += HandleWeatherChanged;
    }

    private void OnDisable()
    {
        EventManager.OnBuildingRegistered -= HandleBuildingRegistered;
        EventManager.OnBuildingUnregistered -= HandleBuildingUnregistered;
        EventManager.OnBuildingManagerReady -= HandleBuildingManagerReady;
        EventManager.OnWeatherChanged -= HandleWeatherChanged;
    }

    private void HandleBuildingManagerReady(Tilemap tilemap)
    {
        sharedBuildingTilemap = tilemap;
    }

    private void HandleBuildingRegistered(Vector3Int pos)
    {
        if (!activeBuildingPositions.Contains(pos))
        {
            activeBuildingPositions.Add(pos);
        }
    }

    private void HandleBuildingUnregistered(Vector3Int pos)
    {
        if (activeBuildingPositions.Contains(pos))
        {
            activeBuildingPositions.Remove(pos);
        }
    }

    // 몬스터 초기화
    public virtual void Initialize(EnemyData data, Vector3 spawnPosition, Transform castleTarget)
    {
        enemyData = data;
        currentHp = enemyData.maxHp;
        target = castleTarget;

        transform.position = spawnPosition;
        attackCooldown = 0f;
        hasBuildingTarget = false;

        baseMoveSpeed = enemyData.moveSpeed;
        baseAttackSpeed = enemyData.attackSpeed;
        baseAttackDamage = enemyData.attackDamage;

        gameObject.SetActive(true);
        EventManager.TriggerRequestSyncBuildings();
    }

    protected virtual void Update()
    {
        if (enemyData == null || !gameObject.activeSelf)
        {
            return;
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        MoveOrAttack();
    }

    protected virtual void MoveOrAttack()
    {
        FindNearestBuilding();

        Vector3 targetWorldPos;

        if (hasBuildingTarget && sharedBuildingTilemap != null)
        {
            targetWorldPos = sharedBuildingTilemap.CellToWorld(currentBuildingCell) + new Vector3(0.5f, 0.5f, 0f);
        }
        else if (target != null)
        {
            targetWorldPos = target.position;
        }
        else
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, targetWorldPos);

        if (distance <= enemyData.attackRange)
        {
            if (attackCooldown <= 0)
            {
                Attack();

                float speedMultiplier = (currentWeather == WeatherType.Storm) ? 1.2f : (currentWeather == WeatherType.Snow ? 0.8f : 1.0f);
                float currentAttackSpeed = baseAttackSpeed * speedMultiplier;

                attackCooldown = 1f / currentAttackSpeed;
            }
        }
        else
        {
            Move(targetWorldPos);
        }
    }

    protected virtual void FindNearestBuilding()
    {
        if (activeBuildingPositions.Count == 0 || sharedBuildingTilemap == null)
        {
            hasBuildingTarget = false;
            return;
        }

        float shortestDistance = Mathf.Infinity;
        Vector3Int nearestPos = Vector3Int.zero;
        bool found = false;

        foreach (var pos in activeBuildingPositions)
        {
            Vector3 worldPos = sharedBuildingTilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0f);
            float distanceToBuilding = Vector2.Distance(transform.position, worldPos);

            if (distanceToBuilding <= searchRadius && distanceToBuilding < shortestDistance)
            {
                shortestDistance = distanceToBuilding;
                nearestPos = pos;
                found = true;
            }
        }

        if (found)
        {
            currentBuildingCell = nearestPos;
            hasBuildingTarget = true;
        }
        else
        {
            hasBuildingTarget = false;
        }
    }

    protected virtual void Move(Vector3 destPosition)
    {
        float speed = GetCurrentMoveSpeed();
        transform.position = Vector3.MoveTowards(transform.position, destPosition, speed * Time.deltaTime);
    }

    protected virtual void Attack()
    {
        int finalDamage = GetCurrentAttackDamage();

        if (hasBuildingTarget)
        {
            EventManager.TriggerRequestDamageBuilding(currentBuildingCell, finalDamage);
            Debug.Log($"[{enemyData.enemyName}] 타일 건물 {currentBuildingCell} 공격 데미지: {enemyData.attackDamage}");
        }
        else if (target != null)
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(finalDamage);
                Debug.Log($"[{enemyData.enemyName}] 메인 성 공격 데미지: {finalDamage}");
            }
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        EventManager.CurrencyAdded(CurrencyType.Gold, enemyData.rewardGold);
        EventManager.TriggerRequestReturnPool(enemyData.enemyType, gameObject);

        gameObject.SetActive(false);
    }

    private void HandleWeatherChanged(WeatherType newWeather)
    {
        currentWeather = newWeather;
    }

    protected virtual float GetCurrentMoveSpeed()
    {
        float multiplier = 1.0f;

        if (currentWeather == WeatherType.Storm)
        {
            multiplier = 1.2f;
        }

        if (currentWeather == WeatherType.Snow)
        {
            multiplier = 0.8f;
        }

        return baseMoveSpeed * multiplier;
    }

    protected virtual int GetCurrentAttackDamage()
    {
        float multiplier = 1.0f;

        if (currentWeather == WeatherType.Storm)
        {
            multiplier = 1.2f;
        }

        return Mathf.RoundToInt(baseAttackDamage * multiplier);
    }
}