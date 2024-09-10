using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestingInteractWeaponChange : MonoBehaviour, IInteractable
{
    [FoldoutGroup("Testing Weapon Change")][SerializeField] private string WeaponName;
    [FoldoutGroup("Testing Weapon Change")][SerializeField] private GameObject objWeapon;
    [FoldoutGroup("Testing Weapon Change")][SerializeField] private BasicPlayerController playerController;
    private void Start()
    {
        if (!playerController) playerController = GameplayManager.instance.playerObj.GetComponent<BasicPlayerController>();
    }
    public void Interact()
    {
        ResetWeaponChange();
    }
    void ResetWeaponChange()
    {
        objWeapon.SetActive(true);
        playerController.OnChangeAttack(null, 0, 0, null);
    }
}
