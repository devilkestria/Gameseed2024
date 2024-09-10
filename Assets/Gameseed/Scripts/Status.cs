using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Status : MonoBehaviour, IDamageable
{
    [FoldoutGroup("Status")][SerializeField] private float Health;
    [FoldoutGroup("Status")][SerializeField] private float MaxHealth;
    [FoldoutGroup("Status")][SerializeField] private bool isInvicible;
    public void ResetHealth()
    {
        Health = MaxHealth;
    }
    public void RestoreHealth(float value)
    {
        Health += value;
    }
    public void Damage(AttackObject atkObj, AudioClip audioClip)
    {
        if (isInvicible) return;
        Health -= atkObj.damageValue;
        if (Health < 0)
            Death();
    }
    public void Death()
    {
        GetComponent<IDeathable>().Death();
    }
}
