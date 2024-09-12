using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [FoldoutGroup("Sound Manager")][SerializeField] private AudioMixer audioMixer;
    [FoldoutGroup("Sound Manager")] public float volumeMasterValue;
    [FoldoutGroup("Sound Manager")] public float volumeBgmValue;
    [FoldoutGroup("Sound Manager")] public float volumeSfxValue;
    [FoldoutGroup("Sound Manager")] const string VOLUME_MASTER = "Volume_Master";
    [FoldoutGroup("Sound Manager")] const string VOLUME_SFX = "Volume_Sfx";
    [FoldoutGroup("Sound Manager")] const string VOLUME_BGM = "Volume_Bgm";

    void Awake()
    {
        volumeMasterValue = PlayerPrefs.GetFloat(VOLUME_MASTER, 100);
        volumeBgmValue = PlayerPrefs.GetFloat(VOLUME_BGM, 60);
        volumeSfxValue = PlayerPrefs.GetFloat(VOLUME_SFX, 60);
        SetVolumeMaster(volumeMasterValue);
        SetVolumeBGM(volumeBgmValue);
        SetVolumeSfx(volumeSfxValue);
    }

    public void SetVolumeMaster(float volume)
    {
        float value = volume / 100f;
        audioMixer.SetFloat(VOLUME_MASTER, MathF.Log10(value) * 20);
        PlayerPrefs.SetFloat(VOLUME_MASTER, volume);
    }
    public void SetVolumeBGM(float volume)
    {
        float value = volume / 100f;
        audioMixer.SetFloat(VOLUME_BGM, MathF.Log10(value) * 20);
        PlayerPrefs.SetFloat(VOLUME_BGM, volume);
    }
    public void SetVolumeSfx(float volume)
    {
        float value = volume / 100f;
        audioMixer.SetFloat(VOLUME_SFX, MathF.Log10(value) * 20);
        PlayerPrefs.SetFloat(VOLUME_SFX, volume);
    }
}
