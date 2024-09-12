using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class KillManager : MonoBehaviour
{
    [FoldoutGroup("Kill Manager")][SerializeField] private int countKill;
    [FoldoutGroup("Kill Manager")][SerializeField] private int targetKill;
    [FoldoutGroup("Kill Manager")] public UnityEvent eventFinishKill;
    public void InitKill()
    {
        countKill = 0;
    }
    public void CheckKill(KillCount script)
    {
        countKill += 1;
        Destroy(script);
        if (countKill >= targetKill)
            eventFinishKill?.Invoke();
    }
}
