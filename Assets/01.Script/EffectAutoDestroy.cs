using UnityEngine;

public class EffectAutoDestroy : MonoBehaviour
{
    [SerializeField] private float destroyTime = 0.5f; // 이펙트가 유지될 시간

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}