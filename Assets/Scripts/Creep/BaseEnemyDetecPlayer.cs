using UnityEngine;

public class BaseEnemyDetecPlayer : MonoBehaviour
{
    [SerializeField] protected EnemyController enemyController;
    [SerializeField] protected Transform m_Player;
    [SerializeField] protected bool m_DetectedPlayer;
    [SerializeField] protected float m_DistacneWithPlayer;
    [SerializeField] protected float m_DictanceStopped;
    [SerializeField] protected float m_MaxDistance;

    protected virtual void Awake()
    {
        m_Player = GameObject.Find("Player").GetComponent<Transform>();
        enemyController = GetComponent<EnemyController>();
    }

    // Ph??ng th?c tính kho?ng cách, ki?m tra phát hi?n và t?n công
    public virtual void CalculateDistance()
    {
        
    }

    public virtual void ResetAttackAnimation()
    {

    }    

    // V? gizmo ?? hi?n th? ph?m vi phát hi?n trong Editor
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_MaxDistance);
    }

}
