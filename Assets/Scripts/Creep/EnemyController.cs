using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(EnemyHeal))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RandomNavMeshMove))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyHeal enemyHeal;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private RandomNavMeshMove randomNavMeshMovement;
    [SerializeField] private BaseEnemyDetecPlayer enemyDetecPlayer;
    [SerializeField] private Collider enemyCollider;
    [SerializeField] private EnemyStatSO enemyStatSO;
    [SerializeField] private BatPanel batPanel;
    [SerializeField] private BaseEnemyItem enemyItem;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        randomNavMeshMovement = GetComponent<RandomNavMeshMove>();
        enemyHeal = GetComponent<EnemyHeal>();
        enemyDetecPlayer = GetComponent<BaseEnemyDetecPlayer>();
        enemyCollider = GetComponentInChildren<Collider>();
        enemyItem = GetComponent<BaseEnemyItem>();

    }
    void Start()
    {
        randomNavMeshMovement.MoveToRandomPosition();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.CREEP_SEND_HEAL_VALUE, (this.gameObject, enemyStatSO));
        }

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
            batPanel.HideHeathlyBar();
            agent.isStopped = true;
            switch (enemyHeal.CreepType)
            {
                case CreepType.BAT:
                    animator.SetBool("Attack", false);
                    animator.SetBool("IsDetec", false);
                    break;
                case CreepType.DRAGON:
                    animator.SetBool("MeleeAttack", false);
                    animator.SetBool("RangeAttack", false);
                    animator.SetBool("IsDetec", false);
                    break;
                default:
                    Debug.LogWarning($"Không có enemy {enemyHeal.CreepType}");
                    break;
            }
            return;
        }
        randomNavMeshMovement.EnemyMoveTarget();
        enemyDetecPlayer.CalculateDistance();
    }

    public void SoundBat(string nameSound)
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE(nameSound);
        }
    }

    public Animator GetAnimator() => animator;
    public NavMeshAgent GetNavMeshAgent() => agent;
    public RandomNavMeshMove GetrandomNavMeshMovement() => randomNavMeshMovement;
    public EnemyHeal GetEnemyHeal() => enemyHeal;
    public BaseEnemyDetecPlayer GetEnemyDetecPlayer() => enemyDetecPlayer;
    public EnemyStatSO GetEnemyStatSO => enemyStatSO;
    public Collider GetCollider => enemyCollider;
    public BaseEnemyItem EnemyItem => enemyItem;


}
