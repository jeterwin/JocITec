using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Required for Slider
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Video Settings")]
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    void Start()
    {
        // 1. Set Sliders and Volume to 0.5 on Start
        masterSlider.value = 0.5f;
        musicSlider.value = 0.5f;
        sfxSlider.value = 0.5f;

        SetMasterVolume(0.5f);
        SetMusicVolume(0.5f);
        SetSFXVolume(0.5f);

        // 2. Setup Resolution Dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentResIndex = i;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetMasterVolume(float vol) => audioMixer.SetFloat("MasterVol", Mathf.Log10(vol) * 20);
    public void SetMusicVolume(float vol) => audioMixer.SetFloat("MusicVol", Mathf.Log10(vol) * 20);
    public void SetSFXVolume(float vol) => audioMixer.SetFloat("SFXVol", Mathf.Log10(vol) * 20);

    public void SetQuality(int index) => QualitySettings.SetQualityLevel(index);
    public void SetFullscreen(bool isFull) => Screen.fullScreen = isFull;
    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}