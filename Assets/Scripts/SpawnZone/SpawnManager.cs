using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public int m_Count;
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

       
    }
    void Start()
    {
       
        
    }
    private void Update()
    {
        // Gọi GetObject để spawn một quái vật tại spawnPoint

        for (int i = 0; i < m_Count; i++)
        {
            GameObject enemy = SpawnEnemyPool.instance.GetObject();

            if (enemy != null)
            {
                // Nếu thành công, đặt quái vật vào vị trí spawn
                enemy.transform.SetPositionAndRotation(transform.position, transform.rotation);

                // Bạn có thể thêm các hành vi khác như cho quái vật di chuyển, chiến đấu, v.v.
            }
        }
    }
}
