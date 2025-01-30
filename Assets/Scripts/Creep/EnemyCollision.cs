using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private int m_EnemyDamage;
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
            Debug.Log("damage");
            enemyController.GetEnemyHeal().ReducePlayerHealth(PlayerManager.instance.playerDamage.GetPlayerDamage());
        }
        if (other.CompareTag("Player"))
        {
            PlayerManager.instance.playerHeal.ReducePlayerHeal(m_EnemyDamage);
        }
    }
}
