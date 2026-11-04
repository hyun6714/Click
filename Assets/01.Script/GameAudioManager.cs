using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class GameAudioManager : MonoBehaviour
{
    [Header("사운드 클립")]
    [SerializeField] private AudioClip resourceGatherClip;
    [SerializeField] private AudioClip buildingPlaceClip;
    [SerializeField] private AudioClip buildingDemolishClip;
    [SerializeField] private AudioClip buildingHitClip;
    [SerializeField] private AudioClip enemyAttackClip;
    [SerializeField] private AudioClip enemyHitClip;
    [SerializeField] private AudioClip waveStartClip;
    [SerializeField] private AudioClip waveClearClip;
    [SerializeField] private AudioClip rainClip;
    [SerializeField] private AudioClip stormClip;
    [SerializeField] private AudioClip snowClip;
    [SerializeField] private AudioClip resourceBuildingGatherClip;
    [SerializeField] private AudioClip trapActivateClip;
    [SerializeField] private AudioClip bombExplodeClip;
    [SerializeField] private AudioClip resourceInsufficientClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip gameClearClip;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip buildingUnlockClip;

    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        EventManager.OnSoundRequested += HandleSoundRequested;
        EventManager.OnWaveStarted += HandleWaveStarted;
        EventManager.OnWaveEnded += HandleWaveEnded;
        EventManager.OnWeatherChanged += HandleWeatherChanged;
        EventManager.OnCastleDestroyed += HandleGameOver;
        EventManager.OnGameCleared += HandleGameClear;
    }

    private void OnDisable()
    {
        EventManager.OnSoundRequested -= HandleSoundRequested;
        EventManager.OnWaveStarted -= HandleWaveStarted;
        EventManager.OnWaveEnded -= HandleWaveEnded;
        EventManager.OnWeatherChanged -= HandleWeatherChanged;
        EventManager.OnCastleDestroyed -= HandleGameOver;
        EventManager.OnGameCleared -= HandleGameClear;
    }

    private void HandleWaveStarted(WaveData wave)
    {
        PlayClip(waveStartClip);
    }

    private void HandleWaveEnded(WaveData wave)
    {
        PlayClip(waveClearClip);
    }

    private void HandleWeatherChanged(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Rain:
                PlayClip(rainClip);
                break;
            case WeatherType.Storm:
                PlayClip(stormClip);
                break;
            case WeatherType.Snow:
                PlayClip(snowClip);
                break;
        }
    }

    private void HandleSoundRequested(SoundCue cue)
    {
        switch (cue)
        {
            case SoundCue.ResourceGather:
                PlayClip(resourceGatherClip); 
                break;

            case SoundCue.BuildingPlace:
                PlayClip(buildingPlaceClip); 
                break;

            case SoundCue.BuildingDemolish: 
                PlayClip(buildingDemolishClip);
                break;

            case SoundCue.BuildingHit: 
                PlayClip(buildingHitClip); 
                break;

            case SoundCue.EnemyAttack: 
                PlayClip(enemyAttackClip); 
                break;

            case SoundCue.EnemyHit: 
                PlayClip(enemyHitClip); 
                break;

            case SoundCue.ResourceBuildingGather:
                PlayClip(resourceBuildingGatherClip);
                break;

            case SoundCue.TrapActivate:
                PlayClip(trapActivateClip);
                break;

            case SoundCue.BombExplode:
                PlayClip(bombExplodeClip);
                break;

            case SoundCue.ResourceInsufficient:
                PlayClip(resourceInsufficientClip);
                break;

            case SoundCue.BuildingUnlock:
                PlayClip(buildingUnlockClip);
                break;

            case SoundCue.ButtonClick:
                PlayClip(buttonClickClip);
                break;
        }
    }

    private void HandleGameOver()
    {
        PlayClip(gameOverClip);
    }

    private void HandleGameClear()
    {
        PlayClip(gameClearClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}