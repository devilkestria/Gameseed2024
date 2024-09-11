using System;
using UnityEngine;
[Serializable]
public struct MusicData
{
    public string soundName;
    public float timeDuration;
    public AudioClip audioClip;
    public Sprite sprNote;
    public MusicData(string name, float time, AudioClip clip, Sprite sprite)
    {
        soundName = name;
        timeDuration = time;
        audioClip = clip;
        sprNote = sprite;
    }
}
