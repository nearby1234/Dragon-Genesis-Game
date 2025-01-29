using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyPool : MonoBehaviour
{
    public static SpawnEnemyPool instance;
    [SerializeField] private Queue<GameObject> pool;
    [SerializeField] private int m_PoolSize = 10;
    [SerializeField] private GameObject m_EnemyPrehabs;
    [SerializeField] private int m_ActiveEnemiesCount = 5;  // Biến để điều chỉnh số lượng quái vật sử dụng

    private List<GameObject> activeEnemies = new List<GameObject>();  // Danh sách quái vật đang sử dụng

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SpawnEnemyPrehabs();
    }

    // Lấy quái vật từ pool nếu còn quái vật và số lượng active chưa đạt giới hạn
    public GameObject GetObject()
    {
        if (pool.Count > 0 && activeEnemies.Count < EnemyManager.instance.m_EnemySpawnCount)
        {
            GameObject enemy = pool.Dequeue();
            activeEnemies.Add(enemy);  // Thêm vào danh sách active
            enemy.SetActive(true);     // Kích hoạt quái vật
            return enemy;
        }
        return null;  // Nếu không còn enemy trong pool hoặc đạt số lượng active tối đa
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
            enemy.SetActive(false);  // Tắt quái vật khi mới tạo
            enemy.transform.SetPositionAndRotation(this.transform.position, Quaternion.identity);
            enemy.transform.parent = this.transform;
            pool.Enqueue(enemy);
        }
    }
}
