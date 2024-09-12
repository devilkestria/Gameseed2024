using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

public class StoryProgressManager : MonoBehaviour
{
    [FoldoutGroup("Story Progress Manager")][SerializeField] private MainMenu mainMenu;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private BlackscreenManager bsManager;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private int IndexProgress;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private Transform transPlayerCameraZoom;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private float timeWaitGotItem;
    [FoldoutGroup("Stroy Progress Manager")] WaitForSeconds wfsTimeWaitGotItem;
    void Awake()
    {
        wfsTimeWaitGotItem = new WaitForSeconds(timeWaitGotItem);
    }
    void Start()
    {
        playerController.ChangeState(PlayerState.PlayerIddle);
        playerUiManager.gameObject.SetActive(false);
        mainMenu.OpenMainMenu();
    }
    public void ContinueGame()
    {
        switch (IndexProgress)
        {
            case 1:
                bsManager.eventOnFadeIn += ContinueProgressChapter1;
                bsManager.FadeIn();
                break;
        }
    }
    #region Player
    [FoldoutGroup("Player")][SerializeField] private PlayerUiManager playerUiManager;
    [FoldoutGroup("Player")][SerializeField] private Status playerStatus;
    [FoldoutGroup("Player")][SerializeField] private BasicPlayerController playerController;
    [FoldoutGroup("Player")][SerializeField] private Transform transPlayer;
    void PlayerMove()
    {
        bsManager.eventOnFadeOut -= PlayerMove;
        playerController.ChangeState(PlayerState.PlayerMoving);
        playerUiManager.gameObject.SetActive(true);
    }
    #endregion
    #region Timeline Story
    [FoldoutGroup("Start Game")][SerializeField] PlayableDirector playableDirector;
    [FoldoutGroup("Start Game")][SerializeField] GameObject objGateStartEffect;
    [FoldoutGroup("start Game")][SerializeField] GameObject objCameraTimeline;
    public void StartGame()
    {
        playableDirector.Play();
    }
    public void TimelineEndStartGame()
    {
        playableDirector.Stop();
        bsManager.eventOnFadeIn += FadeInTimelineStartGame;
        bsManager.FadeIn();
    }
    void FadeInTimelineStartGame()
    {
        bsManager.eventOnFadeIn -= FadeInTimelineStartGame;
        objGateStartEffect.SetActive(false);
        objCameraTimeline.SetActive(false);
        bsManager.eventOnFadeOut += PlayerMove;
        bsManager.FadeOut();
    }
    #endregion
    #region Chapter 1 : Arrive
    [FoldoutGroup("Chapter 1 : Arrive")][SerializeField] private bool chapterArrive;
    [FoldoutGroup("Chapter 1 : Arrive")][SerializeField] Transform transZ1Spawn;
    [FoldoutGroup("Chapter 1 : Arrive")][SerializeField] GameObject objWoodStick;
    [FoldoutGroup("Chapter 1 : Arrive")][SerializeField] GameObject objShovel;
    [FoldoutGroup("Chapter 1 : Arrive")][SerializeField] bool GotShovel;

    void ContinueProgressChapter1()
    {
        bsManager.eventOnFadeIn -= ContinueProgressChapter1;
        transPlayer.position = transZ1Spawn.position;
        transPlayer.rotation = transZ1Spawn.rotation;
        playerStatus.ResetHealth();
        playerController.Revive();
        bsManager.eventOnFadeOut += PlayerMove;
        bsManager.FadeOut();
    }
    public void GotWoodStick()
    {
        playerController.ChangeState(PlayerState.PlayerIddle);
        playerController.CamOnGetItem(true);
        StartCoroutine(IeGotWoodStick());
    }
    IEnumerator IeGotWoodStick()
    {
        yield return wfsTimeWaitGotItem;
        playerController.PlayerOnGetItem(true);
        objWoodStick.transform.position = playerController.transGrab.position;
        objWoodStick.transform.LookAt(transPlayerCameraZoom, Vector3.up);
        objWoodStick.SetActive(true);
        PlayGotItem();
        yield return wfsTimeWaitGotItem;
        objWoodStick.SetActive(false);
        playerController.PlayerOnGetItem(false);
        playerController.CamOnGetItem(false);
        yield return wfsTimeWaitGotItem;
        playerController.ChangeState(PlayerState.PlayerMoving);
    }
    public void OpenShovelArea()
    {

    }
    #endregion

    #region SFX Region
    [FoldoutGroup("SFX Region")][SerializeField] private AudioSource sfxAudio;
    [FoldoutGroup("SFX Region")][SerializeField] private AudioClip clipGotItem;
    void PlayGotItem()
    {
        sfxAudio.PlayOneShot(clipGotItem);
    }
    #endregion
}
