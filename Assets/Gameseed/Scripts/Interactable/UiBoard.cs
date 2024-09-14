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
    [FoldoutGroup("Ui Board")][SerializeField] private BasicPlayerController playerController;
    private void Start()
    {
        if (playerController == null) playerController = GameplayManager.instance.playerObj.GetComponent<BasicPlayerController>();
    }
    public void OpenBoard(Sprite img)
    {
        playerController.ChangeState(PlayerState.PlayerIddle);
        panelBoard.SetActive(true);
        imgBoard.gameObject.SetActive(true);
        imgBoard.sprite = img;
        btnBoard.Select();
    }
    public void OpenBoard(string text)
    {
        playerController.ChangeState(PlayerState.PlayerIddle);
        panelBoard.SetActive(true);
        txtJava.gameObject.SetActive(true);
        txtJava.text = Transliterator.LatinToJava(text);
        btnBoard.Select();
    }
    public void OpenBoard(Sprite img, string text)
    {
        playerController.ChangeState(PlayerState.PlayerIddle);
        panelBoard.SetActive(true);
        imgBoard.gameObject.SetActive(true);
        imgBoard.sprite = img;
        txtJava.gameObject.SetActive(true);
        txtJava.text = Transliterator.LatinToJava(text);
        btnBoard.Select();
    }
    public void CloseBoard()
    {
        playerController.ChangeState(PlayerState.PlayerMoving);
        imgBoard.gameObject.SetActive(false);
        txtJava.gameObject.SetActive(false);
        panelBoard.SetActive(false);
    }
}
