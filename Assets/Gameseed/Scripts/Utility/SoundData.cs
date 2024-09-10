using System;
using UnityEngine;
[Serializable]
public struct MusicData
{
    public string soundName;
    public float timeDuration;
    public AudioClip audioClip;
    public MusicData(string name, float time, AudioClip clip)
    {
        soundName = name;
        timeDuration = time;
        audioClip = clip;
    }
}
