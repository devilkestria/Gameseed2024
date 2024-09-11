using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Status : MonoBehaviour, IDamageable
{
    [FoldoutGroup("Status")] public float Health;
    [FoldoutGroup("Status")] public float MaxHealth;
    [FoldoutGroup("Status")][SerializeField] private bool isInvicible;
    [FoldoutGroup("Status")] public UnityAction<float, float> eventOnChangeHealth;
    public void ResetHealth()
    {
        Health = MaxHealth;
        eventOnChangeHealth?.Invoke(Health, MaxHealth);
    }
    public void RestoreHealth(float value)
    {
        Health += value;
        Health = Health > MaxHealth ? MaxHealth : Health;
        eventOnChangeHealth?.Invoke(Health, MaxHealth);
    }
    public void Damage(AttackObject atkObj, AudioClip audioClip)
    {
        if (isInvicible) return;
        Health -= atkObj.damageValue;
        eventOnChangeHealth?.Invoke(Health, MaxHealth);
        if (Health < 0)
            Death();
    }
    public void Death()
    {
        GetComponent<IDeathable>().Death();
    }
}
