using JetBrains.Annotations;
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
        IDLESTAND,
        WALK,
        ATTACK,
        DETEC,
        RUN,

    }
   
    public ENEMYSTATE currentState;
    public ENEMYSTATE beforState;
    public int currentAttackIndex = 0;
    public bool m_IsMiniBossAttacked = false;

    private FSM finiteSM;
    [SerializeField] private float m_Range;
    [SerializeField] private float m_AngularSpeed;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private float m_RunSpeed;
    [SerializeField] private float m_LateralDistance;

    [SerializeField] private bool m_HasChosenLeftOrRight =false;
    

    private Vector3 center;
    private Vector3 size;

    [SerializeField] private NavMeshSurface surface;
    [SerializeField] private Animator animator;
    public GameObject m_Player;
    [SerializeField] private NavMeshAgent m_NavmeshAgent;

    public List<string> AttackList = new List<string>();

    #region Zone Property
    
    public float RunSpeed => m_RunSpeed;
    public float StopDistance => stopDistance;
    public Animator Animator => animator;
    public NavMeshAgent NavmeshAgent => m_NavmeshAgent;
    #endregion

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

            case ENEMYSTATE.IDLESTAND:
                finiteSM.ChangeState(new IdleStandState(this, finiteSM));
                break;

            case ENEMYSTATE.WALK:
                // Trước khi chuyển sang Walk, lấy điểm ngẫu nhiên
                //MoveToRandomPosition();
                finiteSM.ChangeState(new WalkState(this, finiteSM));
                break;

            // Nếu có các state khác (như RUN, ATTACK, DETEC) thì thêm logic chuyển state tại đây.
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
        {
            yield break;
        }
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
        Vector3 lateralDirection = (randomSide == 1) ? this.transform.right : -this.transform.right;
        Vector3 targetPos = this.transform.position + lateralDirection * m_LateralDistance;
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
        return (Vector3.Distance(transform.position, m_Player.transform.position) <= m_Range);
    }

    public bool PlayerInStopRange()
    {
        return (Vector3.Distance(transform.position, m_Player.transform.position) <= stopDistance);
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
        currentState = newState;
    }

    public void SetDestinationToPlayerPosition(bool lockRotation)
    {
        // Lấy vị trí hiện tại của player
        Vector3 targetPos = m_Player.transform.position;
        // Set destination cho NavMeshAgent
        m_NavmeshAgent.SetDestination(targetPos);

        // Nếu yêu cầu khóa hướng, thì xoay boss về phía player
        if (lockRotation)
        {
            // Không cần thay đổi updatePosition; chỉ xử lý rotation
            Vector3 direction = (targetPos - transform.position).normalized;
            direction.y = 0; // đảm bảo xoay theo trục ngang
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }
    }


    public IEnumerator PlaySingleAttackAnimation()
    {
        // Lấy tên animation cần chạy dựa trên chỉ số hiện tại
        string attackAnim = AttackList[currentAttackIndex];

        // Chuyển đổi sang animation với crossFade (hoặc theo cách bạn đã làm)
        animator.CrossFade(attackAnim, 0.2f);

        // Đợi cho đến khi animation bắt đầu chạy
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(attackAnim));

        // Đợi hết thời gian của animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        m_IsMiniBossAttacked = true;
        // Sau khi hoàn thành, cập nhật chỉ số cho lần tấn công kế tiếp (lật qua lại giữa 0 và 1)
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
        Gizmos.DrawWireSphere(this.transform.position, m_Range);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, stopDistance);
    } // hàm vẽ zone

    

}
