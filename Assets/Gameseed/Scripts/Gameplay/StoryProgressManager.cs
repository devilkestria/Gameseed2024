using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

public class StoryProgressManager : MonoBehaviour
{
    [FoldoutGroup("Story Progress Manager")][SerializeField] private MainMenu mainMenu;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private BlackscreenManager bsManager;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private GridManagement gridManagement;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private int IndexProgress;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private Transform transPlayerCameraZoom;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private float timeTransitionCamera;
    [FoldoutGroup("Stroy Progress Manager")] WaitForSeconds wfsTimeTransitioncamera;
    void Awake()
    {
        wfsTimeTransitioncamera = new WaitForSeconds(timeTransitionCamera);
    }
    void Start()
    {
        bsManager.FadeOut();
        playerController.ChangeState(PlayerState.PlayerIddle);
        playerUiManager.gameObject.SetActive(false);
        PlayBgm(clipMainMenu);
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
    [FoldoutGroup("Start Game")][SerializeField] GameObject objCameraTimeline;
    [FoldoutGroup("Start Game")][SerializeField] GameObject objSoundTimeline;
    public void StartGame()
    {
        BgmAudio.Stop();
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
        PlayBgm(clipField);
        bsManager.eventOnFadeOut += PlayerMove;
        bsManager.FadeOut();
    }
    #endregion
    #region Chapter 1 : Arrive
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private Transform transZ1Spawn;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private GameObject objWoodStick;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private CinemachineVirtualCamera vcOpenShovelArea;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private GameObject objShovel;
    [FoldoutGroup("Chapter 1 : Arrival")] bool isPlayerGotShovel = false;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private GameObject objPrisonShovelArea;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private float StartHeightPrisonShovelArea;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private float FinalHeightPrisonShovelArea;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private CinemachineVirtualCamera vcPrisonShovelArea;
    [FoldoutGroup("Chapter 1 : Arrival")] private float deltaTimeShovelArea;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private List<Vector3> listAreaDigKeyPrisonShovelArea;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private CinemachineVirtualCamera vcFindKeyPrisonShovelArea;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private GameObject objKeyPrisonShovelArea;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private float StartHeightKeyPrison;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private float FinalHeightKeyPrison;

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
    #region Got Wood Stick
    public void GotWoodStick()
    {
        playerController.ChangeState(PlayerState.PlayerIddle);
        playerController.CamOnGetItem(true);
        StartCoroutine(IeGotWoodStick());
    }
    IEnumerator IeGotWoodStick()
    {
        yield return wfsTimeTransitioncamera;
        playerController.PlayerOnGetItem(true);
        objWoodStick.transform.position = playerController.transGrab.position;
        objWoodStick.transform.LookAt(transPlayerCameraZoom, Vector3.up);
        objWoodStick.SetActive(true);
        PlayGotItem();
        yield return wfsTimeTransitioncamera;
        objWoodStick.SetActive(false);
        playerController.PlayerOnGetItem(false);
        playerController.CamOnGetItem(false);
        yield return wfsTimeTransitioncamera;
        playerController.ChangeState(PlayerState.PlayerMoving);
    }
    #endregion

    #region Shovel
    public void OpenShovelArea()
    {
        playerController.ChangeState(PlayerState.PlayerIddle);
        vcOpenShovelArea.enabled = true;
        StartCoroutine(IeOpenShovelArea());
    }
    IEnumerator IeOpenShovelArea()
    {
        yield return wfsTimeTransitioncamera;
        yield return wfsTimeTransitioncamera;
        PlayFinishShomething();
        vcOpenShovelArea.enabled = false;
        yield return wfsTimeTransitioncamera;
        playerController.ChangeState(PlayerState.PlayerMoving);
    }
    public void GotShovel()
    {
        playerController.ChangeState(PlayerState.PlayerIddle);
        playerController.CamOnGetItem(true);
        StartCoroutine(IeShovel());
    }
    IEnumerator IeShovel()
    {
        yield return wfsTimeTransitioncamera;
        playerController.PlayerOnGetItem(true);
        objShovel.transform.position = playerController.transGrab.position;
        objShovel.transform.LookAt(transPlayerCameraZoom, Vector3.up);
        objShovel.SetActive(true);
        PlayGotItem();
        yield return wfsTimeTransitioncamera;
        objShovel.SetActive(false);
        playerController.PlayerOnGetItem(false);
        playerController.CamOnGetItem(false);
        isPlayerGotShovel = true;
        yield return wfsTimeTransitioncamera;
        playerController.ChangeState(PlayerState.PlayerMoving);
    }
    public void OnBackShovelArea()
    {
        if (!isPlayerGotShovel) return;
        deltaTimeShovelArea = 0;
        StartCoroutine(IeOnBackShovelArea());
    }
    IEnumerator IeOnBackShovelArea()
    {
        yield return wfsTimeTransitioncamera;
        yield return wfsTimeTransitioncamera;
        playerController.ChangeState(PlayerState.PlayerIddle);
        PlaySomethingHappen();
        vcPrisonShovelArea.enabled = true;
        objPrisonShovelArea.SetActive(true);
        yield return StartCoroutine(IePrisonMoving(true));
        vcPrisonShovelArea.enabled = false;
        gridManagement.eventOnDigging += OnCheckAreaDig;
        yield return wfsTimeTransitioncamera;
        playerController.ChangeState(PlayerState.PlayerMoving);
    }
    public void OnCheckAreaDig(Vector3Int digarea)
    {
        for (int i = 0; i < listAreaDigKeyPrisonShovelArea.Count; i++)
        {
            if (listAreaDigKeyPrisonShovelArea[i] == digarea)
            {
                OnFinishPrisonShovelArea();
                return;
            }
        }
    }
    public void OnFinishPrisonShovelArea()
    {
        gridManagement.eventOnDigging -= OnCheckAreaDig;
        playerController.ChangeState(PlayerState.PlayerIddle);
        PlayFinishShomething();
        vcFindKeyPrisonShovelArea.enabled = true;
        objKeyPrisonShovelArea.SetActive(true);
        deltaTimeShovelArea = 0;
        StartCoroutine(IeOnFinishPrisonShovelArea());
    }
    IEnumerator IeOnFinishPrisonShovelArea()
    {
        yield return wfsTimeTransitioncamera;
        while (deltaTimeShovelArea < 5)
        {
            float height = Mathf.Lerp(StartHeightKeyPrison, FinalHeightKeyPrison, deltaTimeShovelArea / 5);
            objKeyPrisonShovelArea.transform.position = new Vector3(objKeyPrisonShovelArea.transform.position.x, height, objKeyPrisonShovelArea.transform.position.z);
            deltaTimeShovelArea += Time.deltaTime;
            yield return null;
        }
        vcFindKeyPrisonShovelArea.enabled = false;
        objKeyPrisonShovelArea.SetActive(false);
        playerController.ChangeState(PlayerState.PlayerMoving);
        deltaTimeShovelArea = 0;
        StartCoroutine(IePrisonMoving(false));
    }
    IEnumerator IePrisonMoving(bool goUp)
    {
        while (deltaTimeShovelArea < 5)
        {
            float height = Mathf.Lerp(goUp ? StartHeightPrisonShovelArea : FinalHeightPrisonShovelArea, goUp ? FinalHeightPrisonShovelArea : StartHeightPrisonShovelArea, deltaTimeShovelArea / 5);
            objPrisonShovelArea.transform.position = new Vector3(objPrisonShovelArea.transform.position.x, height, objPrisonShovelArea.transform.position.z);
            deltaTimeShovelArea += Time.deltaTime;
            yield return null;
        }
        objPrisonShovelArea.transform.position = new Vector3(objPrisonShovelArea.transform.position.x, goUp ? FinalHeightPrisonShovelArea : StartHeightPrisonShovelArea, objPrisonShovelArea.transform.position.z);
    }
    #endregion
    #endregion

    #region Bgm
    [FoldoutGroup("BGM Sound")][SerializeField] private AudioSource BgmAudio;
    [FoldoutGroup("BGM Sound")][SerializeField] private AudioClip clipMainMenu;
    [FoldoutGroup("BGM Sound")][SerializeField] private AudioClip clipField;
    [FoldoutGroup("BGM Sound")][SerializeField] private AudioClip clipMiniBoss;
    [FoldoutGroup("BGM Sound")][SerializeField] private AudioClip clipBoss;
    void PlayBgm(AudioClip clip)
    {
        BgmAudio.Stop();
        BgmAudio.clip = clip;
        BgmAudio.Play();
    }
    #endregion
    #region SFX Sound
    [FoldoutGroup("SFX Sound")][SerializeField] private AudioSource sfxAudio;
    [FoldoutGroup("SFX Sound")][SerializeField] private AudioClip clipGotItem;
    [FoldoutGroup("SFX Sound")][SerializeField] private AudioClip clipSomethingHappen;
    [FoldoutGroup("SFX Sound")][SerializeField] private AudioClip clipFinishSomething;
    void PlayGotItem()
    {
        sfxAudio.PlayOneShot(clipGotItem);
    }
    void PlayFinishShomething()
    {
        sfxAudio.PlayOneShot(clipFinishSomething);
    }
    void PlaySomethingHappen()
    {
        sfxAudio.PlayOneShot(clipSomethingHappen);
    }
    #endregion
}
