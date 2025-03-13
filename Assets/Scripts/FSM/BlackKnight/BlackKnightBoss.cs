using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;
using System.Collections.Generic;

public class BlackKnightBoss : BaseBoss<BlackKnightBoss,ENEMYSTATE>
{
    public int currentAttackIndex = 0;
    public bool m_IsMiniBossAttacked = false;

    [SerializeField] private float m_Range;
    [SerializeField] private float m_AngularSpeed;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private float m_RunSpeed;
    [SerializeField] private float m_LateralDistance;
    [SerializeField] private bool m_HasChosenLeftOrRight = false;

    private Vector3 center;
    private Vector3 size;

    public GameObject m_Player;
    public List<string> AttackList = new List<string>();

    public string NameNavmeshOfBoss;

    #region Zone Property
    public float RunSpeed => m_RunSpeed;
    public float StopDistance => stopDistance;
    public Animator Animator => animator;
    public NavMeshAgent NavmeshAgent => m_NavmeshAgent;
    public ENEMYSTATE BeforState
    {
        get => beforeState;
        set => beforeState = value;
    }
    public ENEMYSTATE CurrentState
    {
        get => currentState;
        set => currentState = value;
    }
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        m_NavmeshAgent = GetComponent<NavMeshAgent>();
        m_Player = GameObject.Find("Player");
        m_NavmeshSurface = GameObject.Find(NameNavmeshOfBoss).GetComponent<NavMeshSurface>();
    }

    protected override  void Start()
    {
        finiteSM = new FSM<BlackKnightBoss, ENEMYSTATE>();
        finiteSM.ChangeState(new IdleState(this, finiteSM));

        if (m_NavmeshSurface != null)
        {
            size = m_NavmeshSurface.size;
            center = m_NavmeshSurface.transform.position + m_NavmeshSurface.center;
        }
    }


    protected override void Update()
    {
        finiteSM.Update();
    }

    public override void RequestStateTransition(ENEMYSTATE requestedState)
    {
        switch (requestedState)
        {
            case ENEMYSTATE.IDLE:
                finiteSM.ChangeState(new IdleState(this, finiteSM));
                break;

            case ENEMYSTATE.IDLESTAND:
                finiteSM.ChangeState(new IdleStandState(this, finiteSM));
                break;

            case ENEMYSTATE.WALK:
                
                finiteSM.ChangeState(new WalkState(this, finiteSM));
                break;

            case ENEMYSTATE.DETEC:
                finiteSM.ChangeState(new DetecState(this, finiteSM));
                break;

            case ENEMYSTATE.ATTACK:
                finiteSM.ChangeState(new AttackState(this, finiteSM));
                break;

            case ENEMYSTATE.RUN:
                finiteSM.ChangeState(new RunState(this, finiteSM));
                break;

            default:
                break;
        }
    }

    public void ResetLateralChoice()
    {
        m_HasChosenLeftOrRight = false;
    }

    public IEnumerator DelayBossMoveLeftOrRighto()
    {
        yield return new WaitForSeconds(2f);
        if (currentState != ENEMYSTATE.DETEC)
            yield break;
        if (!m_HasChosenLeftOrRight)
        {
            BossMoveLeftOrRighto();
            m_HasChosenLeftOrRight = true;
        }
    }

    public void BossMoveLeftOrRighto()
    {
        int randomSide = Random.Range(1, 3);
        animator.SetInteger("Random", randomSide);
        Vector3 lateralDirection = (randomSide == 1) ? transform.right : -transform.right;
        Vector3 targetPos = transform.position + lateralDirection * m_LateralDistance;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 5f, NavMesh.AllAreas))
        {
            NavmeshAgent.isStopped = false;
            NavmeshAgent.stoppingDistance = 0;
            NavmeshAgent.SetDestination(hit.position);
        }
    }

    public Vector3 GetRandomPointInVolume()
    {
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = center.y;
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
        return new Vector3(x, y, z);
    }

    public Vector3 DistanceNormalized()
    {
        return (m_Player.transform.position - transform.position).normalized;
    }

    public float Distance()
    {
        return Vector3.Distance(m_Player.transform.position, transform.position);
    }

    public bool IsMoveWayPoint()
    {
        return (!m_NavmeshAgent.pathPending &&
                m_NavmeshAgent.remainingDistance <= m_NavmeshAgent.stoppingDistance &&
                m_NavmeshAgent.velocity.sqrMagnitude < 0.1f);
    }

    public bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, m_Player.transform.position) <= m_Range;
    }

    public bool PlayerInStopRange()
    {
        return Vector3.Distance(transform.position, m_Player.transform.position) <= stopDistance;
    }

    public void MoveToPlayer()
    {
        m_NavmeshAgent?.SetDestination(m_Player.transform.position);
    }

    public void MoveToRandomPosition()
    {
        Vector3 randomPoint = GetRandomPointInVolume();
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
        {
            m_NavmeshAgent.stoppingDistance = 0;
            m_NavmeshAgent.SetDestination(hit.position);
        }else
        {
            Debug.Log("ko tim thay duong di");
        }    
    }

    public void Rotation()
    {
        Vector3 directionToPlayer = DistanceNormalized();
        directionToPlayer.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        float rotationSpeed = m_AngularSpeed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void SetDestinationToPlayerPosition(bool lockRotation)
    {
        Vector3 targetPos = m_Player.transform.position;
        m_NavmeshAgent.SetDestination(targetPos);

        if (lockRotation)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }
    }

    public IEnumerator PlaySingleAttackAnimation()
    {
        string attackAnim = AttackList[currentAttackIndex];
        animator.CrossFade(attackAnim, 0.2f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(attackAnim));
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        m_IsMiniBossAttacked = true;
        currentAttackIndex = (currentAttackIndex + 1) % AttackList.Count;
    }

    public IEnumerator WaitFinishAttack()
    {
        Debug.Log($"{AttackList[currentAttackIndex]} : {animator.GetCurrentAnimatorStateInfo(0).normalizedTime}");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(AttackList[currentAttackIndex]));
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        RequestStateTransition(ENEMYSTATE.RUN);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_Range);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
