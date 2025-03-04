using System.Collections;
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
    private FSM fSM;
    public ENEMYSTATE state;
    [SerializeField] private float m_Range;
    [SerializeField] private float m_Distacne;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private float m_RunSpeed;
    private Vector3 center;
    private Vector3 size;

    [SerializeField] private NavMeshSurface surface;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject m_Player;
    [SerializeField] private NavMeshAgent m_NavmeshAgent;

    public Animator Animator => animator;
    public NavMeshAgent NavmeshAgent => m_NavmeshAgent;
    public float Range => m_Range;
    public float RunSpeed => m_RunSpeed;    

    public float Distacne => m_Distacne;
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
        fSM = new FSM();
        fSM.ChangeState(new IdleState(this, fSM));
        size = surface.size;
        center = surface.transform.position + surface.center;
    }

    // Update is called once per frame
    void Update()
    {
        fSM.Update();
    }

    public bool PlayerInRange(float range)
    {
        return (Vector3.Distance(this.transform.position, m_Player.transform.position) <= range);
    }
    public void MoveToPlayer()
    {
        Debug.Log("boss enter walk");
        m_NavmeshAgent?.SetDestination(m_Player.transform.position);
    }
    public void MoveToRandomPosition()
    {
        Vector3 randomPoint = GetRandomPointInVolume();
        NavMeshHit hit;

        // Kiểm tra vị trí có hợp lệ trên NavMesh hay không
        if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
        {
            m_NavmeshAgent.stoppingDistance = 0;
            m_NavmeshAgent.SetDestination(hit.position);
        }
    }
    public Vector3 GetRandomPointInVolume()
    {
        // Tạo điểm ngẫu nhiên trong vùng NavMesh Surface (Volume)
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = center.y; // Giữ nguyên độ cao
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(x, y, z);
    }
    public bool IsMoveTarget()
    {
        return (!m_NavmeshAgent.pathPending
         && m_NavmeshAgent.remainingDistance <= m_NavmeshAgent.stoppingDistance
        && m_NavmeshAgent.velocity.sqrMagnitude < 0.1f);
    }    
    public bool MoveTarget()
    {
        return (!m_NavmeshAgent.pathPending && m_NavmeshAgent.remainingDistance <= m_NavmeshAgent.stoppingDistance);
    }
    public Vector3 Distance()
    {
        return ( m_Player.transform.position - this.transform.position).normalized;
    }    
    public void ChangeStateCurrent(ENEMYSTATE state)
    {
        this.state = state;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_Range);
    }
}
