using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
public class AttackObject : MonoBehaviour
{
    [FoldoutGroup("Attack Object")] public bool onActive = false;
    [FoldoutGroup("Attack Object")] public string nameAtk;
    [FoldoutGroup("Attack Object")] public float damageValue;
    [FoldoutGroup("Attack Object")][SerializeField] private float timeDisapear;
    [FoldoutGroup("Attack Object")] private float deltaTimeDisapear;
    [FoldoutGroup("Attack Object")][SerializeField] private Vector3 forwardDirection;
    [FoldoutGroup("Attack Object")][SerializeField] private float forceForward;
    [FoldoutGroup("Attack Object")] public float forcePush;
    [FoldoutGroup("Attack Object")][SerializeField] private bool isPenetration;
    private void OnEnable()
    {
        onActive = true;
        deltaTimeDisapear = 0;
    }
    private void Update()
    {
        if (onActive)
        {
            if (forceForward > 0)
            {
                transform.Translate(forwardDirection * forceForward * Time.deltaTime);
            }
            if (deltaTimeDisapear < timeDisapear)
            {
                deltaTimeDisapear += Time.deltaTime;
            }
            else
            {
                onActive = false;
                gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Utility.CheckHitable(tag, other.tag))
        {
            IDamageable[] dmgables = other.GetComponents<IDamageable>();
            for (int i = 0; i < dmgables.Length; i++)
                dmgables[i].Damage(this);
            if (!isPenetration)
            {
                onActive = false;
                gameObject.SetActive(false);
            }
        }
    }
}
