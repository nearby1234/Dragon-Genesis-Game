using UnityEngine;

public class EnemyHeal : MonoBehaviour
{
    [SerializeField] private int m_EnemyHeal;
    [SerializeField] private EnemyController m_EnemyController;
    [SerializeField] private bool m_IsDead;

    private void Awake()
    {
        m_EnemyController = GetComponent<EnemyController>();
        m_EnemyHeal = m_EnemyController.GetEnemyStatSO().heal;
    }

    public void ReducePlayerHealth(int damaged)
    {
        if (m_IsDead) return;
        m_EnemyHeal -= damaged;
        if (m_EnemyHeal > 0)
        {
            m_EnemyController.GetAnimator().SetTrigger("Hit");
            return;
        }
        Die();
    }
    private void Die()
    {
        m_IsDead = true;
        m_EnemyController.GetAnimator().SetTrigger("Death");
        m_EnemyController.GetCollider.enabled = false;
    }
    public bool IsEnemyDead() => m_IsDead;
}
