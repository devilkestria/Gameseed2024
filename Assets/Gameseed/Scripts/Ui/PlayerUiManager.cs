using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiManager : MonoBehaviour
{
    [FoldoutGroup("Player Ui Manager")][SerializeField] BasicPlayerController playerController;
    [FoldoutGroup("Player Ui Manager")][SerializeField] private Status playerStatus;
    [FoldoutGroup("Player Ui Manager")][SerializeField] private UiHealthBar healthBar;
    [FoldoutGroup("Player Ui Manager")][SerializeField] private UiAction actionWoodAtk;
    [FoldoutGroup("Player Ui Manager")][SerializeField] private UiAction actionShovelAtk;
    [FoldoutGroup("Player Ui Manager")][SerializeField] private UiAction actionShovelUse;
    [FoldoutGroup("Player Ui Manager")][SerializeField] private UiAction actionDodge;
    [FoldoutGroup("Player Ui Manager")][SerializeField] private UiAction actionMusicUse;
    [FoldoutGroup("Player Ui Manager")] private int indexNote;
    [FoldoutGroup("Player Ui Manager")][SerializeField] private Image imgNote;
    [FoldoutGroup("Player Ui Manager")][SerializeField] private Animator animNote;
    [FoldoutGroup("Player Ui Manager")][SerializeField] private GameObject panelInputChangeNote;
    [FoldoutGroup("Player Ui Manager")] private int animNoteIDNext;
    [FoldoutGroup("Player Ui Manager")] private int animNoteIDPrevious;
    private void Start()
    {
        if (!playerController) playerController = GameplayManager.instance.playerObj.GetComponent<BasicPlayerController>();
        if (!animNote) animNote = GetComponent<Animator>();
        if (!playerStatus) playerStatus = GameplayManager.instance.playerObj.GetComponent<Status>();
        SetAnimNoteID();
        healthBar.InitHealthBar(playerStatus.Health, playerStatus.MaxHealth);
        playerStatus.eventOnChangeHealth += healthBar.OnHealthChange;
        actionDodge.InitUiAction(playerController.timeDodge);
        playerController.eventActionDodge += actionDodge.StartCooldown;
        playerController.eventOnChangeAttack += OnChangeWeapon;
        playerController.eventOnAddMusic += AddMusic;
        playerController.eventOnChangeMusic += OnChangeMusic;
        actionWoodAtk.gameObject.SetActive(false);
        actionShovelAtk.gameObject.SetActive(false);
        actionShovelUse.gameObject.SetActive(false);
        actionMusicUse.gameObject.SetActive(false);
        panelInputChangeNote.SetActive(false);
        imgNote.gameObject.SetActive(false);
    }
    void OnChangeWeapon(AttackObject attackObject, float time)
    {
        actionWoodAtk.gameObject.SetActive(false);
        actionShovelAtk.gameObject.SetActive(false);
        actionShovelUse.gameObject.SetActive(false);
        if (attackObject == null) return;
        switch (attackObject.nameAtk)
        {
            case "Wood Slash":
                WoodAtkActive(time);
                break;
            case "Shovel Slash":
                ShovelAtkActive(time);
                break;
        }
    }
    void WoodAtkActive(float time)
    {
        actionWoodAtk.InitUiAction(time);
        actionWoodAtk.gameObject.SetActive(true);
        playerController.eventActionAttack = null;
        playerController.eventActionAttack += actionWoodAtk.StartCooldown;

    }
    void ShovelAtkActive(float time)
    {
        actionShovelAtk.InitUiAction(time);
        actionShovelAtk.gameObject.SetActive(true);
        playerController.eventActionAttack = null;
        playerController.eventActionAttack += actionShovelAtk.StartCooldown;
        actionShovelUse.InitUiAction(playerController.timeDurationDigPileUp);
        actionShovelUse.gameObject.SetActive(true);
        playerController.eventActionDigPileUp += actionShovelUse.StartCooldown;
    }
    void AddMusic()
    {
        actionMusicUse.InitUiAction(playerController.timeDurationPlaySound);
        actionMusicUse.gameObject.SetActive(true);
        playerController.eventActionMusic = null;
        playerController.eventActionMusic += actionMusicUse.StartCooldown;
        indexNote = playerController.indexPlayMusic;
        imgNote.gameObject.SetActive(true);
        imgNote.sprite = playerController.listDataMusic[indexNote].sprNote;
        if (playerController.listDataMusic.Count > 1)
            panelInputChangeNote.SetActive(true);
    }
    void OnChangeMusic(bool next)
    {
        animNote.SetTrigger(next ? animNoteIDNext : animNoteIDPrevious);
        indexNote = playerController.indexPlayMusic;
    }
    public void SetNote()
    {
        imgNote.sprite = playerController.listDataMusic[indexNote].sprNote;
    }
    void SetAnimNoteID()
    {
        animNoteIDNext = Animator.StringToHash("Next");
        animNoteIDPrevious = Animator.StringToHash("Previous");
    }
}
