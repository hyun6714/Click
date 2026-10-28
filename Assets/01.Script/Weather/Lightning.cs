using UnityEngine;
using System.Collections;

public class LightningManager : MonoBehaviour
{
    [Header("사운드 & 이펙트")]
    [SerializeField] private AudioClip thunderSound; // 천둥 소리
    [SerializeField] private GameObject lightningEffectPrefab; // 번개 맞은 자리 이펙트
    private AudioSource audioSource;

    private bool isStormActive = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        EventManager.OnWeatherChanged += HandleWeatherChanged;
    }

    private void OnDisable()
    {
        EventManager.OnWeatherChanged -= HandleWeatherChanged;
    }

    private void HandleWeatherChanged(WeatherType newWeather)
    {
        if (newWeather == WeatherType.Storm)
        {
            isStormActive = true;
            StartCoroutine(StormRoutine());
        }
        else
        {
            isStormActive = false;
            StopAllCoroutines();
        }
    }

    IEnumerator StormRoutine()
    {
        while (isStormActive)
        {
            float waitTime = Random.Range(3f, 7f);
            yield return new WaitForSeconds(waitTime);

            TriggerRandomLightning();
        }
    }

    private void TriggerRandomLightning()
    {
        //현재 카메라 영역을 기준으로 랜덤한 월드 좌표 생성
        Camera cam = Camera.main;
        if (cam == null)
        {
            return;
        }

        // 카메라 화면 크기 계산 (2D 기준)
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        // 화면 안쪽 혹은 화면 주변에서 랜덤 위치 지정 (원하는 범위로 조절 가능)
        float randomX = Random.Range(cam.transform.position.x - horzExtent, cam.transform.position.x + horzExtent);
        float randomY = Random.Range(cam.transform.position.y - vertExtent, cam.transform.position.y + vertExtent);
        Vector3 strikePos = new Vector3(randomX, randomY, 0f);

        //안개나 시야에 가려져 있는지 체크
        bool isInsideCamera = IsPositionInsideCamera(strikePos, cam);

        bool isFogged = EventManager.CheckIfPositionFogged(strikePos);

        if (isInsideCamera && !isFogged)
        {
            // === 시야 안쪽: 소리 + 이펙트 + 데미지
            audioSource.PlayOneShot(thunderSound, 1.0f);

            if (lightningEffectPrefab != null)
            {
                Instantiate(lightningEffectPrefab, strikePos, Quaternion.identity);
            }

            ApplyLightningDamageAt(strikePos);

            Debug.Log($"[번개] 시야 내 낙하! 위치: {strikePos}");
        }
        else
        {
            //안개 속이거나 화면 밖: 웅장한 천둥 소리만 재생 
            audioSource.PlayOneShot(thunderSound, 0.4f);
            Debug.Log("[번개] 시야 밖/안개 속 낙하 (소리만 재생)");
        }
    }

    // 위치가 현재 카메라 화면 안에 있는지 체크하는 함수
    private bool IsPositionInsideCamera(Vector3 pos, Camera cam)
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(pos);
        // 0~1 사이에 있으면 화면 내부
        return (viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1);
    }

    private void ApplyLightningDamageAt(Vector3 pos)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(pos);

        foreach (var hit in hits)
        {
            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(20);

                Debug.Log($"{hit.gameObject.name}에게 낙뢰 데미지 20 적용!");
            }
        }
    }
}