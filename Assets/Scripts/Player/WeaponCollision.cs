using Unity.Mathematics;
using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    //[SerializeField] private GameObject m_BloodPrehabs;
    [SerializeField] private GameObject m_HitPrehabs;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Creep"))
        {
            if (PlayerManager.instance.m_PlayerState.Equals(PlayerManager.PlayerState.idle)) return;
            EnemyHeal enemyHeal = other.GetComponentInParent<EnemyHeal>();
            if (enemyHeal != null)
            {
                enemyHeal.ReducePlayerHealth(PlayerManager.instance.playerDamage.GetPlayerDamage());
            }
            Vector3 hitPos = other.ClosestPoint(transform.position);
            //GameObject BloodFX = Instantiate(m_BloodPrehabs, hitPos, Quaternion.identity);
            GameObject HitFX = Instantiate(m_HitPrehabs, hitPos, Quaternion.identity);
        }

        if(other.gameObject.CompareTag("Boss"))
        {
            
            if (PlayerManager.instance.m_PlayerState.Equals(PlayerManager.PlayerState.idle)) return;
            WormBoss wormBoss = other.GetComponent<WormBoss>();
            if(wormBoss != null)
            {
                //Debug.Log("aaa");
                Debug.Log($"Collision {other.name}");
                wormBoss.GetDamage(PlayerManager.instance.playerDamage.GetPlayerDamage());
            }
            
        }
    }
}
  
