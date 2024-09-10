using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BasicMonster : MonoBehaviour, IDamageable, IDeathable
{
    [FoldoutGroup("Basic Monster")][SerializeField] private Status status;
    [FoldoutGroup("Basic Monster")][SerializeField] private EnemyState state;
    [FoldoutGroup("Basic Monster")][SerializeField] private Rigidbody rb;
    [FoldoutGroup("Basic Monster")][SerializeField] private Collider collider;
    [FoldoutGroup("Basic Monster")] public int IndexMonster;
    [FoldoutGroup("Basic Monster")] public bool isActive;
    [FoldoutGroup("Basic Monster")] private Transform target;
    [FoldoutGroup("Basic Monster")][SerializeField] private NavMeshAgent agent;
    [FoldoutGroup("Basic Monster")][SerializeField] private AudioSource sfxAudioSource;
    [FoldoutGroup("Basic Monster")][SerializeField] float timeIddle = 5f;
    [FoldoutGroup("Basic Monster")] float deltatimeIddle;
    [FoldoutGroup("Basic Monster/Damage")][SerializeField] float timeDurationDamage = 0.25f;
    [FoldoutGroup("Basic Monster/Damage")] WaitForSeconds wfsTimeDurationDamage;
    [FoldoutGroup("Basic Monster/Damage")][Range(0.001f, 0.1f)][SerializeField] private float stillTreshold = 0.05f;
    private void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!collider) collider = GetComponent<Collider>();
        if (!status) status = GetComponent<Status>();
        if (!_animator) _animator = GetComponentInChildren<Animator>();
        _hasAnimator = _animator != null;
        originPosition = transform.position;
        wfsTimePrepareAttack = new WaitForSeconds(timePrepareAttack);
        wfsTimeFinishAttack = new WaitForSeconds(timeFinishAttack);
        wfsTimeDurationDamage = new WaitForSeconds(timeDurationDamage);
        wfsTimeDurationDeath = new WaitForSeconds(timeDurationDeath);
    }
    private void Start()
    {
        if (!target) target = GameplayManager.instance.playerObj.transform;
        AssignAnimationIDs();
    }
    private void Update()
    {
        distanceTarget = Vector3.Distance(target.position, transform.position);
        distanceOrigin = Vector3.Distance(originPosition, transform.position);
        distancePatrol = Vector3.Distance(transform.position, patrolPos);
        CheckState();
    }
    public virtual void CheckState()
    {
        switch (state)
        {
            case EnemyState.EnemyWaiting:
                WaitingCommand();
                break;
            case EnemyState.EnemyChasePlayer:
                ChasingTarget();
                break;
            case EnemyState.EnemyAttack:
                Attacking();
                break;
            case EnemyState.EnemyBackToOriginPos:
                BackToOriginPos();
                break;
            case EnemyState.EnemyPatrol:
                Patroling();
                break;
        }
    }
    public virtual void WaitingCommand()
    {
        if (_hasAnimator) _animator.SetBool(_animIDWalk, false);
        if (distanceTarget <= attackRadius)
        {
            deltatimeIddle = 0;
            state = EnemyState.EnemyAttack;
            return;
        }
        if (canChaseTarget && distanceTarget <= ChaseRadius)
        {
            deltatimeIddle = 0;
            state = EnemyState.EnemyChasePlayer;
            return;
        }
        if (canPatrol)
        {
            deltatimeIddle += Time.deltaTime;
            if (deltatimeIddle > timeIddle)
            {
                deltatimeIddle = 0;
                createPatrol = true;
                state = EnemyState.EnemyPatrol;
                return;
            }
        }
    }

    #region Chase Target
    [FoldoutGroup("Basic Monster/Chase Target")] float distanceTarget;
    [FoldoutGroup("Basic Monster/Chase Target")][SerializeField] private bool canChaseTarget;
    [FoldoutGroup("Basic Monster/Chase Target")][SerializeField] private float ChaseRadius;
    public virtual void ChasingTarget()
    {
        if (_hasAnimator) _animator.SetBool(_animIDWalk, true);
        agent.SetDestination(target.position);
        if (distanceTarget <= attackRadius)
        {
            state = EnemyState.EnemyAttack;
            return;
        }
        if (distanceTarget > ChaseRadius)
        {
            state = EnemyState.EnemyBackToOriginPos;
            return;
        }
    }
    #endregion

    #region Attack
    [FoldoutGroup("Basic Monster/Attack")][SerializeField] private float attackRadius;
    [FoldoutGroup("Basic Monster/Attack")][SerializeField] private float timePrepareAttack;
    [FoldoutGroup("Basic Monster/Attack")] WaitForSeconds wfsTimePrepareAttack;
    [FoldoutGroup("Basic Monster/Attack")][SerializeField] private float timeFinishAttack;
    [FoldoutGroup("Basic Monster/Attack")] WaitForSeconds wfsTimeFinishAttack;
    [FoldoutGroup("Basic Monster/Attack")][SerializeField] private AttackObject prefabAttack;
    [FoldoutGroup("Basic Monster/Attack")][SerializeField] List<AudioClip> listSfxAttack;
    [FoldoutGroup("Basic Monster/Attack")][SerializeField] private Transform transAttackPoint;
    [FoldoutGroup("Basic Monster/Attack")] List<AttackObject> listAttack = new List<AttackObject>();
    [FoldoutGroup("Basic Monster/Attack")] Coroutine corouAttack;
    [FoldoutGroup("Basic Monster/Attack")][SerializeField] private float speedLook;
    [FoldoutGroup("Basic Monster/Attack")][SerializeField] private float minRotToAtk;
    public virtual void Attacking()
    {
        if (prefabAttack && corouAttack == null)
        {
            FaceTarget();
            corouAttack = StartCoroutine(IeAttacking());
        }
    }
    IEnumerator IeAttacking()
    {
        if (_hasAnimator) _animator.SetBool(_animIDWalk, false);
        agent.enabled = false;
        // Harus Muter Menghadap player

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookrotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        float angle = Quaternion.Angle(lookrotation, transform.rotation);
        while (angle > minRotToAtk)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookrotation, Time.deltaTime * speedLook);
            angle = Quaternion.Angle(lookrotation, transform.rotation);
            // transform.rotation = Quaternion.Slerp(transform.rotation, lookrotation, deltatimeRot / 1);
            yield return null;
        }

        if (_hasAnimator) _animator.SetTrigger(_animIDAttack);
        yield return wfsTimePrepareAttack;
        int indexatk = -1;
        for (int i = 0; i < listAttack.Count; i++)
        {
            if (!listAttack[i].onActive && listAttack[i].nameAtk == prefabAttack.nameAtk)
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
        int random = Random.Range(0, listSfxAttack.Count);
        sfxAudioSource.PlayOneShot(listSfxAttack[random]);
        listAttack[indexatk].transform.position = transAttackPoint.position;
        listAttack[indexatk].transform.rotation = transAttackPoint.rotation;
        listAttack[indexatk].gameObject.SetActive(true);
        yield return wfsTimeFinishAttack;
        agent.enabled = true;
        corouAttack = null;
        if (distanceTarget > attackRadius)
        {
            if (distanceTarget < ChaseRadius) state = EnemyState.EnemyChasePlayer;
            else state = EnemyState.EnemyBackToOriginPos;
        }
    }
    #endregion

    #region  Back To Origin Pos
    [FoldoutGroup("Basic Monster/Back Origin Pos")] private Vector3 originPosition;
    [FoldoutGroup("Basic Monster/Back Origin Pos")] float distanceOrigin;
    [FoldoutGroup("Basic Monster/Back Origin Pos")][SerializeField] private float minOriginRadius;
    public virtual void BackToOriginPos()
    {
        if (_hasAnimator) _animator.SetBool(_animIDWalk, true);
        agent.SetDestination(originPosition);
        if (distanceTarget <= attackRadius)
        {
            state = EnemyState.EnemyAttack;
            return;
        }
        if (distanceTarget < ChaseRadius)
        {
            state = EnemyState.EnemyChasePlayer;
            return;
        }
        if (distanceOrigin < minOriginRadius)
        {
            deltatimeIddle = 0;
            state = EnemyState.EnemyWaiting;
            return;
        }
    }
    #endregion

    #region Patrol
    [FoldoutGroup("Basic Monster/Patrol")][SerializeField] private bool canPatrol;
    [FoldoutGroup("Basic Monster/Patrol")] private float distancePatrol;
    [FoldoutGroup("Basic Monster/Patrol")][SerializeField] private float patrolRadius;
    [FoldoutGroup("Basic Monster/Patrol")] private Vector3 patrolPos;
    [FoldoutGroup("Basic Monster/Patrol")] private bool createPatrol;
    public virtual void Patroling()
    {
        if (createPatrol)
        {
            bool canreach = GetRandomPosInRadius(out patrolPos);
            while (!canreach) canreach = GetRandomPosInRadius(out patrolPos);
            createPatrol = false;
        }
        if (_hasAnimator) _animator.SetBool(_animIDWalk, true);
        agent.SetDestination(patrolPos);
        if (distanceTarget <= attackRadius)
        {
            deltatimeIddle = 0;
            state = EnemyState.EnemyAttack;
            return;
        }
        if (distanceTarget <= ChaseRadius)
        {
            deltatimeIddle = 0;
            state = EnemyState.EnemyChasePlayer;
            return;
        }
        if (distancePatrol <= agent.stoppingDistance || distancePatrol <= 0.1)
        {
            if (_hasAnimator) _animator.SetBool(_animIDWalk, false);
            deltatimeIddle += Time.deltaTime;
            if (deltatimeIddle > timeIddle)
            {
                deltatimeIddle = 0;
                state = EnemyState.EnemyBackToOriginPos;
            }
        }
    }
    bool GetRandomPosInRadius(out Vector3 pos)
    {
        bool canreach = false;
        var path = new NavMeshPath();
        Vector2 randomdir = Random.insideUnitCircle * patrolRadius;
        Vector3 randompos = new Vector3(randomdir.x, 0, randomdir.y) + transform.position;
        pos = randompos;
        return canreach = agent.CalculatePath(randompos, path);
    }
    #endregion
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookrotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookrotation, Time.deltaTime * speedLook);
    }

    #region Damage
    public void Damage(AttackObject atkobj, AudioClip clip)
    {
        StopAllCoroutines();
        corouAttack = null;
        state = EnemyState.EnemyOnDamage;
        StartCoroutine(IeDamage(atkobj, clip));
    }
    IEnumerator IeDamage(AttackObject atkobj, AudioClip clip)
    {
        if (_hasAnimator) _animator.SetBool(_animIDWalk, false);
        yield return null;
        agent.enabled = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        sfxAudioSource.PlayOneShot(clip);
        if (_hasAnimator) _animator.SetTrigger(_animIDHurt);
        if (atkobj.forcePush > 0)
            rb.AddForce(atkobj.forcePush * atkobj.transform.forward, ForceMode.Impulse);

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.magnitude < stillTreshold);
        yield return wfsTimeDurationDamage;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        agent.Warp(transform.position);
        agent.enabled = true;

        state = EnemyState.EnemyWaiting;
    }
    #endregion

    #region Death
    [FoldoutGroup("Basic Monster/Death")][SerializeField] private float timeDurationDeath;
    [FoldoutGroup("Basic Monster/Death")] private WaitForSeconds wfsTimeDurationDeath;
    public void Death()
    {
        StopAllCoroutines();
        corouAttack = null;
        state = EnemyState.EnemyDeath;
        StartCoroutine(IeDeath());
    }
    IEnumerator IeDeath()
    {
        if (_hasAnimator) _animator.SetBool(_animIDWalk, false);
        yield return null;
        rb.isKinematic = true;
        rb.useGravity = false;
        collider.enabled = false;
        agent.enabled = false;
        isActive = false;

        if (_hasAnimator) _animator.SetTrigger(_animIDDeath);
        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.magnitude < stillTreshold);
        yield return wfsTimeDurationDeath;

        if (GameplayManager.instance.listMonster.Contains(this))
            GameplayManager.instance.listMonster.Add(this);
        gameObject.SetActive(false);
    }
    #endregion

    #region Spawn
    [FoldoutGroup("Basic Monster/Spawn")] private float timeDurationSpawn;
    [FoldoutGroup("Basic Monster/Spawn")] private WaitForSeconds wfsTimeDurationSpawn;
    public void SpawnMonster(Vector3 pos, Quaternion rot)
    {
        gameObject.SetActive(true);
        transform.position = pos;
        transform.rotation = rot;
        StartCoroutine(IeSpawnMonster());
    }
    IEnumerator IeSpawnMonster()
    {
        if (_hasAnimator) _animator.SetTrigger(_animIDSpawn);
        yield return wfsTimeDurationSpawn;
        collider.enabled = true;
        agent.enabled = true;
        rb.useGravity = false;
        rb.isKinematic = false;
        isActive = true;
        status.ResetHealth();
        state = EnemyState.EnemyWaiting;
    }
    #endregion
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ChaseRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(originPosition, minOriginRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }

    #region Animation
    [FoldoutGroup("Basic Monster/Animation")][SerializeField] private Animator _animator;
    private bool _hasAnimator;
    private int _animIDWalk;
    private int _animIDAttack;
    private int _animIDHurt;
    private int _animIDDeath;
    private int _animIDSpawn;
    private void AssignAnimationIDs()
    {
        _animIDWalk = Animator.StringToHash("Walk");
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDHurt = Animator.StringToHash("Hit");
        _animIDDeath = Animator.StringToHash("Death");
        _animIDSpawn = Animator.StringToHash("Spawn");
    }
    #endregion
}
