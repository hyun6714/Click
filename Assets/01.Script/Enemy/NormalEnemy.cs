using UnityEngine;

public class NormalEnemy : EnemyBase
{
    [SerializeField] private float bounceSpeed = 8f;
    [SerializeField] private float bounceAmount = 0.08f;
    [SerializeField] private float moveThreshold = 0.0001f;

    private Vector3 originalScale;
    private Vector3 previousPosition;

    private void Start()
    {
        originalScale = transform.localScale;
        previousPosition = transform.position;
    }

    private void LateUpdate()
    {
        bool isMoving = (transform.position - previousPosition).sqrMagnitude> moveThreshold * moveThreshold;

        previousPosition = transform.position;

        if (!isMoving)
        {
            transform.localScale = originalScale;
            return;
        }

        float bounce = Mathf.Abs(Mathf.Sin(Time.time * bounceSpeed));

        transform.localScale = new Vector3(originalScale.x * (1f - bounce * bounceAmount * 0.5f), originalScale.y * (1f + bounce * bounceAmount), originalScale.z );
    }
}