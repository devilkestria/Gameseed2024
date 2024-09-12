using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [FoldoutGroup("Main Menu")][SerializeField] private StoryProgressManager storyManager;
    [FoldoutGroup("Main Menu")][SerializeField] private GameObject panelMainMenu;
    [FoldoutGroup("Main Menu")][SerializeField] private GameObject panelOption;
    [FoldoutGroup("Main Menu")][SerializeField] private Button firstButtonMainMenu;
    [FoldoutGroup("Main Menu")][SerializeField] CinemachineVirtualCamera vcCameraMain;
    [FoldoutGroup("Main Menu")][SerializeField] CinemachineVirtualCamera vcCameraOption;
    public void OpenMainMenu()
    {
        panelMainMenu.SetActive(true);
        panelOption.SetActive(false);
        firstButtonMainMenu.Select();
        vcCameraMain.enabled = true;
    }
    public void ButtonNewGame()
    {
        panelMainMenu.SetActive(false);
        vcCameraMain.enabled = false;
        storyManager.StartGame();
    }
    public void ButtonOption()
    {
        vcCameraMain.enabled = false;
        vcCameraOption.enabled = true;
        panelMainMenu.SetActive(false);
        sliderVolumeBgm.value = soundManager.volumeBgmValue;
        sliderVolumeSfx.value = soundManager.volumeSfxValue;
        panelOption.SetActive(true);
        firstButtonOptions.Select();
    }
    public void ButtonQuitgame()
    {
        Application.Quit();
    }
    [FoldoutGroup("Option")][SerializeField] private SoundManager soundManager;
    [FoldoutGroup("Option")][SerializeField] private Button firstButtonOptions;
    [FoldoutGroup("Option")][SerializeField] private Slider sliderVolumeBgm;
    [FoldoutGroup("Option")][SerializeField] private Slider sliderVolumeSfx;

    public void ButtonBackOption()
    {
        vcCameraOption.enabled = false;
        vcCameraMain.enabled = true;
        panelOption.SetActive(false);
        panelMainMenu.SetActive(true); 
        firstButtonMainMenu.Select();
    }
}
