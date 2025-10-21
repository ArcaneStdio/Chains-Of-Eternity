using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections.Generic; 
using System.Linq;

public class SettingsManager : MonoBehaviour
{
    [Header("Sound Settings")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Graphics Settings")]
    public TMP_Dropdown shaderQualityDropdown;
    public Toggle vsyncToggle;
    public TMP_Dropdown fpsCapDropdown;
    public Slider brightnessSlider;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fullscreenModeDropdown;
    public Toggle particleEffectsToggle;
    public Toggle bloomToggle;

    [Header("Controls Settings")]
    public TMP_Dropdown inputSupportDropdown;

    private Resolution[] resolutions;

    void Start()
    {
        // resoltuionm
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // fullscreen
        fullscreenModeDropdown.ClearOptions();
        fullscreenModeDropdown.AddOptions(new List<string> { "Exclusive Fullscreen", "Borderless Fullscreen", "Windowed" });
        fullscreenModeDropdown.value = (int)Screen.fullScreenMode;
        fullscreenModeDropdown.RefreshShownValue();
    }


    // soundds
    public void SetMasterVolume(float volume)
    {
        Debug.Log("Master Volume set to: " + volume);
        // TODO: @RONIN@RYUGA
    }

    public void SetMusicVolume(float volume)
    {
        Debug.Log("Music Volume set to: " + volume);
        // TODO: @RONIN@RYUGA
    }

    public void SetSfxVolume(float volume)
    {
        Debug.Log("SFX Volume set to: " + volume);
        // TODO: @RONIN@RYUGA
    }


    // GRAPOHICS
    public void SetShaderQuality(int qualityIndex)
    {
        Debug.Log("Shader Quality set to index: " + qualityIndex);
        // TODO: @RONIN@RYUGA
    }

    public void SetVSync(bool isEnabled)
    {
        Debug.Log("VSync set to: " + isEnabled);
        QualitySettings.vSyncCount = isEnabled ? 1 : 0;
    }

    public void SetFpsCap(int fpsIndex)
    {
        // TODO: @RONIN@RYUGA
        Debug.Log("FPS Cap index selected: " + fpsIndex);
    }
    
    public void SetBrightness(float brightness)
    {
        Debug.Log("Brightness set to: " + brightness);
        // TODO: @RONIN@RYUGA ALSO NEED TO CHECK IF IT WORKS IN ALL PLATFROMS
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        Debug.Log("Resolution set to: " + resolution.width + "x" + resolution.height);
    }
    
    public void SetFullscreenMode(int modeIndex)
    {
        Screen.fullScreenMode = (FullScreenMode)modeIndex;
        Debug.Log("Fullscreen mode set to: " + (FullScreenMode)modeIndex);
    }

    public void SetParticleEffects(bool isEnabled)
    {
        Debug.Log("Particle Effects set to: " + isEnabled);
    }

    public void SetBloom(bool isEnabled)
    {
        Debug.Log("Bloom set to: " + isEnabled);
    }


    // conrols
    public void SetInputSupport(int supportIndex)
    {
        // TODO: @RONIN@RYUGA
        Debug.Log("Input support set to index: " + supportIndex);
    }

    //back
    public void BackToMainMenu()
    {
        // TODO: @RONIN@RYUGA
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
