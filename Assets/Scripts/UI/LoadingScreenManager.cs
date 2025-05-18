using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public TextMeshProUGUI pressAnyKeyText;
    
    public float minimumLoadTime = 1.0f;
    
    private static string targetScene;
    private bool isLoadingComplete = false;
    private bool minimumTimeReached = false;
    
    private void Start()
    {
        if (pressAnyKeyText != null)
            pressAnyKeyText.gameObject.SetActive(false);

        StartCoroutine(LoadTargetScene());
        
        StartCoroutine(MinimumLoadTimeCoroutine());
    }
    
    private IEnumerator LoadTargetScene()
    {
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("No target scene specified for loading!");
            yield break;
        }
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        
        asyncLoad.allowSceneActivation = false;
        
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                
            if (asyncLoad.progress >= 0.9f)
            {
                isLoadingComplete = true;

                if (minimumTimeReached)
                {
                    if (pressAnyKeyText != null)
                        pressAnyKeyText.gameObject.SetActive(true);
                }
                
                break;
            }
            
            yield return null;
        }
        
        yield return new WaitUntil(() => minimumTimeReached && Input.anyKeyDown);

        // activate the scene
        MenuMusicController.instance.StopMusic();
        asyncLoad.allowSceneActivation = true;
    }
    
    private IEnumerator MinimumLoadTimeCoroutine()
    {
        yield return new WaitForSeconds(minimumLoadTime);
        minimumTimeReached = true;
        
        if (isLoadingComplete && pressAnyKeyText != null)
        {
            pressAnyKeyText.gameObject.SetActive(true);
        }
    }
    
    // static method to start loading a scene through the loading screen
    public static void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.LoadScene("LoadingScreen");
    }
}