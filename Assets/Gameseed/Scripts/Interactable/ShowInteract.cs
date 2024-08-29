using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInteract : MonoBehaviour
{
    [SerializeField] private float interactRadius;
    [SerializeField] private float interactDistance;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private GameObject objInfoInteract;
    [SerializeField] private Vector3 offsetSpherecast;
    void Update()
    {
        if (Physics.SphereCast(transform.position + offsetSpherecast, interactRadius, transform.forward, out RaycastHit hit, interactDistance, interactLayer))
            objInfoInteract.SetActive(true);
        else
            objInfoInteract.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + offsetSpherecast, transform.position + transform.forward * interactDistance);
        Gizmos.DrawWireSphere(transform.position + transform.forward * interactDistance, interactRadius);
    }
}
