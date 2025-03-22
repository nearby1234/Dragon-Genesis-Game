using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    //[SerializeField] private GameObject m_BloodPrehabs;
    [SerializeField] private GameObject m_HitPrehabs;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Creep"))
        {
            if (PlayerManager.instance.m_PlayerState.Equals(PlayerManager.PlayerState.idle)) return;
            if (other.TryGetComponent<EnemyHeal>(out var enemyHeal))
                if (enemyHeal != null)
                {
                    enemyHeal.ReducePlayerHealth(PlayerManager.instance.playerDamage.GetPlayerDamage());
                }
            SpawnHitPrehabs(other);
        }

        if (other.gameObject.CompareTag("Boss"))
        {
            if (PlayerManager.instance.m_PlayerState.Equals(PlayerManager.PlayerState.idle)) return;
            if (other.TryGetComponent<WormBoss>(out var wormBoss))
                if (wormBoss != null)
                {
                    //Debug.Log($"Collision {other.name}");
                    wormBoss.GetDamage(PlayerManager.instance.playerDamage.GetPlayerDamage());
                }
            SpawnHitPrehabs(other);
        }
    }
    private void SpawnHitPrehabs(Collider other)
    {
        Vector3 hitPos = other.ClosestPoint(transform.position);
        GameObject HitFX = Instantiate(m_HitPrehabs, hitPos, Quaternion.identity);
        HitFX.SetActive(true);
        Destroy(HitFX, 1f);

    }
   
}

