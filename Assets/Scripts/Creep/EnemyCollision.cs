using UnityEngine;

[RequireComponent (typeof(CapsuleCollider))]
public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private int m_EnemyDamage;
    [SerializeField] private GameObject m_BloodPrehabs;
    [SerializeField] private EnemyStatSO m_EnemyStatSO;
    [SerializeField] private float lastHitTime;
    [SerializeField] private float hitCooldown = 0.1f;
    private void Start()
    {
        m_EnemyDamage = m_EnemyStatSO.damage;
    }
    [SerializeField] private EnemyController enemyController;
    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerManager.instance.playerHeal.m_IsDamaging == false)
            {
                PlayerManager.instance.playerHeal.ReducePlayerHeal(m_EnemyDamage);
            }

            if (Time.time - lastHitTime > hitCooldown)
            {
                lastHitTime = Time.time;

                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Instantiate(m_BloodPrehabs, hitPoint, Quaternion.identity);
            }
        }
    }
}
