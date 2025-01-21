using UnityEngine;
using UnityEngine.AI;

public class EnemyDetecPlayer : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Transform m_Player;
    [SerializeField] private float m_DistacneWithPlayer;
    [SerializeField] private float m_MaxDistance;
    [SerializeField] private float m_DictanceStopped;
    [SerializeField] private bool m_DetectedPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        m_Player = GameObject.Find("Player").GetComponent<Transform>();
        enemyController = GetComponent<EnemyController>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CalculateDistance();
    }
    private void CalculateDistance()
    {
        m_DistacneWithPlayer = Vector3.Distance(this.transform.position, m_Player.position);
        if (m_DistacneWithPlayer < m_MaxDistance)
        {
            m_DetectedPlayer = true;
            enemyController.GetAnimator().SetBool("IsDetec", true);
            enemyController.GetNavMeshAgent().SetDestination(m_Player.position);
            enemyController.GetNavMeshAgent().stoppingDistance = m_DictanceStopped;
            if (enemyController.GetNavMeshAgent().remainingDistance <= enemyController.GetNavMeshAgent().stoppingDistance
                 && !enemyController.GetNavMeshAgent().pathPending)
            {
                enemyController.GetAnimator().SetBool("Attack", true);
            }
            else
            {
                enemyController.GetAnimator().SetBool("Attack", false);
            }
        }
        else
        {
            m_DetectedPlayer = false;
            enemyController.GetAnimator().SetBool("IsDetec", false);
            enemyController.GetNavMeshAgent().stoppingDistance = 0f;
            //enemyController.GetrandomNavMeshMovement().EnemyMoveTarget();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, m_MaxDistance);
    }
}
