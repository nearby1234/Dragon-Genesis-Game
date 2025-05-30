using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    public BehaviorTreeSO behaviorTreeSO;
    public float m_ZoneTest;
    public float m_distance;
    public Transform m_Player;
    private void Update()
    {
        m_distance = Vector3.Distance(transform.position, m_Player.transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position,behaviorTreeSO.WanderRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, behaviorTreeSO.RangeAttackSword);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, behaviorTreeSO.RangeAttackThrow);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, m_ZoneTest);
    }
}
