using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyPool : MonoBehaviour
{
    [SerializeField] private Queue<GameObject> pool;
    [SerializeField] private int m_PoolSize = 10;
    [SerializeField] private GameObject m_EnemyPrehabs;
    [SerializeField] private int m_ActiveEnemiesCount = 5;  // Biến để điều chỉnh số lượng quái vật sử dụng

    private List<GameObject> activeEnemies = new List<GameObject>();  // Danh sách quái vật đang sử dụng

    private void Awake()
    {
        SpawnEnemyPrehabs();
    }

    // Lấy quái vật từ pool nếu còn quái vật và số lượng active chưa đạt giới hạn
    public GameObject GetObject()
    {
        if (pool.Count > 0 && activeEnemies.Count < m_ActiveEnemiesCount) // Giới hạn theo từng pool
        {
            GameObject enemy = pool.Dequeue();
            activeEnemies.Add(enemy);
            enemy.SetActive(true);
            return enemy;
        }
        return null;
    }

    // Trả lại quái vật vào pool
    public void ReturnObject(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            enemy.SetActive(false);  // Tắt game object khi trả lại pool
            pool.Enqueue(enemy);     // Trả lại vào pool
        }
    }

    private void SpawnEnemyPrehabs()
    {
        pool = new Queue<GameObject>();
        for (int i = 0; i < m_PoolSize; i++)
        {
            GameObject enemy = Instantiate(m_EnemyPrehabs);
            enemy.gameObject.name = $"{m_EnemyPrehabs.name} {i}";
            enemy.SetActive(false);  // Tắt quái vật khi mới tạo
            enemy.transform.SetPositionAndRotation(this.transform.position, Quaternion.identity);
            enemy.transform.parent = this.transform;
            pool.Enqueue(enemy);
        }
    }
    public int GetActiveEnemiesCount() => m_ActiveEnemiesCount;
}
