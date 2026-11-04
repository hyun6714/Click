using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WavePopupUI : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button skipButton;
    [SerializeField] private float popupDuration = 5f;

    private Coroutine hidePopupCoroutine;

    private void OnEnable()
    {
        EventManager.OnWaveStarted += HandleWaveStarted;
        EventManager.OnWaveEnded += HandleWaveEnded;

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    private void OnDisable()
    {
        EventManager.OnWaveStarted -= HandleWaveStarted;
        EventManager.OnWaveEnded -= HandleWaveEnded;

        if (hidePopupCoroutine != null)
        {
            StopCoroutine(hidePopupCoroutine);
            hidePopupCoroutine = null;
        }
    }

    private void HandleWaveStarted(WaveData wave)
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }

        if (messageText != null && wave != null)
        {
            messageText.text = $"{wave.targetDay}일차 웨이브 진행 중";
        }

        if (skipButton != null)
        {
            skipButton.interactable = false;
        }

        if (hidePopupCoroutine != null)
        {
            StopCoroutine(hidePopupCoroutine);
        }

        hidePopupCoroutine = StartCoroutine(HidePopupAfterDelay());
    }

    private IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSecondsRealtime(popupDuration);

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        hidePopupCoroutine = null;
    }

    private void HandleWaveEnded(WaveData wave)
    {
        if (hidePopupCoroutine != null)
        {
            StopCoroutine(hidePopupCoroutine);
            hidePopupCoroutine = null;
        }


        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        if (skipButton != null)
        {
            skipButton.interactable = true;
        }
    }
}