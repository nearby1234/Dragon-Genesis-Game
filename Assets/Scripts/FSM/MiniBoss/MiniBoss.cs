using UnityEngine;
using UnityEngine.AI;

public class MiniBoss : MonoBehaviour
{
    public enum ENEMYSTATE
    {
        DEFAULT =0,
        IDLE,
        WALK,
        ATTACK,
    }
    private FSM fSM;
    public ENEMYSTATE state;
    [SerializeField] private float m_Range;
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_Distacne;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject m_Player;
    [SerializeField] private NavMeshAgent m_NavmeshAgent;

    public Animator Animator => animator;
    public NavMeshAgent NavmeshAgent => m_NavmeshAgent;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        m_NavmeshAgent = GetComponent<NavMeshAgent>();
        m_Player = GameObject.Find("Player");
    }
    void Start()
    {
        fSM = new FSM();
        fSM.ChangeState(new IdleState(this,fSM));
    }

    // Update is called once per frame
    void Update()
    {
        fSM.Update();
    }

    public bool PlayerInRange()
    {
        return (Vector3.Distance(this.transform.position, m_Player.transform.position) <= m_Range);
    }

    public void TestButoon()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            fSM.ChangeState(new WalkState(this, fSM));
        }
    }    

    public void MoveToPlayer()
    {
        Debug.Log("boss enter walk");
        //transform.position = Vector3.MoveTowards(transform.position,m_Player.transform.position, m_Speed * Time.deltaTime);
        m_NavmeshAgent.SetDestination(m_Player.transform.position);
        

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,m_Range);
    }
}
