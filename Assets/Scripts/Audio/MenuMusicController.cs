using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicController : MonoBehaviour
{
    [SerializeField] private AudioSource initialAudioSource; // Assign initial clip (no reverb tail)
    [SerializeField] private AudioSource loopAudioSource;    // Assign looping clip (with reverb tail)

    public static MenuMusicController instance;

    void Awake()
    {
        // Singleton: Keep only one instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    void Start()
    {
        PlayMusic();
    }
    public void PlayMusic()
    {
        // Only play if not already playing
        if (!initialAudioSource.isPlaying && !loopAudioSource.isPlaying)
        {
            loopAudioSource.loop = true;
            initialAudioSource.Play();
            double initialClipLength = initialAudioSource.clip.samples / (double)initialAudioSource.clip.frequency;
            double startTime = AudioSettings.dspTime + initialClipLength;
            loopAudioSource.PlayScheduled(startTime);
        }
    }
    public static void StartMenuMusic(GameObject musicPrefab = null)
    {
        if (instance == null && musicPrefab != null)
        {
            Instantiate(musicPrefab);
        }
        else if (instance != null)
        {
            instance.PlayMusic();
        }
    }
    public void StopMusic()
    {
        initialAudioSource.Stop();
        loopAudioSource.Stop();
        Destroy(gameObject);
    }
}