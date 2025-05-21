using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameMusicController : MonoBehaviour
{
    [SerializeField] private AudioSource initialAudioSource;
    [SerializeField] private AudioSource loopAudioSource;   
    
    [Header("Auto-Instantiation")]
    [SerializeField] private string mapScenePrefix = "map_";
    
    public static GameMusicController instance;
    private string currentScene;
    
    public static void EnsureInstance()
    {
        if (instance != null) return;
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName.StartsWith("map_"))
        {
            GameObject prefab = Resources.Load<GameObject>("GameMusic");
            if (prefab != null)
            {
                Instantiate(prefab);
            }
            else
            {
                Debug.LogError("GameMusic prefab not found in Resources folder!");
            }
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            currentScene = SceneManager.GetActiveScene().name;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        PlayGameMusic();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newScene = scene.name;

        if (!newScene.StartsWith(mapScenePrefix))
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
            return;
        }
        
        currentScene = newScene;
    }
    
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        if (instance == this)
        {
            instance = null;
        }
    }
    
    public void PlayGameMusic()
    {
        if (!initialAudioSource.isPlaying && !loopAudioSource.isPlaying)
        {
            loopAudioSource.loop = true;
            initialAudioSource.Play();

            double initialClipLength = initialAudioSource.clip.samples / (double)initialAudioSource.clip.frequency;
            double startTime = AudioSettings.dspTime + initialClipLength;
            loopAudioSource.PlayScheduled(startTime);
        }
    }
    
    public void StopGameMusic()
    {
        initialAudioSource.Stop();
        loopAudioSource.Stop();
    }
}