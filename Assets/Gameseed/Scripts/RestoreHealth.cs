using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class RestoreHealth : MonoBehaviour
{
    [FoldoutGroup("Restore Health")][SerializeField] float healthPoint = 5;
    [FoldoutGroup("Restore Health")] public bool onActive;
    [FoldoutGroup("Restore Health")][SerializeField] float overalapRadius;
    [FoldoutGroup("Restore Health")][SerializeField] LayerMask layerOverlap;
    [FoldoutGroup("Restore Helath")] public Rigidbody rb;
    private void Awake() {
        if(!rb) rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        onActive = true;
    }
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, overalapRadius, layerOverlap);
        if (colliders.Length > 0)
        {
            Status status = colliders[0].GetComponent<Status>();
            if (status)
            {
                if (!GameplayManager.instance.listRestoreHealth.Contains(this))
                    GameplayManager.instance.listRestoreHealth.Add(this);
                status.RestoreHealth(healthPoint);
                gameObject.SetActive(false);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, overalapRadius);
    }
}
