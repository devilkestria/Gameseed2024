using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class InteractionBoard : MonoBehaviour, IInteractable
{
    [FoldoutGroup("Interaction Board")][SerializeField] private UiBoard uiBoard;
    [FoldoutGroup("Interaction Board")][SerializeField] private bool haveImage;
    [FoldoutGroup("Interaction Board")][ShowIf("$haveImage")][SerializeField] private Sprite sprInfo;
    [FoldoutGroup("Interaction Board")][SerializeField] private bool haveText;
    [FoldoutGroup("Interaction Board")][ShowIf("$haveText")][TextArea(3, 10)][SerializeField] private string textBoard;
    [FoldoutGroup("Inetraction Board")][SerializeField] private UnityEvent eventOnInteract;
    private void Start()
    {
        if (!uiBoard) uiBoard = GameplayManager.instance.uiBoard;
    }
    public void Interact()
    {
        if (haveImage && !haveText)
            uiBoard.OpenBoard(sprInfo);
        else if (!haveImage && haveText)
            uiBoard.OpenBoard(textBoard);
        else if (haveImage && haveText)
            uiBoard.OpenBoard(sprInfo, textBoard);
        eventOnInteract?.Invoke();
    }
}
