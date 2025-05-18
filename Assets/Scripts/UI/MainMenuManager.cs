using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public List<Button> menuButtons;
    public Image controlsImage;
    public AudioMixer mainMixer;

    public InputActionAsset inputActions;
    public GameObject controlsPanel;
    public GameObject rebindingControlPrefab;
    public Transform rebindingContainer;
    public Button showControlsButton;
    public Button hideControlsButton;
    private Dictionary<string, RebindingUIController> rebindingControls = new Dictionary<string, RebindingUIController>();
    
    public Toggle fullscreenToggle;

    public Button hideOptionsButton;

    private void ResetGameState()
    {
        FloorAccessController.isLevelComplete = false;
    }

    private void Start()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        float fxVolume = PlayerPrefs.GetFloat("FXVolume", 1.0f);

        mainMixer.SetFloat("MasterVolume", LinearToDecibel(masterVolume));
        mainMixer.SetFloat("MusicVolume", LinearToDecibel(musicVolume));
        mainMixer.SetFloat("FXVolume", LinearToDecibel(fxVolume));

        InitializeDisplaySettings();
        LoadSavedBindings();
        SetupRebindingControls();
        
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);


        if (menuButtons.Count >= 4)
        {
            menuButtons[0].onClick.AddListener(() =>
            {  // lambda function, have no idea about this syntax
                ResetGameState();
                SceneManager.LoadScene("LevelSelect");
            });

            menuButtons[1].onClick.AddListener(() =>
            {
                ShowOptionsMenu();
            });
            menuButtons[2].onClick.AddListener(() =>
            {
                ShowControlsPanel();
            });
            menuButtons[3].onClick.AddListener(() =>
            {
                QuitGame();
            });
        }

        //back button
        hideOptionsButton.onClick.AddListener(() =>
        {
            HideOptionsMenu();
        });

        hideControlsButton.onClick.AddListener(() =>
        {
            HideControlsPanel();
        });
    }
    private void QuitGame()
    {
        Application.Quit();

        // stops play mode in editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private float LinearToDecibel(float linear)
    {
        if (linear <= 0) return -80f;
        return 20f * Mathf.Log10(linear);
    }

    private void LoadSavedBindings()
    {
        if (PlayerPrefs.HasKey("InputBindings"))
        {
            string rebinds = PlayerPrefs.GetString("InputBindings");
            inputActions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    private void SetupRebindingControls()
    {
        foreach (Transform child in rebindingContainer)
        {
            Destroy(child.gameObject);
        }
        rebindingControls.Clear();

        foreach (var actionMap in inputActions.actionMaps)
        {
            foreach (var action in actionMap.actions)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    var binding = action.bindings[i];

                    if (!binding.path.StartsWith("<Keyboard>"))
                        continue;

                    GameObject controlObj = Instantiate(rebindingControlPrefab, rebindingContainer);
                    RebindingUIController controller = controlObj.GetComponent<RebindingUIController>();

                    string actionName = action.name + (binding.isPartOfComposite ? " " + binding.name : "");
                    controller.Initialize(action, i, actionName);

                    rebindingControls[action.id.ToString() + i] = controller;
                }
            }
        }
    }

    private void InitializeDisplaySettings()
    {

    bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
    fullscreenToggle.isOn = isFullscreen;
    SetFullscreen(isFullscreen);
    fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void SetFullscreen(bool isFullscreen)
    {
    PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    PlayerPrefs.Save();
    
    Resolution currentResolution = Screen.currentResolution;
    
    if (isFullscreen)
    {
        Screen.SetResolution(currentResolution.width, currentResolution.height, FullScreenMode.FullScreenWindow);
    }
    else
        {
            int windowedWidth = Mathf.RoundToInt(currentResolution.width * 0.9f);
            int windowedHeight = Mathf.RoundToInt(currentResolution.height * 0.9f);
            Screen.SetResolution(windowedWidth, windowedHeight, FullScreenMode.Windowed);
        }
    }

    private void ShowControlsPanel()
    {
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    private void HideControlsPanel()
    {
        mainMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }
    
    private void ShowOptionsMenu()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    private void HideOptionsMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

}