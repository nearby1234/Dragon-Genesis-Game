using System.Collections.Generic;
using UnityEngine;

public class SlashPool : MonoBehaviour
{
    public static SlashPool instance;
    [SerializeField] private Queue<GameObject> pool;
    [SerializeField] private int m_PoolCount;
    [SerializeField] private GameObject m_Slash;

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

        pool = new Queue<GameObject>();

        for (int i = 0; i < m_PoolCount; i++)
        {
            GameObject slash = Instantiate(m_Slash, this.transform);
            slash.SetActive(false);
            pool.Enqueue(slash);
        }
    }
    public GameObject GetSlash()
    {
        if (pool.Count > 0)
        {
            GameObject slash = pool.Dequeue();
            slash.SetActive(true);
            return slash;
        }
        return null;
    }
    public void ReturnPool(GameObject slash)
    {
        slash.SetActive(false);
        pool.Enqueue(slash);
        slash.transform.position = Vector3.zero;
        slash.transform.rotation = Quaternion.identity;
    }
}
