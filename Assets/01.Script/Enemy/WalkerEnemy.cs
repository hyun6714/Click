using UnityEngine;

public class WalkerEnemy : EnemyBase
{
    [SerializeField] private float swaySpeed = 10f; 
    [SerializeField] private float swayAmount = 5f;

    private Vector3 previousPosition;

    private void Start()
    {
        previousPosition = transform.position;
    }

    private void LateUpdate()
    {
        bool isMoving = (transform.position - previousPosition).sqrMagnitude > 0.00001f;
        previousPosition = transform.position;

        if (!isMoving || spriteRenderer == null)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.transform.localRotation = Quaternion.identity;
            }
            return;
        }

        float zRotation = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        spriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);
    }
}