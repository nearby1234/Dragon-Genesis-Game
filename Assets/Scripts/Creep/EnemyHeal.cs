using UnityEngine;

public class EnemyHeal : MonoBehaviour
{
    public int m_EnemyHeal { get; set; }
    [SerializeField] private EnemyController m_EnemyController;

    private void Awake()
    {
        m_EnemyController = GetComponent<EnemyController>();
    }

    public void ReducePlayerHealth(int damaged)
    {
        m_EnemyHeal -= damaged;
        if(m_EnemyHeal <= 0)
        {
            m_EnemyController.IsDead = true;
            m_EnemyController.GetAnimator().SetTrigger("Death");
        }
    }  
    //public void SetHeal(int heal)
    //{
    //    m_EnemyHeal = heal;
    //}

    //public int GetHeal() => m_EnemyHeal;


}
