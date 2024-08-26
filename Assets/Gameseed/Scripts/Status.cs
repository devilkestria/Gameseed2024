using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Status : MonoBehaviour
{
    [FoldoutGroup("Status")][SerializeField] private float Health;
    [FoldoutGroup("Status")][SerializeField] private Rigidbody rb;
    public void OnDamage(AttackObject atkobj)
    {
        Debug.Log("Damage :" + atkobj.damageValue);
        rb.AddForce(atkobj.forcePush * atkobj.transform.forward, ForceMode.Impulse);
    }
}
