using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WormBoss : BaseBoss<WormBoss, WORMSTATE>
{
    [Header("Atribute")]
    public float m_AngularSpeed;
    public int currentAttackIndex = 0;
    private Vector3 center;
    private Vector3 size;

    [Header("Detection & Timing")]
    public float detectionRange = 10f;
    public float undergroundDuration = 3f;

    [Header("Attack Settings")]
    public List<WormAttackData> wormAttackDatas = new();

    [Header("Animations")]
    public Animator Animator => animator;
    public string undergroundAnimation = "GroundDiveIn";
    public string emergeAnimation = "GroundBreakThrough";

    [Header("References")]
    public string[] listStringRefer;
    public GameObject m_Player;
    public NavMeshAgent NavMeshAgent => m_NavmeshAgent;

    private void Awake()
    {
        m_NavmeshSurface = GameObject.Find(listStringRefer[0]).GetComponent<NavMeshSurface>();
        m_Player = GameObject.Find(listStringRefer[1]);
        animator = GetComponent<Animator>();
        m_NavmeshAgent = GetComponent<NavMeshAgent>();
    }
    protected override void Start()
    {

        finiteSM = new FSM<WormBoss, WORMSTATE>();
        // Kh?i t?o state ban ??u là Idle
        finiteSM.ChangeState(new WormIdleState(this, finiteSM));
        if (m_NavmeshSurface != null)
        {
            size = m_NavmeshSurface.size;
            center = m_NavmeshSurface.transform.position + m_NavmeshSurface.center;
        }
    }
    protected override void Update()
    {
        finiteSM?.Update();
    }
    public override void RequestStateTransition(WORMSTATE requestedState)
    {
        switch (requestedState)
        {
            case WORMSTATE.IDLE:
                finiteSM.ChangeState(new WormIdleState(this, finiteSM));
                break;
            case WORMSTATE.WALK:
                finiteSM.ChangeState(new WormWalkState(this, finiteSM));
                break;
            case WORMSTATE.UNDERGROUND:
                finiteSM.ChangeState(new WormUndergroundState(this, finiteSM));
                break;
            case WORMSTATE.EMERGE:
                finiteSM.ChangeState(new WormEmergeState(this, finiteSM));
                break;
            case WORMSTATE.DETEC:
                finiteSM.ChangeState(new WormDetecState(this, finiteSM));
                break;
            case WORMSTATE.ATTACK:
                finiteSM.ChangeState(new WormAttackState(this, finiteSM));
                break;
            default:
                break;
        }
    }

    // Ph??ng th?c l?y v? trí ng?u nhiên trên NavMesh cách player ít nh?t minDistance
    public Vector3 GetRandomEmergencePosition()
    {
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = center.y;
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
        return new Vector3(x, y, z);
    }

    public void MoveToRandomPosition()
    {
        Vector3 randomPoint = GetRandomEmergencePosition();
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
        {
            m_NavmeshAgent.stoppingDistance = 0;
            m_NavmeshAgent.Warp(hit.position);
        }
        else
        {
            Debug.Log("ko tim thay duong di");
        }
    }

    public void ChangeBeforeState(WORMSTATE wormState)
    {
        beforeState = wormState;
    }
    public bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, m_Player.transform.position) <= detectionRange;
    }
    public Vector3 DistanceToPlayerNormalized()
    {
        return (m_Player.transform.position - transform.position).normalized;
    }
    public void Rotation()
    {
        Vector3 directionToPlayer = DistanceToPlayerNormalized();
        directionToPlayer.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        float rotationSpeed = m_AngularSpeed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wormAttackDatas[currentAttackIndex].stopDistance);

    }
}
