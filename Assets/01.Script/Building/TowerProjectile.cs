using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    public enum ProjectileType { Arrow, Straight, Mortar, Ice, Poison }

    [Header("투사체 타입")]
    [SerializeField] private ProjectileType projectileType;

    private Transform target;
    private int damage;
    private float speed;
    private bool initialized;

    private bool isAreaAttack = false;
    private float splashRadius = 0f;

    [Header("마법 타워")]
    private float slowAmount = 0f;   // 슬로우 비율
    private float slowDuration = 0f; // 지속 시간
    private bool isDoT = false;      // 도트 데미지 여부
    private float dotDamage = 0f;    // 틱당 도트 데미지
    private float dotDuration = 0f;

    [Header("박격포")]
    [SerializeField] private GameObject explosionEffectPrefab;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float flightDuration = 0.6f;
    private float elapsedTime = 0f;
    private float arcHeight = 2.0f;

    public void InitializeArrow(Transform newTarget, int newDamage, float newSpeed)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        isAreaAttack = false;
        projectileType = ProjectileType.Arrow;
        initialized = true;
    }

    public void Initialize(Transform newTarget, int newDamage, float newSpeed)
    {
        InitializeArrow(newTarget, newDamage, newSpeed);
    }

    public void InitializeMortar(Transform newTarget, int newDamage, float newSpeed, float radius)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        splashRadius = radius;
        isAreaAttack = true;
        projectileType = ProjectileType.Mortar;

        startPosition = transform.position;
        if (target != null)
        {
            targetPosition = target.position;
        }
        else
        {
            targetPosition = startPosition + Vector3.right * 2f;
        }

        initialized = true;
    }

    public void InitializeMagic(Transform newTarget, int newDamage, float newSpeed, float radius, float slowAmt, float slowDur, bool dotFlag, float dotDmg, float dotDur)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        splashRadius = radius;
        isAreaAttack = (radius > 0f);
        slowAmount = slowAmt;
        slowDuration = slowDur;
        isDoT = dotFlag;
        dotDamage = dotDmg;
        dotDuration = dotDur;

        if (slowAmt > 0f)
        {
            projectileType = ProjectileType.Ice;
        }
        else if (dotFlag)
        {
            projectileType = ProjectileType.Poison;
        }
        else
        {
            projectileType = ProjectileType.Straight;
        }

        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        if (projectileType == ProjectileType.Mortar)
        {
            UpdateMortarMovement();
            return;
        }

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 previousPos = transform.position;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        Vector3 moveDir = (transform.position - previousPos);
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (Vector3.Distance(transform.position, target.position) <= 0.05f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        Vector3 hitCenter = (target != null) ? target.position : targetPosition;

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, hitCenter, Quaternion.identity);
        }

        if (isAreaAttack)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(target.position, splashRadius, LayerMask.GetMask("Enemy"));

            foreach (var hit in hits)
            {
                ApplyEffects(hit.transform);
            }
        }
        else if (target != null)
        {
            ApplyEffects(target);
        }

        Destroy(gameObject);
    }

    private void ApplyEffects(Transform enemyTransform)
    {
        IDamageable damageable = enemyTransform.GetComponent<IDamageable>();
        if (damageable == null)
        {
            damageable = enemyTransform.GetComponentInParent<IDamageable>();
        }

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        EnemyBase enemy = enemyTransform.GetComponent<EnemyBase>();
        if (enemy == null)
        {
            enemy = enemyTransform.GetComponentInParent<EnemyBase>();
        }

        if (enemy != null)
        {
            if (slowAmount > 0f)
            {
                enemy.ApplySlow(slowAmount, slowDuration);
            }

            if (isDoT && dotDamage > 0f)
            {
                enemy.ApplyDoT(dotDamage, dotDuration);
            }
        }
    }

    private void UpdateMortarMovement()
    {
        elapsedTime += Time.deltaTime;
        float linearT = elapsedTime / flightDuration;

        if (linearT >= 1f)
        {
            HitTarget();
            return;
        }


        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, linearT);

        float height = Mathf.Sin(linearT * Mathf.PI) * arcHeight;
        currentPos.y += height;

        transform.position = currentPos;
    }
}