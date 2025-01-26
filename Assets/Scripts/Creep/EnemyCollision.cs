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
            if (!PlayerManager.instance.m_PlayerState.Equals(PlayerManager.PlayerState.idle))
            {
                if (enemyController.IsDead) // Ki?m tra tr?ng thái c?a enemy tr??c
                {
                    return;
                }
                enemyController.GetEnemyHeal().ReducePlayerHealth(1);
                enemyController.GetAnimator().SetTrigger("Hit");
            }
            else
            {
                Debug.Log($"Is Idle not collision");
            }

        }
    }
}
