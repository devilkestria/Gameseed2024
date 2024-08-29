using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class InteractionBoard : MonoBehaviour, IInteractable
{
    [FoldoutGroup("Interaction Board")][SerializeField] private UiBoard uiBoard;
    [FoldoutGroup("Interaction Board")][SerializeField] private Sprite sprInfo;
    [FoldoutGroup("Interaction Board")][TextArea(3,10)][SerializeField] private string textBoard;
    private void Start()
    {
        if (!uiBoard) uiBoard = GameplayManager.instance.uiBoard;
    }
    public void Interact()
    {
        if (!sprInfo) uiBoard.OpenBoard(sprInfo);
        if (textBoard.Length > 0) uiBoard.OpenBoard(textBoard);
    }
}
