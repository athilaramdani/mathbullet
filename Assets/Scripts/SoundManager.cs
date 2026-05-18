using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Clips")]
    public AudioClip shootSound;        // shoot.wav
    public AudioClip correctSound;      // benar.wav
    public AudioClip freezeSound;       // freezesound.wav
    public AudioClip ostMusic;          // ost1.wav
    public AudioClip[] zombieDeadSounds; // zombiedead1-4.ogg

    [Header("Volume")]
    [Range(0f, 1f)] public float sfxVolume   = 0.8f;
    [Range(0f, 1f)] public float musicVolume = 0.45f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        var sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)       { musicSource = sources[0]; sfxSource = sources[1]; }
        else if (sources.Length == 1)  { musicSource = sources[0]; sfxSource = gameObject.AddComponent<AudioSource>(); }
        else { musicSource = gameObject.AddComponent<AudioSource>(); sfxSource = gameObject.AddComponent<AudioSource>(); }

        musicSource.loop = true; musicSource.playOnAwake = false; musicSource.volume = musicVolume;
        musicSource.spatialBlend = 0f; // 2D audio

        sfxSource.loop   = false; sfxSource.playOnAwake  = false; sfxSource.volume   = sfxVolume;
        sfxSource.spatialBlend = 0f; // 2D audio - PENTING agar suara tidak hilang saat kamera jauh
    }

    void Start() => PlayMusic();

    public void PlayMusic()
    {
        if (ostMusic == null) return;
        musicSource.clip = ostMusic;
        musicSource.Play();
    }

    public void PlayShoot()        => PlaySFX(shootSound);
    public void PlayCorrect()      => PlaySFX(correctSound);
    public void PlayFreeze()       => PlaySFX(freezeSound);

    public void PlayZombieDead()
    {
        if (zombieDeadSounds == null || zombieDeadSounds.Length == 0)
        {
            Debug.LogWarning("[Sound] zombieDeadSounds kosong! Jalankan SETUP GAME ulang.");
            return;
        }
        var clip = zombieDeadSounds[Random.Range(0, zombieDeadSounds.Length)];
        if (clip == null) return;
        // PlayClipAtPoint agar tidak tergantung lifecycle enemy (langsung Destroy)
        AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : Vector3.zero, sfxVolume);
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}
