using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Outlinable))]
public class ChangeWeapon : MonoBehaviour, IInteractable
{
    [FoldoutGroup("Change Weapon")][SerializeField] private AttackObject attackObject;
    [FoldoutGroup("Change Weapon")][SerializeField] private float timePrepareAttack;
    [FoldoutGroup("Change Weapon")][SerializeField] private float timeFinishAttack;
    [FoldoutGroup("Change Weapon")][SerializeField] private List<AudioClip> listsfxAtk;
    [FoldoutGroup("Change Weapon")][SerializeField] private BasicPlayerController playerController;
    [FoldoutGroup("Show Interact")][SerializeField] private float interactRadius;
    [FoldoutGroup("Show Interact")][SerializeField] private LayerMask layerInteract;
    [FoldoutGroup("Show Interact")][SerializeField] private GameObject objInteract;
    [FoldoutGroup("Show Interact")][SerializeField] private Outlinable outlinable;
    [FoldoutGroup("Show Interact")] public UnityEvent eventInteract;
    private void Start()
    {
        if (!outlinable) outlinable = GetComponent<Outlinable>();
        if (!playerController) playerController = GameplayManager.instance.playerObj.GetComponent<BasicPlayerController>();
    }

    public void Interact()
    {
        playerController.OnChangeAttack(attackObject, timePrepareAttack, timeFinishAttack, listsfxAtk);
        gameObject.SetActive(false);
        eventInteract?.Invoke();
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
