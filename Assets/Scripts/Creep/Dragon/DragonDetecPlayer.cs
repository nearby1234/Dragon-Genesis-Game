using UnityEngine;
using UnityEngine.AI;

public class DragonDetecPlayer : BaseEnemyDetecPlayer
{
    private bool hasPlayedIdle = false;
    [SerializeField] private float meleeRange = 5f;    // Tầm tấn công cận chiến
    protected override void Awake()
    {
        base.Awake();
    }
    public override void CalculateDistance()
    {
        // Tính khoảng cách giữa enemy (dragon) và player
        m_DistacneWithPlayer = Vector3.Distance(transform.position, m_Player.position);

        // Nếu player nằm trong tầm tấn công fireball hoặc melee (fireballRange)
        if (m_DistacneWithPlayer < m_MaxDistance)
        {
            if(!hasPlayedIdle)
            {
                if (AudioManager.HasInstance)
                {
                    AudioManager.Instance.PlaySE("DragonIdle");
                }
                hasPlayedIdle = true;
            }
            m_DetectedPlayer = true;
            //    // Chuyển qua trạng thái battle (idle battle) trước khi tấn công
            //    enemyController.GetAnimator().SetBool("IsDetec", true);
            //    // Liên tục cập nhật vị trí của player
            enemyController.GetNavMeshAgent().stoppingDistance = 0f;
            enemyController.GetNavMeshAgent().SetDestination(m_Player.position);
            enemyController.GetAnimator().SetBool("RangeAttack", true);

            //    // Nếu player quá gần (trong meleeRange), thì tiến hành melee attack (attack1)
            if (m_DistacneWithPlayer <= meleeRange)
            {
                m_DetectedPlayer = true;
                enemyController.GetNavMeshAgent().SetDestination(m_Player.position);
                enemyController.GetNavMeshAgent().stoppingDistance = m_DictanceStopped;
                if (m_DistacneWithPlayer <= m_DictanceStopped) // Trong ph?m vi t?n công
                {
                    enemyController.GetNavMeshAgent().isStopped = true; // D?ng di chuy?n
                    enemyController.GetAnimator().SetBool("MeleeAttack", true); // T?n công
                    enemyController.GetAnimator().SetBool("RangeAttack", false); // T?n công
                }
                else // Ngoài ph?m vi t?n công nh?ng trong maxDistance
                {
                    enemyController.GetNavMeshAgent().isStopped = false; // Ti?p t?c di chuy?n
                    enemyController.GetAnimator().SetBool("MeleeAttack", false); // Không t?n công
                }
            }
        }
        else
        {
            m_DetectedPlayer = false;
            hasPlayedIdle = false;
            enemyController.GetAnimator().SetBool("IsDetec", false);
            //enemyController.GetAnimator().SetBool("RangeAttack", false); // T?t t?n công
            enemyController.GetAnimator().SetBool("RangeAttack", false);
            enemyController.GetNavMeshAgent().stoppingDistance = 0f; // Reset kho?ng cách d?ng
            enemyController.GetNavMeshAgent().isStopped = false; // Cho phép di chuy?n t? do
        }
    }
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        // Vẽ thêm 1 sphere cho tầm melee (tuỳ chọn)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
    public override void ResetAttackAnimation()
    {
        enemyController.GetAnimator().SetBool("MeleeAttack", false);
        enemyController.GetAnimator().SetBool("Fireball", false);
    }
}
