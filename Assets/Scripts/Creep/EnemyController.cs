using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(RandomNavMeshMovement))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public EnemyHeal enemyHeal;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private RandomNavMeshMovement randomNavMeshMovement;
    [SerializeField] private EnemyDetecPlayer enemyDetecPlayer;
    [SerializeField] private EnemyStatSO enemyStatSO;
    public bool IsDead;
    private int m_EnemyHeal;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        randomNavMeshMovement = GetComponent<RandomNavMeshMovement>();
        enemyHeal = GetComponent<EnemyHeal>();
        enemyDetecPlayer = GetComponent<EnemyDetecPlayer>();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        randomNavMeshMovement.MoveToRandomPosition();
        m_EnemyHeal = enemyStatSO.heal;
        enemyHeal.m_EnemyHeal = this.m_EnemyHeal;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHeal.m_EnemyHeal <= 0)
        {
            agent.isStopped = true;
            IsDead = true;
            return;
        }else
        {
            IsDead = false;
        }    
        randomNavMeshMovement.EnemyMoveTarget();
        enemyDetecPlayer.CalculateDistance();
    }

    public Animator GetAnimator() => animator;
    public NavMeshAgent GetNavMeshAgent() => agent;
    public RandomNavMeshMovement GetrandomNavMeshMovement() => randomNavMeshMovement;
    public EnemyHeal GetEnemyHeal() => enemyHeal;
    public EnemyDetecPlayer GetEnemyDetecPlayer() => enemyDetecPlayer;
    public int GetHeal() => m_EnemyHeal;


}
