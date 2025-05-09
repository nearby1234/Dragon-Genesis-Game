using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormTakeDamage : MonoBehaviour
{
    [SerializeField] private WormBoss m_WormBoss;
    [SerializeField] private float m_DamageCurrent;
    [SerializeField] private bool m_IsPlayerReceiverDamage;
    [SerializeField] private float m_TimerResetPlayerReceiverDamage;

    private void Awake()
    {
        m_WormBoss = GetComponentInParent<WormBoss>();
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.BOSS_SENDER_DAMAGED, ReceiverBossDamage);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.BOSS_SENDER_DAMAGED, ReceiverBossDamage);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerHeal>(out var playerHeal))
            {
                if (!m_IsPlayerReceiverDamage && m_WormBoss.CurrenState.Equals(WORMSTATE.ATTACK))
                {
                    playerHeal.ReducePlayerHeal((int)m_DamageCurrent);
                    m_IsPlayerReceiverDamage = true;
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    if (EffectManager.HasInstance)
                    {
                        GameObject bloodPrehaps = EffectManager.Instance.GetPrefabs("BloodFx");
                        GameObject particle = Instantiate(bloodPrehaps, hitPoint, Quaternion.identity);
                        Destroy(particle, 2f);
                    }
                    if (AudioManager.HasInstance)
                    {
                        AudioManager.Instance.PlaySE("PlayerHit");
                    }
                    StartCoroutine(ResetIsPlayerReceiverDamage());
                }
            }
        }
    }

    private void ReceiverBossDamage(object value)
    {
        if (value != null)
        {
            if (m_WormBoss != null)

            {
                if (value is (int currentAttackIndex, List<WormAttackData> attackDataList))
                {
                    m_DamageCurrent = attackDataList[currentAttackIndex].CalculateDamage(m_WormBoss);
                }
            }

        }
    }
    private IEnumerator ResetIsPlayerReceiverDamage()
    {
        yield return new WaitForSeconds(m_TimerResetPlayerReceiverDamage);
        m_IsPlayerReceiverDamage = false;
    }

}
