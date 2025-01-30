using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private int m_EnemyDamage;
    [SerializeField] private GameObject m_BloodPrehabs;
    [SerializeField] private EnemyStatSO m_EnemyStatSO;

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
        if (other.gameObject.CompareTag("Sword"))
        {
            if (enemyController.GetEnemyHeal().IsEnemyDead()) return; // kiểm tra trạng thái của enemy
            if (PlayerManager.instance.m_PlayerState.Equals(PlayerManager.PlayerState.idle)) return;
            enemyController.GetEnemyHeal().ReducePlayerHealth(PlayerManager.instance.playerDamage.GetPlayerDamage());
        }
        if (other.CompareTag("Player"))
        {
            PlayerManager.instance.playerHeal.ReducePlayerHeal(m_EnemyDamage);
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            GameObject BloodFX = Instantiate(m_BloodPrehabs, hitPoint, Quaternion.identity);
        }
    }
}
