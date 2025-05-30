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
                EnemyStatSO dragonFireData = DataManager.Instance.GetClonedData<EnemyStatSO, EnemyType>(EnemyType.DragonFire);
                if (dragonFireData != null)
                {
                    m_FireBallDamage = dragonFireData.m_EffectDamage;
                    if (other.TryGetComponent<PlayerHeal>(out var playerHeal))
                    {
                        playerHeal.ReducePlayerHeal(m_FireBallDamage, TypeCollider.UnKnown);
                        if (AudioManager.HasInstance)
                        {
                            AudioManager.Instance.PlaySE("FireExplosion");
                            AudioManager.Instance.PlaySE("PlayerHit");
                        }
                        Debug.Log($"m_FireBallDamage : {m_FireBallDamage}");
                    }
                }
            }
        }
    }

}
