using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceGainPopup : MonoBehaviour
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float riseDistance = 0.7f;

    private Vector3 startPosition;
    private float elapsedTime;

    public void Initialize(int amount, Sprite icon)
    {
        startPosition = transform.position;

        if (amountText != null)
        {
            amountText.text = $"+{amount}";
        }

        if (resourceIcon != null)
        {
            resourceIcon.sprite = icon;
            resourceIcon.enabled = icon != null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        elapsedTime = 0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / duration);

        transform.position = startPosition + Vector3.up * riseDistance * progress;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f - progress;
        }

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
}