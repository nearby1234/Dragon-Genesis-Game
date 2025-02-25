using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(EnemyHeal))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RandomNavMeshMovement))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyHeal enemyHeal;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private RandomNavMeshMovement randomNavMeshMovement;
    [SerializeField] private BaseEnemyDetecPlayer enemyDetecPlayer;
    [SerializeField] private Collider enemyCollider;
    [SerializeField] private EnemyStatSO enemyStatSO;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        randomNavMeshMovement = GetComponent<RandomNavMeshMovement>();
        enemyHeal = GetComponent<EnemyHeal>();
        enemyDetecPlayer = GetComponent<BaseEnemyDetecPlayer>();
        enemyCollider = GetComponentInChildren<Collider>();
    }
    void Start()
    {
        randomNavMeshMovement.MoveToRandomPosition();
    }
    void Update()
    {
        if (PlayerManager.instance.playerHeal.GetPlayerDeath())
        {
            agent.isStopped = true;
            enemyDetecPlayer.ResetAttackAnimation();
            return;
        }
        if (enemyHeal.IsEnemyDead())
        {
            agent.isStopped = true;
            return;
        }
        randomNavMeshMovement.EnemyMoveTarget();
        enemyDetecPlayer.CalculateDistance();
    }
    public Animator GetAnimator() => animator;
    public NavMeshAgent GetNavMeshAgent() => agent;
    public RandomNavMeshMovement GetrandomNavMeshMovement() => randomNavMeshMovement;
    public EnemyHeal GetEnemyHeal() => enemyHeal;
    public BaseEnemyDetecPlayer GetEnemyDetecPlayer() => enemyDetecPlayer;
    public EnemyStatSO GetEnemyStatSO() => enemyStatSO;
    public Collider GetCollider => enemyCollider;
    private void ReturnWhenPlayerDeath()
    {

    }    
}
