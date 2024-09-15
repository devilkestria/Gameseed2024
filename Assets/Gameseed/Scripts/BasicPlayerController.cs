using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BasicPlayerController : MonoBehaviour, IDamageable
{
    #region Player State
    public PlayerState playerState;
    public void ChangeState(PlayerState state)
    {
        playerState = state;
    }
    #endregion
    #region Unity Function
    void OnDestroy()
    {
        RemoveInput();
    }
    void Awake()
    {
        wfsTimeDodge = new WaitForSeconds(timeDodge);
        wfsTimeGrabThrow = new WaitForSeconds(timeGrabThrow);
        wfsTimePlantSeed = new WaitForSeconds(timePlantSeed / 2);
        wfsTimeDurationDigPileUp = new WaitForSeconds(timeDurationDigPileUp / 2);
        wfsTimeDurationDamage = new WaitForSeconds(timeDurationDamage);
        SetInputAction();
        AddInput();
    }
    private void Start()
    {
        if (_animator) _animator = GetComponent<Animator>();
        _hasAnimator = _animator != null ? true : false;
        if (!rb) rb = GetComponent<Rigidbody>();
        AssignAnimationIDs();
        CheckWeapon();
    }
    private void Update()
    {
        GroundedCheck();
    }
    private void FixedUpdate()
    {
        if (playerState == PlayerState.PlayerMoving || playerState == PlayerState.PlayerPlayMusic)
            PlayerMove();
    }
    #endregion

    #region Player Input
    [FoldoutGroup("Player Input")]
    public PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction attackAction;
    private InputAction dodgeAction;
    private InputAction useEquipmentAction;
    private InputAction playMusicAction;
    private InputAction interactAction;
    private InputAction changeMusicNextAction;
    private InputAction changeMusicPreviousAction;
    void SetInputAction()
    {
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        attackAction = playerInput.actions["Attack"];
        dodgeAction = playerInput.actions["Dodge"];
        useEquipmentAction = playerInput.actions["Use Equipment"];
        interactAction = playerInput.actions["Interact"];
        playMusicAction = playerInput.actions["Play Music"];
        changeMusicNextAction = playerInput.actions["Change Music Next"];
        changeMusicPreviousAction = playerInput.actions["Change Music Previous"];
    }
    void AddInput()
    {
        lookAction.performed += Look_Performed;
        attackAction.performed += Attack_Performed;
        dodgeAction.performed += Dodge_Performed;
        useEquipmentAction.performed += UseEquipment_Performed;
        interactAction.performed += Interact_Performed;
        playMusicAction.performed += PlayMusic_Performed;
        changeMusicNextAction.performed += ChangeMusicNext_Performed;
        changeMusicPreviousAction.performed += ChangeMusicPrevious_Performed;
    }
    void RemoveInput()
    {
        lookAction.performed -= Look_Performed;
        attackAction.performed -= Attack_Performed;
        dodgeAction.performed -= Dodge_Performed;
        useEquipmentAction.performed -= UseEquipment_Performed;
        playMusicAction.performed -= PlayMusic_Performed;
        changeMusicNextAction.performed -= ChangeMusicNext_Performed;
        changeMusicPreviousAction.performed -= ChangeMusicPrevious_Performed;
    }

    #region Action Performed
    void Look_Performed(InputAction.CallbackContext context)
    {
    }
    void Attack_Performed(InputAction.CallbackContext context)
    {
        if (playerState == PlayerState.PlayerMoving && !onGrab)
            Attacking();
    }
    void Dodge_Performed(InputAction.CallbackContext context)
    {
        if ((playerState == PlayerState.PlayerMoving || playerState == PlayerState.PlayerAttack) && !onGrab)
            Dodging();
    }
    void UseEquipment_Performed(InputAction.CallbackContext context)
    {
        if (playerState == PlayerState.PlayerMoving && !onGrab)
            UsingEquipment();
    }
    void Interact_Performed(InputAction.CallbackContext context)
    {
        if (playerState == PlayerState.PlayerMoving)
            Interacting();
    }
    void PlayMusic_Performed(InputAction.CallbackContext context)
    {
        if (playerState == PlayerState.PlayerMoving && !onGrab)
            PlayMusic();
    }
    void ChangeMusicNext_Performed(InputAction.CallbackContext context)
    {
        ChangeMusic(true);
    }
    void ChangeMusicPrevious_Performed(InputAction.CallbackContext context)
    {
        ChangeMusic(false);
    }
    #endregion

    #endregion

    #region Player Basic Movement

    #region Player Move
    [Tooltip("Player Rigid Body")]
    [FoldoutGroup("Player Movement")][SerializeField] private Rigidbody rb;
    [Tooltip("Move speed of the character in m/s")]
    [FoldoutGroup("Player Movement")] public float MoveSpeed = 5.0f;
    [Tooltip("How fast the character turns to face movement direction")]
    [FoldoutGroup("Player Movement")] public float RotateOnMove = 360f;
    [FoldoutGroup("Player Movement")][SerializeField] private Transform transCamera;
    [FoldoutGroup("Player Movement")][SerializeField] private float maxSlopeAngle;
    [FoldoutGroup("Player Movement")] bool isMovingDownhill;
    private void PlayerMove()
    {
        Vector2 inputmove = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(inputmove.x, 0, inputmove.y);
        float speed = onGrab ? MoveSpeedGrab : playerState == PlayerState.PlayerPlayMusic ? MoveSpeedPlayMusic : MoveSpeed;
        Vector3 movement = transform.forward * moveDirection.magnitude * speed * Time.deltaTime;
        if (inputmove != Vector2.zero)
        {
            Vector3 camrot = new Vector3(0, transCamera.rotation.eulerAngles.y, 0);
            var relative = transform.position + moveDirection.ToIso(camrot) - transform.position;
            var rot = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, RotateOnMove * Time.deltaTime);

            // Slope Fix
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 1.5f, GroundLayers))
            {
                // movement = Vector3.ProjectOnPlane(movement, hit.normal);
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                Debug.Log(slopeAngle);
                Vector3 slopeDownDirection = Vector3.Cross(Vector3.Cross(hit.normal, Vector3.down), hit.normal).normalized;

                // Check if player is moving downhill
                float dotProduct = Vector3.Dot(movement.normalized, slopeDownDirection);
                isMovingDownhill = dotProduct > 0;

                // Only move if the slope angle is below maxSlopeAngle
                if (slopeAngle <= maxSlopeAngle)
                {
                    // Project movement vector onto the slope
                    movement = Vector3.ProjectOnPlane(movement, hit.normal);
                }
                else
                {
                    // Nullify movement if slope is too steep
                    movement = Vector3.zero;
                }

                // Apply a custom force to counteract sliding
                Vector3 slopeDirection = Vector3.Cross(Vector3.Cross(hit.normal, Vector3.down), hit.normal);
                if (slopeAngle > 0 && !isMovingDownhill)
                    rb.AddForce(-slopeDirection * speed * 3, ForceMode.Acceleration);
            }
        }
        //Move Position
        rb.MovePosition(transform.position + movement);
        if (_hasAnimator) _animator.SetFloat(_animIDSpeed, moveDirection.magnitude);
    }
    public void ChangeCamera(Transform cameratarget)
    {
        transCamera = cameratarget;
    }
    #endregion

    #region Player Ground
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [FoldoutGroup("Player Ground")] public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    [FoldoutGroup("Player Ground")] public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [FoldoutGroup("Player Ground")] public float GroundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    [FoldoutGroup("Player Ground")] public LayerMask GroundLayers;
    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
        if (Grounded)
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDFreeFall, false);
            }
        }
        else
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDFreeFall, rb.velocity.y > 0 ? false : true);
            }
        }
    }
    #endregion

    #region Dodging
    [FoldoutGroup("Dodge")][SerializeField] Collider bodyCollider;
    [FoldoutGroup("Dodge")][SerializeField] float forceDodge;
    [FoldoutGroup("Dodge")] public float timeDodge;
    [FoldoutGroup("Dodge")] public UnityAction eventActionDodge;
    WaitForSeconds wfsTimeDodge;
    private void Dodging()
    {
        if (corouAttacking != null)
        {
            StopCoroutine(corouAttacking);
            corouAttacking = null;
        }
        StartCoroutine(IeDodging());
    }
    IEnumerator IeDodging()
    {
        eventActionDodge?.Invoke();
        bodyCollider.enabled = false;
        playerState = PlayerState.PlayerDodging;
        if (_hasAnimator) _animator.SetTrigger(_animIDDodge);
        rb.AddForce(transform.forward * forceDodge, ForceMode.Impulse);
        yield return wfsTimeDodge;
        rb.velocity = Vector3.zero;
        bodyCollider.enabled = true;
        playerState = PlayerState.PlayerMoving;
    }
    #endregion

    #region Attacking

    [FoldoutGroup("Attack")][SerializeField] private AttackObject prefabAttack;
    [FoldoutGroup("Attack")] private AttackObject currentAttack;
    [FoldoutGroup("Attack")][SerializeField] private float timePrepareAttack;
    [FoldoutGroup("Attack")][SerializeField] private float timeFinishAttack;
    [FoldoutGroup("Attack")][SerializeField] private List<AudioClip> listSfxAttack = new List<AudioClip>();
    [FoldoutGroup("Attack")][SerializeField] private Transform transAttackPoint;
    [FoldoutGroup("Attack")] private List<AttackObject> listAttack = new List<AttackObject>();
    private WaitForSeconds wfsTimePrepareAttack;
    private WaitForSeconds wfsTimeFinishAttack;
    private Coroutine corouTempAttack;
    private Coroutine corouAttacking;
    [FoldoutGroup("Attack")] private AttackObject originalAttack;
    [FoldoutGroup("Attack")] private float originalPrepareTime;
    [FoldoutGroup("Attack")] private float originalFinishTime;
    [FoldoutGroup("Attack")] private List<AudioClip> originalSfxAtk = new List<AudioClip>();
    [FoldoutGroup("Attack")] public UnityAction<AttackObject, float> eventOnChangeAttack;
    [FoldoutGroup("Attack")] public UnityAction eventActionAttack;
    [FoldoutGroup("Attack")] private bool haveWood;
    [FoldoutGroup("Attack")][SerializeField] private GameObject objWood;
    [FoldoutGroup("Attack")] private bool haveShovel;
    [FoldoutGroup("Attack")][SerializeField] private GameObject objShovel;
    void CheckWeapon()
    {
        if (prefabAttack)
        {
            SetAttack(prefabAttack, timePrepareAttack, timeFinishAttack, listSfxAttack);
        }
    }

    public void OnChangeAttack(AttackObject atk, float prepareTime, float finishTime, List<AudioClip> audioatk)
    {
        SetAttack(atk, prepareTime, finishTime, audioatk);
        eventOnChangeAttack?.Invoke(atk, prepareTime + finishTime);
    }

    public void OnTemporaryChangeAttack(AttackObject atk, float prepareTime, float finishTime, float temporaryTime, List<AudioClip> sfxatk)
    {
        if (corouTempAttack != null)
        {
            StopCoroutine(corouTempAttack);
        }
        corouTempAttack = StartCoroutine(TemporaryChangeAttack(atk, prepareTime, finishTime, temporaryTime, sfxatk));
    }

    private IEnumerator TemporaryChangeAttack(AttackObject atk, float prepareTime, float finishTime, float temporaryTime, List<AudioClip> sfxatk)
    {
        originalPrepareTime = timePrepareAttack;
        originalFinishTime = timeFinishAttack;
        originalSfxAtk = listSfxAttack;
        originalAttack = prefabAttack;

        SetAttack(atk, prepareTime, finishTime, sfxatk);

        yield return new WaitForSeconds(temporaryTime);

        SetAttack(originalAttack, originalPrepareTime, originalFinishTime, originalSfxAtk);

        originalAttack = null;
        originalPrepareTime = 0;
        originalFinishTime = 0;
        originalSfxAtk = null;
    }

    private void SetAttack(AttackObject atk, float prepareTime, float finishTime, List<AudioClip> sfxatk)
    {
        prefabAttack = atk;
        currentAttack = atk;
        timePrepareAttack = prepareTime;
        timeFinishAttack = finishTime;
        listSfxAttack = sfxatk;
        wfsTimePrepareAttack = new WaitForSeconds(timePrepareAttack);
        wfsTimeFinishAttack = new WaitForSeconds(timeFinishAttack);
    }

    private void Attacking()
    {
        if (currentAttack != null)
        {
            corouAttacking = StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        eventActionAttack?.Invoke();
        playerState = PlayerState.PlayerAttack;
        rb.velocity = Vector3.zero;

        if (_hasAnimator)
        {
            _animator.SetTrigger(_animIDAttack);
        }

        yield return wfsTimePrepareAttack;

        AttackObject attack = GetAvailableAttackObject();
        if (attack == null)
        {
            GameObject attackInstance = Instantiate(currentAttack.gameObject, transAttackPoint.position, transAttackPoint.rotation);
            attack = attackInstance.GetComponent<AttackObject>();
            listAttack.Add(attack);
        }
        int random = Random.Range(0, listSfxAttack.Count);
        sfxAudioSource.PlayOneShot(listSfxAttack[random]);
        attack.transform.SetPositionAndRotation(transAttackPoint.position, transAttackPoint.rotation);
        attack.gameObject.SetActive(true);

        yield return wfsTimeFinishAttack;

        corouAttacking = null;
        playerState = PlayerState.PlayerMoving;
    }

    private AttackObject GetAvailableAttackObject()
    {
        foreach (var attack in listAttack)
        {
            if (!attack.onActive && attack.nameAtk == currentAttack.nameAtk)
            {
                return attack;
            }
        }
        return null;
    }
    public void ActiveWood()
    {
        haveWood = true;
        haveShovel = false;
        objWood.SetActive(true);
        objShovel.SetActive(false);
    }
    public void ActiveShovel()
    {
        haveWood = false;
        haveShovel = true;
        objWood.SetActive(false);
        objShovel.SetActive(true);
    }
    public void ActiveWeaponObj(bool value)
    {
        objWood.SetActive(!haveWood ? false : value ? true : false);
        objShovel.SetActive(!haveWood ? false : value ? true : false);
    }
    #endregion

    #region Interact
    [FoldoutGroup("Interact")][SerializeField] private float interactRadius;
    [FoldoutGroup("Interact")][SerializeField] private float interactDistance;
    [FoldoutGroup("Interact")][SerializeField] private LayerMask layerInteractable;
    [FoldoutGroup("Interact")][SerializeField] private Vector3 interactOffsetPos;
    void Interacting()
    {
        if (onGrab)
        {
            ThrowObject();
            return;
        }
        RaycastHit[] hits = Physics.SphereCastAll(transform.position + interactOffsetPos, interactRadius, transform.forward, interactRadius, layerInteractable);
        if (hits.Length > 0)
        {
            IInteractable interactable = hits[0].collider.GetComponent<IInteractable>();
            Debug.Log(hits[0].collider.name);
            if (interactable != null)
                interactable.Interact();
        }
    }
    #endregion

    #region Grabing & Throw
    [FoldoutGroup("Grabing & Throw")] public Transform transGrab;
    [FoldoutGroup("Grabing & Throw")] private bool onGrab;
    [FoldoutGroup("Grabing & Throw")] private GameObject objGrab;
    [FoldoutGroup("Grabing & Throw")][SerializeField] private float MoveSpeedGrab = 0.25f;
    [FoldoutGroup("Grabing & Throw")][SerializeField] private float timeGrabThrow = 0.5f;
    [FoldoutGroup("Grabing & Throw")][SerializeField] private float timePlantSeed = 1f;
    [FoldoutGroup("Grabing & Throw")] private WaitForSeconds wfsTimeGrabThrow;
    [FoldoutGroup("Grabing & Throw")] private WaitForSeconds wfsTimePlantSeed;

    public void SetObjectGrab(GameObject grab, UnityAction action)
    {
        StartCoroutine(IeGrabThrow(true, grab, action));
    }
    private void ThrowObject()
    {
        Seed seed = objGrab.GetComponent<Seed>();
        if (seed != null)
        {
            Vector3Int area = grid.WorldToCell(transform.position);
            int index = -1;
            if (gridManagement.CheckPlayerCanPlantSeed(area, out index))
            {
                StartCoroutine(IePlantSeed(seed, index));
                return;
            }
        }
        StartCoroutine(IeGrabThrow(false, null, null));
    }
    IEnumerator IeGrabThrow(bool playerGrab, GameObject obj, UnityAction action)
    {
        playerState = PlayerState.PlayerAction;
        if (_hasAnimator) _animator.SetFloat(_animIDSpeed, 0);
        if (!onGrab) _animator.SetLayerWeight(1, 1);
        if (playerGrab)
            onGrab = true;
        else
        {
            onGrab = false;
            objGrab.GetComponent<IThrowable>().Throw();
        }
        if (_hasAnimator) _animator.SetTrigger(playerGrab ? _animIDGrab : _animIDThrow);
        yield return wfsTimeGrabThrow;
        action?.Invoke();
        if (!playerGrab) _animator.SetLayerWeight(1, 0);
        objGrab = obj;
        playerState = PlayerState.PlayerMoving;
    }
    IEnumerator IePlantSeed(Seed seed, int index)
    {
        playerState = PlayerState.PlayerAction;
        if (_hasAnimator) _animator.SetTrigger(_animIdPlantSeed);
        yield return wfsTimePlantSeed;
        _animator.SetLayerWeight(1, 0);
        onGrab = false;
        seed.PlantSeed(index);
        objGrab = null;
        yield return wfsTimePlantSeed;
        playerState = PlayerState.PlayerMoving;
    }
    #endregion

    #region Using Equipment
    void UsingEquipment()
    {
        if (prefabAttack == null) return;
        switch (prefabAttack.nameAtk)
        {
            case "Shovel Slash":
                StartCoroutine(IeShovelDig());
                break;
        }
    }
    #region Shovel Digging
    [FoldoutGroup("Using Equipment")]
    [FoldoutGroup("Using Equipment/Shovel Digging")][SerializeField] private List<AudioClip> listAudioDig;
    [FoldoutGroup("Using Equipment/Shovel Digging")][SerializeField] private List<AudioClip> listAudioPileUp;
    [FoldoutGroup("Using Equipment/Shovel Digging")][SerializeField] private LayerMask layerGroundDigging;
    [FoldoutGroup("Using Equipment/Shovel Digging")][SerializeField] private float distanceRayDig = 0.25f;
    [FoldoutGroup("Using Equipment/Shovel Digging")][SerializeField] private GridManagement gridManagement => GameplayManager.instance.gridManagement;
    [FoldoutGroup("Using Equipment/Shovel Digging")][SerializeField] private Grid grid => gridManagement.grid;
    [FoldoutGroup("Using Equipment/Shovel Digging")] public float timeDurationDigPileUp;
    [FoldoutGroup("Using Equipment/Shovel Digging")] WaitForSeconds wfsTimeDurationDigPileUp;
    [FoldoutGroup("Using Equipment/Shovel Digging")] public UnityAction eventActionDigPileUp;
    IEnumerator IeShovelDig()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + (0.1f * Vector3.up), Vector3.down, out hit, distanceRayDig, layerGroundDigging))
        {
            eventActionDigPileUp?.Invoke();
            playerState = PlayerState.PlayerAction;
            int indexOut = -1;
            Vector3Int gridPosition = grid.WorldToCell(hit.point);

            // Do Pile Up
            if (gridManagement.CheckAreaIsOcupied(gridPosition, out indexOut))
            {
                if (_hasAnimator) _animator.SetTrigger(_animIDPileUp);
                int random = Random.Range(0, listAudioPileUp.Count);
                sfxAudioSource.PlayOneShot(listAudioPileUp[random]);
                yield return wfsTimeDurationDigPileUp;
                gridManagement.RemoveAreaData(indexOut);
            }
            // Digging
            else
            {
                if (_hasAnimator) _animator.SetTrigger(_animIDDig);
                int random = Random.Range(0, listAudioDig.Count);
                sfxAudioSource.PlayOneShot(listAudioDig[random]);
                yield return wfsTimeDurationDigPileUp;
                gridManagement.AddAreaData(gridPosition);
            }
            yield return wfsTimeDurationDigPileUp;
            playerState = PlayerState.PlayerMoving;
        }
    }
    #endregion
    #endregion

    #region Play Music
    [FoldoutGroup("Play Music")][SerializeField] private float MoveSpeedPlayMusic = 4;
    [FoldoutGroup("Play Music")][HideInInspector] public int indexPlayMusic = -1;
    [FoldoutGroup("Play Music")][SerializeField] private LayerMask musicableLayer;
    [FoldoutGroup("Play Music")][SerializeField] private float musicableRadius;
    [FoldoutGroup("Play Music")][HideInInspector] public float timeDurationPlaySound;
    [FoldoutGroup("Play Music")] private WaitForSeconds wfsTimeDurationPlaySound;
    [FoldoutGroup("Play Music")] public List<MusicData> listDataMusic = new List<MusicData>();
    [FoldoutGroup("Play Music")] public UnityAction eventOnAddMusic;
    [FoldoutGroup("Play Music")] public UnityAction eventActionMusic;
    [FoldoutGroup("Play Music")] public UnityAction<bool> eventOnChangeMusic;
    [FoldoutGroup("Play Music")][SerializeField] private GameObject objPuiPui;
    public void AddNewMusic(MusicData data)
    {
        listDataMusic.Add(data);
        indexPlayMusic = listDataMusic.Count - 1;
        timeDurationPlaySound = listDataMusic[indexPlayMusic].timeDuration;
        wfsTimeDurationPlaySound = new WaitForSeconds(timeDurationPlaySound);
        eventOnAddMusic?.Invoke();
    }
    public void PlayMusic()
    {
        if (listDataMusic.Count == 0) return;
        StartCoroutine(IePlayMusic());
    }
    IEnumerator IePlayMusic()
    {
        eventActionMusic?.Invoke();
        ActiveWeaponObj(false);
        objPuiPui.SetActive(true);
        playerState = PlayerState.PlayerPlayMusic;
        if (_hasAnimator) _animator.SetLayerWeight(2, 1);
        sfxAudioSource.PlayOneShot(listDataMusic[indexPlayMusic].audioClip);
        yield return wfsTimeDurationPlaySound;
        ActiveWeaponObj(true);
        objPuiPui.SetActive(false);
        if (_hasAnimator) _animator.SetLayerWeight(2, 0);
        Collider[] hits = Physics.OverlapSphere(transform.position, musicableRadius, musicableLayer);
        foreach (var hitcollider in hits)
        {
            Debug.Log(hitcollider.name);
            IMusicable musicable = hitcollider.GetComponent<IMusicable>();
            if (musicable != null) musicable.Music(listDataMusic[indexPlayMusic]);
        }
        playerState = PlayerState.PlayerMoving;
    }
    public void ChangeMusic(bool next)
    {
        if (listDataMusic.Count <= 1) return;
        indexPlayMusic = indexPlayMusic + (next ? 1 : -1);
        indexPlayMusic = indexPlayMusic < 0 ? listDataMusic.Count - 1 : indexPlayMusic > listDataMusic.Count - 1 ? 0 : indexPlayMusic;
        timeDurationPlaySound = listDataMusic[indexPlayMusic].timeDuration;
        wfsTimeDurationPlaySound = new WaitForSeconds(timeDurationPlaySound);
        eventOnChangeMusic?.Invoke(next);
    }
    #endregion

    #region Damage
    [FoldoutGroup("Damage")] float timeDurationDamage = 0.25f;
    [FoldoutGroup("Damage")] WaitForSeconds wfsTimeDurationDamage;
    [FoldoutGroup("Damage")][Range(0.001f, 0.1f)][SerializeField] private float stillTreshold = 0.05f;
    public void Damage(AttackObject atkobj, AudioClip clip)
    {
        playerState = PlayerState.PlayerOnDamage;
        StopAllCoroutines();
        corouAttacking = null;
        if (originalAttack != null)
            SetAttack(originalAttack, originalPrepareTime, originalFinishTime, originalSfxAtk);
        StartCoroutine(IeOnDamage(atkobj, clip));
    }
    IEnumerator IeOnDamage(AttackObject atkobj, AudioClip clip)
    {
        yield return new WaitForFixedUpdate();
        sfxAudioSource.PlayOneShot(clip);
        if (_hasAnimator) _animator.SetTrigger(_animIDHurt);
        if (atkobj.forcePush > 0)
            rb.AddForce(atkobj.forcePush * atkobj.transform.forward, ForceMode.Impulse);
        yield return new WaitUntil(() => rb.velocity.magnitude < stillTreshold);
        yield return wfsTimeDurationDamage;
        playerState = PlayerState.PlayerMoving;
    }
    #endregion

    #region Revive
    public void Revive()
    {
        if (_hasAnimator) _animator.SetTrigger(_animIDRevive);
    }
    #endregion

    #region Got Item
    [FoldoutGroup("Got Item")][SerializeField] private CinemachineVirtualCamera camZoomInFocusPlayer;
    public void CamOnGetItem(bool value)
    {
        if (_hasAnimator) _animator.SetTrigger(_animIDLookCamera);
        camZoomInFocusPlayer.Priority = value ? 2 : 0;
        objWood.SetActive(!haveWood ? false : value ? false : true);
        objShovel.SetActive(!haveShovel ? false : value ? false : true);
    }
    public void CamOnGetPuiPui(bool value)
    {
        if (_hasAnimator) _animator.SetTrigger(_animIDLookCamera);
        camZoomInFocusPlayer.Priority = value ? 2 : 0;
        objPuiPui.SetActive(value);
        ActiveWeaponObj(!value);
    }
    public void PlayerOnGetItem(bool value)
    {
        if (_hasAnimator) _animator.SetLayerWeight(3, value ? 1 : 0);
    }
    #endregion
    #endregion

    #region Animation
    [FoldoutGroup("Animation")][SerializeField] private Animator _animator;
    private bool _hasAnimator;
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDFreeFall;
    private int _animIDDodge;
    private int _animIDAttack;
    private int _animIDDig;
    private int _animIDPileUp;
    private int _animIDThrow;
    private int _animIDGrab;
    private int _animIdPlantSeed;
    private int _animIDHurt;
    private int _animIDRevive;
    private int _animIDLookCamera;
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDDodge = Animator.StringToHash("Dodging");
        _animIDAttack = Animator.StringToHash("Attacking");
        _animIDDig = Animator.StringToHash("Digging");
        _animIDPileUp = Animator.StringToHash("Piling Up");
        _animIDGrab = Animator.StringToHash("Grabbing");
        _animIDThrow = Animator.StringToHash("Throwing");
        _animIdPlantSeed = Animator.StringToHash("Plant Seed");
        _animIDHurt = Animator.StringToHash("Hurt");
        _animIDRevive = Animator.StringToHash("Revive");
        _animIDLookCamera = Animator.StringToHash("Turn Arround");
    }
    #endregion

    #region Player Sfx
    [FoldoutGroup("Player SFX")][SerializeField] private AudioSource sfxAudioSource;
    #region Player FootStep
    [FoldoutGroup("Player SFX")] public AudioClip LandingAudioClip;
    [FoldoutGroup("Player SFX")] public AudioClip[] FootstepAudioClips;
    [FoldoutGroup("Player SFX")][Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(transform.position), FootstepAudioVolume);
            }
        }
    }
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(transform.position), FootstepAudioVolume);
        }
    }
    #endregion
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transAttackPoint.position + interactOffsetPos, transAttackPoint.position + transform.forward * interactRadius);
        Gizmos.DrawWireSphere(transAttackPoint.position + interactOffsetPos + transform.forward * interactRadius, interactDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, musicableRadius);
    }
}
