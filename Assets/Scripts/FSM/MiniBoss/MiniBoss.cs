using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MiniBoss : MonoBehaviour
{
    public enum ENEMYSTATE
    {
        DEFAULT = 0,
        IDLE,
        WALK,
        ATTACK,
        DETEC,
        RUN,
    }
    private FSM finiteSM;
    public ENEMYSTATE state;
    [SerializeField] private float m_Range;
    [SerializeField] private float m_Distance;
    [SerializeField] private float m_AngularSpeed;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private float m_RunSpeed;
    [SerializeField] private float m_LateralDistance;

    [SerializeField] private bool m_HasChosenLeftOrRight;
    private Vector3 center;
    private Vector3 size;

    [SerializeField] private NavMeshSurface surface;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject m_Player;
    [SerializeField] private NavMeshAgent m_NavmeshAgent;

    [SerializeField] private List<string> AttackList = new List<string>();

    public Animator Animator => animator;
    public NavMeshAgent NavmeshAgent => m_NavmeshAgent;
    public float Range => m_Range;
    public float RunSpeed => m_RunSpeed;
    public float Distacne => m_Distance;
    public float StopDistance => stopDistance;
    public GameObject Player => m_Player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        m_NavmeshAgent = GetComponent<NavMeshAgent>();
        m_Player = GameObject.Find("Player");
    }

    void Start()
    {
        finiteSM = new FSM();
        finiteSM.ChangeState(new IdleState(this, finiteSM));

        size = surface.size;
        center = surface.transform.position + surface.center;
    }

    void Update()
    {
        finiteSM.Update();
    }
    public void RequestStateTransition(ENEMYSTATE requestedState) // Siêu cấp quan trọng 
    {
        switch (requestedState)
        {
            case ENEMYSTATE.IDLE:
                finiteSM.ChangeState(new IdleState(this, finiteSM));
                break;
            case ENEMYSTATE.WALK:
                // Trước khi chuyển sang Walk, lấy điểm ngẫu nhiên
                MoveToRandomPosition();
                finiteSM.ChangeState(new WalkState(this, finiteSM));
                break;
            // Nếu có các state khác (như RUN, ATTACK, DETEC) thì thêm logic chuyển state tại đây.
            case ENEMYSTATE.DETEC:
                if (PlayerInRange())
                {
                    finiteSM.ChangeState(new DetecState(this, finiteSM));
                }
                break;
            case ENEMYSTATE.ATTACK:
                finiteSM.ChangeState(new AttackState(this, finiteSM));
                break;
            case ENEMYSTATE.RUN:
                if (IsMoveWayPoint())
                {
                    finiteSM.ChangeState(new RunState(this, finiteSM));
                }
                break;
            default:
                break;
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
        return (Vector3.Distance(transform.position, m_Player.transform.position) <= m_Range);
    }

    public bool PlayerInStopRange()
    {
        return (Vector3.Distance(transform.position, m_Player.transform.position) <= m_NavmeshAgent.stoppingDistance);
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
        }
    }
    public void Rotation()
    {
        Vector3 directionToPlayer = DistanceNormalized();
        directionToPlayer.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        float rotationSpeed = m_AngularSpeed; // tốc độ góc tối đa (độ/giây hoặc radian/giây)
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void ChangeStateCurrent(ENEMYSTATE newState)
    {
        state = newState;
    }

    public void BossMoveLeftOrRight()
    {
        //Rotation();

        if (PlayerInStopRange())
        {
            RequestStateTransition(ENEMYSTATE.ATTACK);
        }
        else
        {
            StartCoroutine(DelayBossMoveLeftOrRighto());
        }

        if (IsMoveWayPoint())
        {
            RequestStateTransition(ENEMYSTATE.RUN);
        }
    }
    public void Attack()
    {
        for (int i = 0; i < AttackList.Count; i++)
        {
            int Index = i % AttackList.Count;
            animator.Play(AttackList[Index]);
        }
    }


    private IEnumerator DelayBossMoveLeftOrRighto()
    {
        yield return new WaitForSeconds(2f);
        if (!m_HasChosenLeftOrRight)
        {
            int randomSide = Random.Range(1, 3);
            animator.SetInteger("Random", 1);
            Vector3 lateralDirection = (randomSide == 1) ? this.transform.right : -this.transform.right;
            Vector3 targetPos = this.transform.position + lateralDirection * m_LateralDistance;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPos, out hit, 5f, NavMesh.AllAreas))
            {
                NavmeshAgent.isStopped = false;
                NavmeshAgent.stoppingDistance = 0;
                NavmeshAgent.SetDestination(hit.position);
            }
            m_HasChosenLeftOrRight = true;
        }
    }

    // Phương thức tập trung xử lý chuyển state dựa trên yêu cầu từ các state
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, m_Range);
    }

}
