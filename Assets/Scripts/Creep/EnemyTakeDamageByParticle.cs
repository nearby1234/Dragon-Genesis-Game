using System.Data.Common;
using UnityEngine;

public class EnemyTakeDamageByParticle : MonoBehaviour
{
    private int m_FireBallDamage;
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ground"))
        {
            if (DataManager.HasInstance)
            {
                EnemyData dragonFireData = DataManager.Instance.GetClonedData<EnemyData, EnemyType>(EnemyType.DragonFire);
                if (dragonFireData != null)
                {
                    m_FireBallDamage = dragonFireData.m_EffectDamage;
                    if (other.TryGetComponent<PlayerHeal>(out var playerHeal))
                    {
                        playerHeal.ReducePlayerHeal(m_FireBallDamage);
                    }
                }
            }
        }
    }

}
