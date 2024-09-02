using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    private InputAction interactAction;
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
        interactAction = playerInput.actions["Interact"];
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
        interactAction.performed += Interact_Performed;
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
    void Interact_Performed(InputAction.CallbackContext context)
    {
        Debug.Log("Interacting Value : " + context.ReadValueAsButton());
        Interacting();
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
        Vector3 movement = transform.forward * moveDirection.magnitude * MoveSpeed * Time.deltaTime;
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
    [FoldoutGroup("Attack")] private AttackObject currentAttack;
    [FoldoutGroup("Attack")][SerializeField] private float timePrepareAttack;
    [FoldoutGroup("Attack")][SerializeField] private float timeFinishAttack;
    [FoldoutGroup("Attack")][SerializeField] private Transform transAttackPoint;
    [FoldoutGroup("Attack")] private List<AttackObject> listAttack = new List<AttackObject>();

    private WaitForSeconds wfsTimePrepareAttack;
    private WaitForSeconds wfsTimeFinishAttack;
    private Coroutine corouTempAttack;

    public void OnChangeAttack(AttackObject atk, float prepareTime, float finishTime)
    {
        SetAttack(atk, prepareTime, finishTime);
    }

    public void OnTemporaryChangeAttack(AttackObject atk, float prepareTime, float finishTime, float temporaryTime)
    {
        if (corouTempAttack != null)
        {
            StopCoroutine(corouTempAttack);
        }
        corouTempAttack = StartCoroutine(TemporaryChangeAttack(atk, prepareTime, finishTime, temporaryTime));
    }

    private IEnumerator TemporaryChangeAttack(AttackObject atk, float prepareTime, float finishTime, float temporaryTime)
    {
        AttackObject originalAttack = prefabAttack;
        float originalPrepareTime = timePrepareAttack;
        float originalFinishTime = timeFinishAttack;

        SetAttack(atk, prepareTime, finishTime);

        yield return new WaitForSeconds(temporaryTime);

        SetAttack(originalAttack, originalPrepareTime, originalFinishTime);
    }

    private void SetAttack(AttackObject atk, float prepareTime, float finishTime)
    {
        prefabAttack = atk;
        currentAttack = atk;
        timePrepareAttack = prepareTime;
        timeFinishAttack = finishTime;
        wfsTimePrepareAttack = new WaitForSeconds(timePrepareAttack);
        wfsTimeFinishAttack = new WaitForSeconds(timeFinishAttack);
    }

    private void Attacking()
    {
        if (currentAttack != null)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
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

        attack.transform.SetPositionAndRotation(transAttackPoint.position, transAttackPoint.rotation);
        attack.gameObject.SetActive(true);

        yield return wfsTimeFinishAttack;

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
        RaycastHit[] hits = Physics.SphereCastAll(transform.position + interactOffsetPos, interactRadius, transform.forward, interactRadius, layerInteractable);
        if (hits.Length > 0)
        {
            IInteractable interactable = hits[0].collider.GetComponent<IInteractable>();
            Debug.Log(hits[0].collider.name);
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transAttackPoint.position + interactOffsetPos, transAttackPoint.position + transform.forward * interactRadius);
        Gizmos.DrawWireSphere(transAttackPoint.position + interactOffsetPos + transform.forward * interactRadius, interactDistance);
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
