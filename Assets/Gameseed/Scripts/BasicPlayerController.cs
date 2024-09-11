using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BasicPlayerController : MonoBehaviour, IDamageable
{
    #region Player State
    public PlayerState playerState;
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
        _hasAnimator = TryGetComponent(out _animator);
        if (!rb) rb = GetComponent<Rigidbody>();
        AssignAnimationIDs();
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
            if (Physics.Raycast(transform.position - (GroundedOffset * Vector3.up), Vector3.down, out hit, 0.2f))
                movement = Vector3.ProjectOnPlane(movement, hit.normal);
        }
        //Move Position
        rb.MovePosition(transform.position + movement);
        if (_hasAnimator) _animator.SetFloat(_animIDSpeed, rb.velocity.magnitude);
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
                _animator.SetBool(_animIDFreeFall, true);
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
        if (_hasAnimator) _animator.SetBool(_animIDDodge, true);
        rb.AddForce(transform.forward * forceDodge, ForceMode.Impulse);
        yield return wfsTimeDodge;
        if (_hasAnimator) _animator.SetBool(_animIDDodge, false);
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

    public void SetObjectGrab(GameObject grab)
    {
        StartCoroutine(IeGrabThrow(true, grab));
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
        StartCoroutine(IeGrabThrow(false, null));
    }
    IEnumerator IeGrabThrow(bool isGrab, GameObject obj)
    {
        playerState = PlayerState.PlayerAction;
        if (_hasAnimator) _animator.SetTrigger(isGrab ? _animIDGrab : _animIDThrow);
        yield return wfsTimeGrabThrow;
        if (isGrab)
            onGrab = true;
        else
        {
            onGrab = false;
            objGrab.GetComponent<IThrowable>().Throw();
        }
        objGrab = obj;
        playerState = PlayerState.PlayerMoving;
    }
    IEnumerator IePlantSeed(Seed seed, int index)
    {
        playerState = PlayerState.PlayerAction;
        if (_hasAnimator)
            _animator.SetTrigger("Plant Seed");
        yield return wfsTimePlantSeed;
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
        playerState = PlayerState.PlayerPlayMusic;
        if (_hasAnimator) _animator.SetTrigger(_animIDPlayMusic);
        sfxAudioSource.PlayOneShot(listDataMusic[indexPlayMusic].audioClip);
        yield return wfsTimeDurationPlaySound;
        Collider[] hits = Physics.OverlapSphere(transform.position, musicableRadius, layerInteractable);
        foreach (var hitcollider in hits)
        {
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
    #endregion

    #region Animation
    private Animator _animator;
    private bool _hasAnimator;
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDFreeFall;
    private int _animIDDodge;
    private int _animIDAttack;
    private int _animIDPlayMusic;
    private int _animIDDig;
    private int _animIDPileUp;
    private int _animIDThrow;
    private int _animIDGrab;
    private int _animIDHurt;
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDDodge = Animator.StringToHash("Dodging");
        _animIDAttack = Animator.StringToHash("Attacking");
        _animIDPlayMusic = Animator.StringToHash("Playing Music");
        _animIDDig = Animator.StringToHash("Digging");
        _animIDPileUp = Animator.StringToHash("Piling Up");
        _animIDGrab = Animator.StringToHash("Grabbing");
        _animIDThrow = Animator.StringToHash("Throwing");
        _animIDHurt = Animator.StringToHash("Hurt");
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
