using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using JVTMPro;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

public class StoryProgressManager : MonoBehaviour
{
    [FoldoutGroup("Story Progress Manager")][SerializeField] private MainMenu mainMenu;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private BlackscreenManager bsManager;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private GridManagement gridManagement;
    [FoldoutGroup("Story Progress Manager")][SerializeField] private UiBoard uiBoard;
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
            case 2:
                bsManager.eventOnFadeIn += ContinueProgressChapter2;
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
        playerController.gameObject.SetActive(true);
        playerUiManager.gameObject.SetActive(true);
    }
    #endregion
    #region Timeline Story
    [FoldoutGroup("Start Game")][SerializeField] PlayableDirector playableDirector;
    [FoldoutGroup("Start Game")][SerializeField] GameObject objGateStartEffect;
    [FoldoutGroup("Start Game")][SerializeField] GameObject objCameraTimeline;
    [FoldoutGroup("Start Game")][SerializeField] GameObject objSoundTimeline;
    [FoldoutGroup("Start Game")][SerializeField] GameObject objPlayerTimeline;
    public void StartGame()
    {
        BgmAudio.Stop();
        playerController.gameObject.SetActive(false);
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
        objPlayerTimeline.SetActive(false);
        PlayBgm(clipField);
        bsManager.eventOnFadeOut += PlayerMove;
        bsManager.FadeOut();
    }
    #endregion
    #region Chapter 1 : Arrival
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private Transform transZ1Spawn;
    [FoldoutGroup("Chapter 1 : Arrival")][SerializeField] private CinemachineVirtualCamera vcOpenShovelArea;
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
        prisonMusic1.CheckReset();
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
        PlayGotItem();
        yield return wfsTimeTransitioncamera;
        yield return wfsTimeTransitioncamera;
        yield return wfsTimeTransitioncamera;
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
        StartCoroutine(IeOpenShovelArea());
    }
    IEnumerator IeOpenShovelArea()
    {
        yield return wfsTimeTransitioncamera;
        PlayFinishShomething();
        vcOpenShovelArea.enabled = true;
        yield return wfsTimeTransitioncamera;
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
        PlayGotItem();
        yield return wfsTimeTransitioncamera;
        yield return wfsTimeTransitioncamera;
        yield return wfsTimeTransitioncamera;
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
        IndexProgress = 2;
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

    #region Chapter 2 : Musical
    [FoldoutGroup("Chapter 2 : Musical")][SerializeField] private Transform transZ2Spawn;
    [FoldoutGroup("Chapter 2 : Musical")][SerializeField] private bool haveWestRune;
    [FoldoutGroup("Chapter 2 : Musical")][SerializeField] private bool haveEastRune;
    [FoldoutGroup("Chapter 2 : Musical")][SerializeField] private List<JVTextMeshProUGUI> listTextFinalBossDooor;
    [FoldoutGroup("Chapter 2 : Musical")] private bool FirstCheckMiniBoss = false;
    [FoldoutGroup("Chapter 2 : Musical")][SerializeField] private float timeTimelineCheckMiniBoss;
    [FoldoutGroup("Chapter 2 : Musical")][SerializeField] private TriggerPrison prisonMusic1;
    [FoldoutGroup("Chapter 2 : Musical")] private bool gotMusic1;
    [FoldoutGroup("Chapter 2 : Musical")][SerializeField] private MusicData musicData1;
    [FoldoutGroup("Chapter 2 : Musical")][SerializeField] private CinemachineVirtualCamera vcPrasastiMusic1;
    void ContinueProgressChapter2()
    {
        bsManager.eventOnFadeIn -= ContinueProgressChapter2;
        transPlayer.position = transZ2Spawn.position;
        transPlayer.rotation = transZ2Spawn.rotation;
        playerStatus.ResetHealth();
        playerController.Revive();
        bsManager.eventOnFadeOut += PlayerMove;
        bsManager.FadeOut();
    }
    public void GotMusicInstrument()
    {
        playerController.ChangeState(PlayerState.PlayerIddle);
        playerController.CamOnGetPuiPui(true);
        StartCoroutine(IeGotMusicalInstrument());
    }
    IEnumerator IeGotMusicalInstrument()
    {
        yield return wfsTimeTransitioncamera;
        playerController.PlayerOnGetItem(true);
        PlayGotItem();
        yield return wfsTimeTransitioncamera;
        yield return wfsTimeTransitioncamera;
        yield return wfsTimeTransitioncamera;
        playerController.PlayerOnGetItem(false);
        playerController.CamOnGetPuiPui(false);
        yield return wfsTimeTransitioncamera;
        playerController.ChangeState(PlayerState.PlayerMoving);
    }
    public void OpenFinalBossDoor()
    {
        if (!haveWestRune || !haveEastRune)
        {
            if (!FirstCheckMiniBoss)
            {
                FirstCheckMiniBoss = true;
                playerController.ChangeState(PlayerState.PlayerIddle);
                playerUiManager.gameObject.SetActive(false);
                CheckMinibossArea();
            }
            else
            {
                deltaTimeShovelArea = 0;
                StartCoroutine(IeFinalBossDoorLocked());
            }
        }
        else
        {
            playerController.ChangeState(PlayerState.PlayerIddle);
            // Coroutine Open Final Boss Door
        }
    }
    IEnumerator IeFinalBossDoorLocked()
    {
        PlaySomethingHappen();
        while (deltaTimeShovelArea < 2)
        {
            Color color = Color.Lerp(Color.white, Color.red, deltaTimeShovelArea / 2);
            if (!haveWestRune) listTextFinalBossDooor[0].color = color;
            if (!haveEastRune) listTextFinalBossDooor[1].color = color;
            listTextFinalBossDooor[2].color = color;
            deltaTimeShovelArea = Time.deltaTime;
            yield return null;
        }
        if (!haveWestRune) listTextFinalBossDooor[0].color = Color.red;
        if (!haveEastRune) listTextFinalBossDooor[1].color = Color.red;
        listTextFinalBossDooor[2].color = Color.red;
        deltaTimeShovelArea = 0;
        while (deltaTimeShovelArea < 2)
        {
            Color color = Color.Lerp(Color.red, Color.white, deltaTimeShovelArea / 2);
            if (!haveWestRune) listTextFinalBossDooor[0].color = color;
            if (!haveEastRune) listTextFinalBossDooor[1].color = color;
            listTextFinalBossDooor[2].color = color;
            deltaTimeShovelArea = Time.deltaTime;
            yield return null;
        }
        if (!haveWestRune) listTextFinalBossDooor[0].color = Color.white;
        if (!haveEastRune) listTextFinalBossDooor[1].color = Color.white;
        listTextFinalBossDooor[2].color = Color.white;
    }
    void CheckMinibossArea()
    {
        StopBgm();
        playableDirector.time = timeTimelineCheckMiniBoss;
        playableDirector.Play();
    }
    public void TimelineEndCheckBossArea()
    {
        playableDirector.Stop();
        bsManager.eventOnFadeIn += FadeInTimelineCheckBossArea;
        bsManager.FadeIn();
    }
    void FadeInTimelineCheckBossArea()
    {
        bsManager.eventOnFadeIn -= FadeInTimelineStartGame;
        PlayBgm(clipField);
        objCameraTimeline.SetActive(false);
        objSoundTimeline.SetActive(false);
        bsManager.eventOnFadeOut += PlayerMove;
        bsManager.FadeOut();
    }
    public void SetButtonAddMusic1()
    {
        if(gotMusic1) return;
        gotMusic1 = true;
        vcPrasastiMusic1.enabled = true;
        uiBoard.btnBoard.onClick.AddListener(AddMusic1);
    }
    public void AddMusic1()
    {
        vcPrasastiMusic1.enabled = false;
        uiBoard.btnBoard.onClick.RemoveListener(AddMusic1);
        playerController.ChangeState(PlayerState.PlayerIddle);
        playerController.CamOnGetPuiPui(true);
        StartCoroutine(IeGotMusic1());
    }
    IEnumerator IeGotMusic1()
    {
        yield return wfsTimeTransitioncamera;
        playerController.PlayerOnGetItem(true);
        playerController.AddNewMusic(musicData1);
        PlayGotItem();
        yield return wfsTimeTransitioncamera;
        yield return wfsTimeTransitioncamera;
        yield return wfsTimeTransitioncamera;
        playerController.PlayerOnGetItem(false);
        playerController.CamOnGetPuiPui(false);
        yield return wfsTimeTransitioncamera;
        playerController.ChangeState(PlayerState.PlayerMoving);
    }
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
    void StopBgm()
    {
        BgmAudio.Stop();
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
