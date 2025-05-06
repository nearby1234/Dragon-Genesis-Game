using UnityEngine;

public class BatDetecPlayer : BaseEnemyDetecPlayer
{
    private bool hasPlayedIdle = false;
    protected override void Awake()
    {
        base.Awake();
    }
    public override void CalculateDistance()
    {
        // Tính kho?ng cách gi?a enemy và player
        m_DistacneWithPlayer = Vector3.Distance(this.transform.position, m_Player.position);

        // N?u player trong ph?m vi phát hi?n (maxDistance)
        if (m_DistacneWithPlayer < m_MaxDistance)
        {
            m_DetectedPlayer = true;
            enemyController.GetAnimator().SetBool("IsDetec", true);

            // Enemy b?t ??u theo ?u?i player
            enemyController.GetNavMeshAgent().SetDestination(m_Player.position);
            enemyController.GetNavMeshAgent().stoppingDistance = m_DictanceStopped;
          
            if (!hasPlayedIdle)
            {
               
                if (AudioManager.HasInstance)
                {
                    Debug.Log($"hasPlayedIdle {hasPlayedIdle}");
                    AudioManager.Instance.PlaySE("BatIdle");
                }

                hasPlayedIdle = true;
            }
           
            // Ki?m tra tr?ng thái t?n công
            if (m_DistacneWithPlayer <= m_DictanceStopped) // Trong ph?m vi t?n công
            {
                enemyController.GetNavMeshAgent().isStopped = true; // D?ng di chuy?n
                enemyController.GetAnimator().SetBool("Attack", true); // T?n công
            }
            else // Ngoài ph?m vi t?n công nh?ng trong maxDistance
            {
                enemyController.GetNavMeshAgent().isStopped = false; // Ti?p t?c di chuy?n
                enemyController.GetAnimator().SetBool("Attack", false); // Không t?n công
            }
        }
        else
        {
            // N?u player v??t kh?i ph?m vi phát hi?n (maxDistance), reset tr?ng thái
            m_DetectedPlayer = false;
            hasPlayedIdle = false;
            enemyController.GetAnimator().SetBool("IsDetec", false);
            enemyController.GetAnimator().SetBool("Attack", false); // T?t t?n công
            enemyController.GetNavMeshAgent().stoppingDistance = 0f; // Reset kho?ng cách d?ng
            enemyController.GetNavMeshAgent().isStopped = false; // Cho phép di chuy?n t? do
           
        }
    }
    public override void ResetAttackAnimation()
    {
        enemyController.GetAnimator().SetBool("Attack", false);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
    }
}
