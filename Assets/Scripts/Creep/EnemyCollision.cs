using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword"))
        {
            if (enemyController.GetEnemyHeal().IsEnemyDead()) return; // kiểm tra trạng thái của enemy
            if (PlayerManager.instance.m_PlayerState.Equals(PlayerManager.PlayerState.idle)) return;
            Debug.Log("damage");
            enemyController.GetEnemyHeal().ReducePlayerHealth(1);
        }
    }
}
