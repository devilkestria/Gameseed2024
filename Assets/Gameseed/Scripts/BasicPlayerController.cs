using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicPlayerController : MonoBehaviour
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
        wfsDodge = new WaitForSeconds(timeDodge);
        wfsTimePrepareAttack = new WaitForSeconds(timePrepareAttack);
        wfsTimeFinishAttack = new WaitForSeconds(timeFinishAttack);
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
        _hasAnimator = TryGetComponent(out _animator);
        GroundedCheck();
    }
    private void FixedUpdate()
    {
        if (playerState == PlayerState.PlayerMoving)
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
    private InputAction item1Action;
    private InputAction item2Action;
    private InputAction item3Action;
    private InputAction item4Action;
    void SetInputAction()
    {
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        attackAction = playerInput.actions["Attack"];
        dodgeAction = playerInput.actions["Dodge"];
        useEquipmentAction = playerInput.actions["Use Equipment"];
        item1Action = playerInput.actions["Item 1"];
        item2Action = playerInput.actions["Item 2"];
        item3Action = playerInput.actions["Item 3"];
        item4Action = playerInput.actions["Item 4"];
    }
    void AddInput()
    {
        lookAction.performed += Look_Performed;
        attackAction.performed += Attack_Performed;
        dodgeAction.performed += Dodge_Performed;
        useEquipmentAction.performed += UseEquipment_Performed;
        item1Action.performed += Item1_Performed;
        item2Action.performed += Item2_Performed;
        item3Action.performed += Item3_Performed;
        item4Action.performed += item4_Performed;
    }
    void RemoveInput()
    {
        lookAction.performed -= Look_Performed;
        attackAction.performed -= Attack_Performed;
        dodgeAction.performed -= Dodge_Performed;
        useEquipmentAction.performed -= UseEquipment_Performed;
        item1Action.performed -= Item1_Performed;
        item2Action.performed -= Item2_Performed;
        item3Action.performed -= Item3_Performed;
        item4Action.performed -= item4_Performed;
    }

    #region Action Performed
    void Look_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("Look Value : " + context.ReadValue<Vector2>());
    }
    void Attack_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("Attack Performed :" + context.ReadValueAsButton());
        if (playerState == PlayerState.PlayerMoving)
            Attacking();
    }
    void Dodge_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("Dodge Value : " + context.ReadValueAsButton());
        if (playerState == PlayerState.PlayerMoving)
            Dodging();
    }
    void UseEquipment_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("UseEquipment Value : " + context.ReadValueAsButton());
    }
    void Item1_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("Item 1 Value : " + context.ReadValueAsButton());
    }
    void Item2_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("Item 2 Value : " + context.ReadValueAsButton());
    }
    void Item3_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("Item 3 Value : " + context.ReadValueAsButton());
    }
    void item4_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("Item 4 Value : " + context.ReadValueAsButton());
    }
    #endregion

    #endregion

    #region Player Basic Movement
    [Tooltip("Player Rigid Body")]
    [FoldoutGroup("Player Movement")][SerializeField] private Rigidbody rb;
    [Tooltip("Move speed of the character in m/s")]
    [FoldoutGroup("Player Movement")] public float MoveSpeed = 5.0f;
    [Tooltip("How fast the character turns to face movement direction")]
    [FoldoutGroup("Player Movement")] public float RotateOnMove = 360f;
    private void PlayerMove()
    {
        Vector2 inputmove = moveAction.ReadValue<Vector2>();
        Vector3 v3Move = new Vector3(inputmove.x, 0, inputmove.y);
        if (inputmove != Vector2.zero)
        {
            var relative = transform.position + v3Move.ToIso() - transform.position;
            var rot = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, RotateOnMove * Time.deltaTime);
        }
        //Move Position
        rb.MovePosition(transform.position + v3Move.magnitude * transform.forward * MoveSpeed * Time.deltaTime);
        if (_hasAnimator) _animator.SetFloat(_animIDSpeed, rb.velocity.magnitude);
    }

    #region Dodging
    [FoldoutGroup("Dodge")][SerializeField] float forceDodge;
    [FoldoutGroup("Dodge")][SerializeField] float timeDodge;
    WaitForSeconds wfsDodge;
    private void Dodging()
    {
        StartCoroutine(IeDodging());
    }
    IEnumerator IeDodging()
    {
        playerState = PlayerState.PlayerDodging;
        if (_hasAnimator) _animator.SetBool(_animIDDodge, true);
        rb.AddForce(transform.forward * forceDodge, ForceMode.Impulse);
        yield return wfsDodge;
        if (_hasAnimator) _animator.SetBool(_animIDDodge, false);
        rb.velocity = Vector3.zero;
        playerState = PlayerState.PlayerMoving;
    }
    #endregion

    #region Attacking
    [FoldoutGroup("Attack")][SerializeField] private AttackObject prefabAttack;
    [FoldoutGroup("Attack")][SerializeField] float timePrepareAttack;
    [FoldoutGroup("Attack")][SerializeField] float timeFinishAttack;
    [FoldoutGroup("Attack")][SerializeField] Transform transAttackPoint;
    [FoldoutGroup("Attack")][SerializeField] List<AttackObject> listAttack;
    WaitForSeconds wfsTimePrepareAttack, wfsTimeFinishAttack, wfsTimeTemporaryChangeAttack;
    public void OnChangeAttack(AttackObject atk, float timeprepare, float timefinish)
    {
        prefabAttack = atk;
        timePrepareAttack = timeprepare;
        wfsTimePrepareAttack = new WaitForSeconds(timePrepareAttack);
        timeFinishAttack = timefinish;
        wfsTimeFinishAttack = new WaitForSeconds(timeFinishAttack);
    }
    public void OnTemporaryChangeAttack(AttackObject atk, float timeprepare, float timefinish, float timetemporary)
    {
        wfsTimeTemporaryChangeAttack = new WaitForSeconds(timetemporary);
        StartCoroutine(IeOnTemporaryChangeAttack(atk, timeprepare, timefinish));
    }
    IEnumerator IeOnTemporaryChangeAttack(AttackObject atk, float timeprepare, float timefinish)
    {
        AttackObject baseAtk = prefabAttack;
        float baseTimePrepare = timePrepareAttack;
        float baseTimeFinish = timeFinishAttack;

        prefabAttack = atk;
        timePrepareAttack = timeprepare;
        wfsTimePrepareAttack = new WaitForSeconds(timePrepareAttack);
        timeFinishAttack = timefinish;
        wfsTimeFinishAttack = new WaitForSeconds(timeFinishAttack);

        yield return wfsTimeTemporaryChangeAttack;

        prefabAttack = baseAtk;
        timePrepareAttack = baseTimePrepare;
        wfsTimePrepareAttack = new WaitForSeconds(timePrepareAttack);
        timeFinishAttack = baseTimeFinish;
        wfsTimeFinishAttack = new WaitForSeconds(timeFinishAttack);
    }
    private void Attacking()
    {
        if (prefabAttack)
            StartCoroutine(IeAttacking());
    }
    IEnumerator IeAttacking()
    {
        playerState = PlayerState.PlayerAttack;
        Vector3 before = rb.velocity;
        rb.velocity = Vector3.zero;
        if (_hasAnimator) _animator.SetTrigger(_animIDAttack);
        yield return wfsTimePrepareAttack;
        int indexatk = -1;
        for (int i = 0; i < listAttack.Count; i++)
        {
            if (!listAttack[i].onActive && listAttack[i].IdAtkObject == prefabAttack.IdAtkObject)
            {
                indexatk = i;
                break;
            }
        }
        if (indexatk == -1)
        {
            GameObject atk = Instantiate(prefabAttack.gameObject, transAttackPoint.position, transAttackPoint.rotation);
            AttackObject atkobj = atk.GetComponent<AttackObject>();
            listAttack.Add(atkobj);
            indexatk = listAttack.Count - 1;
        }
        listAttack[indexatk].transform.position = transAttackPoint.position;
        listAttack[indexatk].transform.rotation = transAttackPoint.rotation;
        listAttack[indexatk].gameObject.SetActive(true);
        yield return wfsTimeFinishAttack;
        // rb.velocity = before;
        playerState = PlayerState.PlayerMoving;
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
        // set sphere position, with offset
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
                _animator.SetBool(_animIDJump, false);
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

    #endregion

    #region Animation
    private Animator _animator;
    private bool _hasAnimator;
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDDodge;
    private int _animIDAttack;
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
    }
    #endregion

    #region Player Sfx
    [FoldoutGroup("Player SFX")] public AudioClip LandingAudioClip;
    [FoldoutGroup("Player SFX")] public AudioClip[] FootstepAudioClips;
    [FoldoutGroup("Player SFX")][Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
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
}
