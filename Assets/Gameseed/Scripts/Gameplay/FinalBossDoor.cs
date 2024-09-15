using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class FinalBossDoor : MonoBehaviour, IMusicable
{
    [FoldoutGroup("Final Boss Door")]public UnityEvent eventOpenDoor;
    public void Music(MusicData data)
    {
        Debug.Log("Music Dorr Boss");
        if(data.soundName != "Music 1") return;
        eventOpenDoor?.Invoke();
    }
}
