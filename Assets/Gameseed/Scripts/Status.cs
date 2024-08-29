using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Status : MonoBehaviour, IDamageable
{
    [FoldoutGroup("Status")][SerializeField] private float Health;
    [FoldoutGroup("Status")][SerializeField] private bool isInvicible;
    public void RestoreHealth(float value)
    {
        Health += value;
    }
    public void Damage(AttackObject atkObj)
    {
        if (isInvicible) return;
        Health -= atkObj.damageValue;
        if (Health < 0)
            Death();
    }
    public void Death()
    {
        gameObject.SetActive(false);
    }
}
