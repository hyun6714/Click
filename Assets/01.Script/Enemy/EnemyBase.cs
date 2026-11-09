using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("시각 효과 관련")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject poisonEffectPrefab;
    [SerializeField] private GameObject iceEffectPrefab;
    [SerializeField] protected Animator animator;

    private Color originalColor;
    private GameObject activePoisonEffect;
    private GameObject activeIceEffect;

    protected WeatherType currentWeather = WeatherType.Sunny;

    protected EnemyData enemyData;
    protected int currentHp;
    protected Transform target;
    protected float attackCooldown;
    private bool isDead;

    [Header("타겟팅 설정")]
    [SerializeField] protected float searchRadius = 5f;

    [Header("디버프 상태 관리")]
    private float slowMultiplier = 1.0f;
    private float slowTimer = 0f;

    private bool isDoTActive = false;
    private float dotTickDamage = 0f;
    private float dotTimer = 0f;
    private float dotTickInterval = 1.0f;
    private float dotTickTimer = 0f;

    protected static HashSet<Vector3Int> activeBuildingPositions = new HashSet<Vector3Int>();
    protected Vector3Int currentBuildingCell;
    protected bool hasBuildingTarget = false;
    protected float baseMoveSpeed;
    protected float baseAttackSpeed;
    protected int baseAttackDamage;

    protected static Tilemap sharedBuildingTilemap;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

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

        isDead = false;
        hasBuildingTarget = false;

        baseMoveSpeed = enemyData.moveSpeed;
        baseAttackSpeed = enemyData.attackSpeed;
        baseAttackDamage = enemyData.attackDamage;

        gameObject.SetActive(true);

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Hit");
            animator.SetBool("Move", false);
            animator.Play("EnemyIdle", 0, 0f);
        }

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

        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                slowMultiplier = 1.0f;
            }
        }

        if (isDoTActive)
        {
            dotTimer -= Time.deltaTime;
            dotTickTimer += Time.deltaTime;

            if (dotTickTimer >= dotTickInterval)
            {
                dotTickTimer = 0f;
                TakeDamage(Mathf.RoundToInt(dotTickDamage));
            }

            if (dotTimer <= 0f)
            {
                isDoTActive = false;
            }
        }

        MoveOrAttack();
    }

    protected virtual void MoveOrAttack()
    {
        FindNearestBuilding();

        Vector3 targetWorldPos;

        if (hasBuildingTarget && sharedBuildingTilemap != null)
        {
            if (!activeBuildingPositions.Contains(currentBuildingCell))
            {
                hasBuildingTarget = false;
            }
        }

        if (hasBuildingTarget && sharedBuildingTilemap != null)
        {
            targetWorldPos = sharedBuildingTilemap.GetCellCenterWorld(currentBuildingCell);
        }
        else if (target != null)
        {
            targetWorldPos = target.position;
        }
        else
        {
            SetMoveAnimation(false);
            return;
        }

        float distance = Vector2.Distance(transform.position, targetWorldPos);

        if (distance <= enemyData.attackRange)
        {
            SetMoveAnimation(false);

            if (attackCooldown <= 0)
            {
                float currentAttackSpeed = baseAttackSpeed;

                if (currentAttackSpeed <= 0f)
                {
                    attackCooldown = 1f;
                    return;
                }

                FaceDirection(targetWorldPos - transform.position);
                PlayAttackAnimation();
                attackCooldown = 1f / currentAttackSpeed;
            }
        }
        else
        {
            SetMoveAnimation(true);
            Move(targetWorldPos);
        }
    }

    protected virtual void FindNearestBuilding()
    {
        if (hasBuildingTarget)
        {
            if (!activeBuildingPositions.Contains(currentBuildingCell))
            {
                hasBuildingTarget = false;
            }
        }

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
            Vector3 worldPos = sharedBuildingTilemap.GetCellCenterWorld(pos);
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
        else if (!hasBuildingTarget)
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
        if (isDead || damage <= 0)
        {
            return;
        }


        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);

        EventManager.PlaySound(SoundCue.EnemyHit);

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    protected virtual void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;


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
        return baseMoveSpeed * slowMultiplier;
    }

    protected virtual int GetCurrentAttackDamage()
    {
        return Mathf.RoundToInt(baseAttackDamage);
    }

    protected void FaceDirection(Vector3 direction)
    {
        if (spriteRenderer == null || Mathf.Abs(direction.x) < 0.01f)
        {
            return;
        }

        spriteRenderer.flipX = direction.x < 0f;
    }

    protected void SetMoveAnimation(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetBool("Move", isMoving);
        }
    }

    protected virtual void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        EventManager.PlaySound(SoundCue.EnemyAttack);
        Attack();
    }

    public virtual void ApplySlow(float amount, float duration)
    {
        slowMultiplier = Mathf.Clamp(1f - amount, 0.1f, 1.0f);
        slowTimer = duration;

        StartCoroutine(IceEffectRoutine(duration));
    }

    public virtual void ApplyDoT(float dDamage, float duration)
    {
        isDoTActive = true;
        dotTickDamage = dDamage;
        dotTimer = duration;
        dotTickTimer = 0f;

        StartCoroutine(PoisonEffectRoutine(duration));
    }

    private IEnumerator IceEffectRoutine(float duration)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.5f, 0.8f, 1f);
        }

        if (iceEffectPrefab != null && activeIceEffect == null)
        {
            activeIceEffect = Instantiate(iceEffectPrefab, transform.position + Vector3.up * 0.8f, Quaternion.identity, transform);
        }

        yield return new WaitForSeconds(duration);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (activeIceEffect != null)
        {
            Destroy(activeIceEffect);
        }
    }

    private IEnumerator PoisonEffectRoutine(float duration)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.4f, 1f, 0.4f); 
        }


        if (poisonEffectPrefab != null && activePoisonEffect == null)
        {
            activePoisonEffect = Instantiate(poisonEffectPrefab, transform.position + Vector3.up * 0.8f, Quaternion.identity, transform);
        }

        yield return new WaitForSeconds(duration);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (activePoisonEffect != null)
        {
            Destroy(activePoisonEffect);
        }
    }
}