using System.Collections.Generic;
using UnityEngine;

public class SkillPool : MonoBehaviour
{
    [SerializeField] private Queue<GameObject> pool = new();
    [SerializeField] private int m_PoolCount = 3;
    [SerializeField] private GameObject m_FireSkill;

    private void Awake()
    {
        for(int i = 0; i < m_PoolCount; ++i)
        {
            GameObject fireSkill = Instantiate(m_FireSkill,this.transform);
            fireSkill.SetActive(false);
            pool.Enqueue(fireSkill);
        }    
    }
   public GameObject FireSkill()
    {
        if(pool.Count > 0)
        {
            GameObject fireSkill= pool.Dequeue();
            fireSkill.SetActive(true);
            return fireSkill;
        }    
        return null;
    }    

    public void ReturnFireSkillPool(GameObject FireFX)
    {
        FireFX.SetActive(false);
        pool.Enqueue(FireFX);
        FireFX.transform.position = Vector3.zero;
        FireFX.transform.rotation = Quaternion.identity;
    }    
}
