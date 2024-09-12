using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Events;

public class MoveArea : MonoBehaviour
{
    [FoldoutGroup("Move Area")][SerializeField] private BlackscreenManager bsManager;
    [FoldoutGroup("Move Area")][SerializeField] private BasicPlayerController playerController;
    [FoldoutGroup("Move Area")][SerializeField] private Transform transPlayer;
    [FoldoutGroup("Move Area")][SerializeField] private Collider colliderTrigger;
    [FoldoutGroup("Move Area")][SerializeField] private bool isOPen;
    [FoldoutGroup("Move Area")][SerializeField] private Transform transSpawn;
    [FoldoutGroup("Move Area")] public UnityEvent eventOnMove;
    void Start()
    {
        if (!bsManager) bsManager = GameplayManager.instance.bsManager;
        if (!playerController) playerController = GameplayManager.instance.playerObj.GetComponent<BasicPlayerController>();
        if (!transPlayer) transPlayer = playerController.transform;
        if (!colliderTrigger) colliderTrigger = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isOPen)
        {
            playerController.ChangeState(PlayerState.PlayerIddle);
            bsManager.eventOnFadeIn += OnMove;
            bsManager.FadeIn();
        }
    }
    void OnMove()
    {
        bsManager.eventOnFadeIn -= OnMove;
        transPlayer.gameObject.SetActive(false);
        transPlayer.position = transSpawn.position;
        transPlayer.rotation = transSpawn.rotation;
        eventOnMove?.Invoke();
        bsManager.eventOnFadeOut += PlayerMove;
        bsManager.FadeOut();
    }
    void PlayerMove()
    {
        bsManager.eventOnFadeOut -= PlayerMove;
        playerController.ChangeState(PlayerState.PlayerMoving);
    }
    public void SetIsOpen(bool value)
    {
        isOPen = value;
        colliderTrigger.isTrigger = value;
    }
}
