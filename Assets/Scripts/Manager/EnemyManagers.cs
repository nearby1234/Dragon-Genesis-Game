using UnityEngine;

public class EnemyManagers : BaseManager<EnemyManagers>
{
    [SerializeField] private Transform m_ParentSpawnEnemyPool;
    [SerializeField] private SpawnEnemyPool[] spawnEnemyPool;
    
    private void Start()
    {
        GetChildSpawnPool();
    }
    private void Update()
    {
        CallObjectSpawnEnemy();
    }

    private void GetChildSpawnPool()
    {
        if (m_ParentSpawnEnemyPool == null)
        {
            Debug.LogError("Zone Parent chưa được gán trong EnemyManagers!");
            return;
        }
        spawnEnemyPool = m_ParentSpawnEnemyPool.GetComponentsInChildren<SpawnEnemyPool>();
    }

    private void CallObjectSpawnEnemy()
    {
        if (spawnEnemyPool == null || spawnEnemyPool.Length == 0)
        {
            Debug.LogWarning("spawnEnemyPool chưa được gán hoặc không có đối tượng!");
            return;
        }

        foreach (var pool in spawnEnemyPool)
        {
            for (int i = 0; i < pool.GetActiveEnemiesCount(); i++) // Spawn theo số lượng riêng
            {
                GameObject enemy = pool.GetObject();

                if (enemy != null)
                {
                    enemy.transform.SetPositionAndRotation(pool.transform.position, Quaternion.identity);
                }
            }
        }
    }
}
