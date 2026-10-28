using UnityEngine;

public class WeatherScaleAdapter : MonoBehaviour
{
    private ParticleSystem rainParticle;
    private ParticleSystem.ShapeModule shapeModule;
    private Camera mainCamera;

    [Header("비 내리는 범위 조절 계수")]
    [SerializeField] private float rangeMultiplier = 4.0f;

    void Start()
    {
        rainParticle = GetComponent<ParticleSystem>();
        shapeModule = rainParticle.shape;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null)
        {
            return;
        }

        float zoomSize = mainCamera.orthographicSize;

        shapeModule.scale = new Vector3(zoomSize * rangeMultiplier, zoomSize * rangeMultiplier, 1f);
    }
}