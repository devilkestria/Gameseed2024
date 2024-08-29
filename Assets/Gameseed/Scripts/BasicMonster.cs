using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class BasicMonster : MonoBehaviour, IDamageable
{
    [FoldoutGroup("Basic Monster")][SerializeField] private EnemyState state;
    [FoldoutGroup("Basic Monster")][SerializeField] private Rigidbody rb;
    [FoldoutGroup("Basic Monster")] private Transform target;
    [FoldoutGroup("Basic Monster")][SerializeField] private NavMeshAgent agent;
    [FoldoutGroup("Basic Monster")] float timeOnDamage = 0.25f;
    [FoldoutGroup("Basic Monster")] WaitForSeconds wfsTimeOnDamage;
    [FoldoutGroup("Basic Monster")][Range(0.001f, 0.1f)][SerializeField] private float stillTreshold = 0.05f;
    [FoldoutGroup("Basic Monster")][SerializeField] float timeIddle = 5f;
    [FoldoutGroup("Basic Monster")] float deltatimeIddle;
    private void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!rb) rb = GetComponent<Rigidbody>();
        originPosition = transform.position;
        wfsTimePrepareAttack = new WaitForSeconds(timePrepareAttack);
        wfsTimeFinishAttack = new WaitForSeconds(timeFinishAttack);
        wfsTimeOnDamage = new WaitForSeconds(timeFinishAttack);
    }
    private void Start()
    {
        if (!target) target = GameplayManager.instance.playerObj.transform;
        _hasAnimator = TryGetComponent(out _animator);
    }
    private void Update()
    {
        distanceTarget = Vector3.Distance(target.position, transform.position);
        distanceOrigin = Vector3.Distance(originPosition, transform.position);
        distancePatrol = Vector3.Distance(transform.position, patrolPos);
        if (_hasAnimator) _animator.SetFloat(_animIDHurt, rb.velocity.magnitude);
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
    public void Damage(AttackObject atkobj)
    {
        StopAllCoroutines();
        corouAttack = null;
        state = EnemyState.EnemyOnDamage;
        StartCoroutine(IeDamage(atkobj));
    }
    IEnumerator IeDamage(AttackObject atkobj)
    {
        yield return null;
        agent.enabled = false;
        rb.useGravity = true;
        rb.isKinematic = false;

        if (_hasAnimator) _animator.SetTrigger(_animIDHurt);
        if (atkobj.forcePush > 0)
        {
            Debug.Log("Pushed " + name);
            rb.AddForce(atkobj.forcePush * atkobj.transform.forward, ForceMode.Impulse);
        }

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.magnitude < stillTreshold);
        yield return wfsTimeOnDamage;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        agent.Warp(transform.position);
        agent.enabled = true;

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
    private Animator _animator;
    private bool _hasAnimator;
    private int _animIDSpeed;
    private int _animIDAttack;
    private int _animIDHurt;
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDHurt = Animator.StringToHash("Hurt");
    }
    #endregion
}
