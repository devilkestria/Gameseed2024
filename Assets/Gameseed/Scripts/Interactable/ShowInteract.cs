using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Outlinable))]
public class ShowInteract : MonoBehaviour
{
    [SerializeField] private Outlinable outlinable;
    [SerializeField] private float interactRadius;
    [SerializeField] private float interactDistance;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private GameObject objInfoInteract;
    [SerializeField] private Vector3 offsetSpherecast;
    private void Start()
    {
        if(!outlinable) outlinable = GetComponent<Outlinable>();
    }
    void Update()
    {
        if (Physics.SphereCast(transform.position + offsetSpherecast, interactRadius, transform.forward, out RaycastHit hit, interactDistance, interactLayer))
        {
            outlinable.enabled = true;
            objInfoInteract.SetActive(true);
        }
        else
        {
            outlinable.enabled = false;
            objInfoInteract.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + offsetSpherecast, transform.position + transform.forward * interactDistance);
        Gizmos.DrawWireSphere(transform.position + transform.forward * interactDistance, interactRadius);
    }
}
