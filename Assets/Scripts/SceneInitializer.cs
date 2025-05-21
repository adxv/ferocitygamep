using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        CheckCurrentScene();
    }
    
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckCurrentScene();
    }
    
    void CheckCurrentScene()
    {
        GameMusicController.EnsureInstance();
    }
}