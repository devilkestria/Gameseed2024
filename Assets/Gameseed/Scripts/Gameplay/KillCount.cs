using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class KillCount : MonoBehaviour, IDeathable
{
    [FoldoutGroup("Kill Count")][SerializeField] KillManager killManager;
    public void Death()
    {
        killManager.CheckKill(this);
    }
}
