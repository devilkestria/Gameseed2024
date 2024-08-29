using System.Collections;
using System.Collections.Generic;
using JVTMPro;
using JVTMPro.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UiBoard : MonoBehaviour
{
    [FoldoutGroup("Ui Board")][SerializeField] private GameObject panelBoard;
    [FoldoutGroup("Ui Board")][SerializeField] private Image imgBoard;
    [FoldoutGroup("Ui Board")][SerializeField] private JVTextMeshProUGUI txtJava;
    [FoldoutGroup("Ui Board")][SerializeField] private Button btnBoard;
    [FoldoutGroup("Ui Board")] private PlayerInput playerInput;
    private void Start()
    {
        if (playerInput == null) playerInput = GameplayManager.instance.playerObj.GetComponent<PlayerInput>();
    }
    public void OpenBoard(Sprite img)
    {
        playerInput.actions.Disable();
        panelBoard.SetActive(true);
        imgBoard.gameObject.SetActive(true);
        imgBoard.sprite = img;
        btnBoard.Select();
    }
    public void OpenBoard(string text)
    {
        playerInput.actions.Disable();
        panelBoard.SetActive(true);
        txtJava.gameObject.SetActive(true);
        txtJava.text = Transliterator.LatinToJava(text);
        btnBoard.Select();
    }
    public void CloseBoard()
    {
        playerInput.actions.Enable();
        imgBoard.gameObject.SetActive(false);
        txtJava.gameObject.SetActive(false);
        panelBoard.SetActive(false);
    }
}
