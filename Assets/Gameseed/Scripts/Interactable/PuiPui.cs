using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;
[RequireComponent(typeof(Outlinable))]
public class PuiPui : MonoBehaviour
{
    [FoldoutGroup("Change Weapon")][SerializeField] private float timeDurationSound;
    [FoldoutGroup("Change Weapon")][SerializeField] private BasicPlayerController playerController;
    [FoldoutGroup("Show Interact")][SerializeField] private float interactRadius;
    [FoldoutGroup("Show Interact")][SerializeField] private LayerMask layerInteract;
    [FoldoutGroup("Show Interact")][SerializeField] private GameObject objInteract;
    [FoldoutGroup("Show Interact")][SerializeField] private Outlinable outlinable;
    private void Start()
    {
        if (!outlinable) outlinable = GetComponent<Outlinable>();
        if (!playerController) playerController = GameplayManager.instance.playerObj.GetComponent<BasicPlayerController>();
    }

    public void Interact()
    {
        // playerController.OnChangeAttack(attackObject, timePrepareAttack, timeFinishAttack);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRadius, layerInteract);
        if (colliders.Length > 0)
        {
            if (colliders[0].CompareTag("Player"))
            {
                outlinable.enabled = true;
                objInteract.SetActive(true);
                return;
            }
        }
        outlinable.enabled = false;
        objInteract.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

}
