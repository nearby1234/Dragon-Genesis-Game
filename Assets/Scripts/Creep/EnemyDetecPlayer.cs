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


    private void Awake()
    {
        m_Player = GameObject.Find("Player").GetComponent<Transform>();
        enemyController = GetComponent<EnemyController>();
    }
    public void CalculateDistance()
    {
        // Tính khoảng cách giữa enemy và player
        m_DistacneWithPlayer = Vector3.Distance(this.transform.position, m_Player.position);

        // Nếu player trong phạm vi phát hiện (maxDistance)
        if (m_DistacneWithPlayer < m_MaxDistance)
        {
            m_DetectedPlayer = true;
            enemyController.GetAnimator().SetBool("IsDetec", true);

            // Enemy bắt đầu theo đuổi player
            enemyController.GetNavMeshAgent().SetDestination(m_Player.position);
            enemyController.GetNavMeshAgent().stoppingDistance = m_DictanceStopped;

            // Kiểm tra trạng thái tấn công
            if (m_DistacneWithPlayer <= m_DictanceStopped) // Trong phạm vi tấn công
            {
                enemyController.GetNavMeshAgent().isStopped = true; // Dừng di chuyển
                enemyController.GetAnimator().SetBool("Attack", true); // Tấn công
            }
            else // Ngoài phạm vi tấn công nhưng trong maxDistance
            {
                enemyController.GetNavMeshAgent().isStopped = false; // Tiếp tục di chuyển
                enemyController.GetAnimator().SetBool("Attack", false); // Không tấn công
            }
        }
        else
        {
            // Nếu player vượt khỏi phạm vi phát hiện (maxDistance), reset trạng thái
            m_DetectedPlayer = false;
            enemyController.GetAnimator().SetBool("IsDetec", false);
            enemyController.GetAnimator().SetBool("Attack", false); // Tắt tấn công
            enemyController.GetNavMeshAgent().stoppingDistance = 0f; // Reset khoảng cách dừng
            enemyController.GetNavMeshAgent().isStopped = false; // Cho phép di chuyển tự do
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, m_MaxDistance);
    }
}
