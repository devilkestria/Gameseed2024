using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour, IInteractable
{
    [FoldoutGroup("Change Weapon")][SerializeField] private AttackObject attackObject;
    [FoldoutGroup("Change Weapon")][SerializeField] private float timePrepareAttack;
    [FoldoutGroup("Change Weapon")][SerializeField] private float timeFinishAttack;
    [FoldoutGroup("Change Weapon")][SerializeField] private BasicPlayerController playerController;
    [FoldoutGroup("Show Interact")][SerializeField] private float interactRadius;
    [FoldoutGroup("Show Interact")][SerializeField] private LayerMask layerInteract;
    [FoldoutGroup("Show Interact")][SerializeField] private GameObject objInteract;
    private void Start()
    {
        if (!playerController) playerController = GameplayManager.instance.playerObj.GetComponent<BasicPlayerController>();
    }

    public void Interact()
    {
        playerController.OnChangeAttack(attackObject, timePrepareAttack, timeFinishAttack);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRadius, layerInteract);
        if (colliders.Length > 0)
        {
            if (colliders[0].CompareTag("Player"))
            {
                objInteract.SetActive(true);
                return;
            }
        }
        objInteract.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
