using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [FoldoutGroup("Power Up")][SerializeField] private AttackObject attackObject;
    [FoldoutGroup("Power Up")][SerializeField] private float timePrepareAttack;
    [FoldoutGroup("Power Up")][SerializeField] private float timeFinishAttack;
    [FoldoutGroup("Power Up")][SerializeField] private List<AudioClip> listAudioAttack;
    [FoldoutGroup("Power Up")][SerializeField] private float timeTemporaryChange;
    [FoldoutGroup("Power Up")] public bool onActive;
    [FoldoutGroup("Power Up")][SerializeField] float overalapRadius;
    [FoldoutGroup("Power Up")][SerializeField] LayerMask layerOverlap;
    private void OnEnable()
    {
        onActive = true;
    }
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, overalapRadius, layerOverlap);
        if (colliders.Length > 0)
        {
            BasicPlayerController playerController = colliders[0].GetComponent<BasicPlayerController>();
            if (playerController)
            {
                if (!GameplayManager.instance.listPowerUp.Contains(this))
                    GameplayManager.instance.listPowerUp.Add(this);
                playerController.OnTemporaryChangeAttack(attackObject, timePrepareAttack, timeFinishAttack, timeTemporaryChange, listAudioAttack);
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
