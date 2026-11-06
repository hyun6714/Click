using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    [SerializeField, Range(0f, 1f)] private float defaultVolume = 0.5f;

    private AudioSource musicSource;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
 

        SaveData data = SaveLoadManager.LoadGame();
        float targetVolume = (data != null) ? data.bgmVolume : defaultVolume;
        musicSource.volume = targetVolume;
    }

    private void Start()
    {
        if (musicClip == null)
        {
            return;
        }

        musicSource.clip = musicClip;
        musicSource.Play();
    }
}