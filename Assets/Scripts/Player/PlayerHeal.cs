using System.Collections;
using UnityEngine;

public class PlayerHeal : MonoBehaviour
{
    [SerializeField] private int m_PlayerHeal;
    [SerializeField] private bool m_IsPlayerDeath;
    public bool m_IsDamaging = false;


    private void Start()
    {
        m_PlayerHeal = PlayerManager.instance.PlayerStatSO.m_PlayerHeal;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.SEND_HEAL_VALUE, m_PlayerHeal);

        }
    }
   
    public void ReducePlayerHeal(int Enemydamage)
    {
        if (m_IsPlayerDeath) return;
        m_PlayerHeal -= Enemydamage;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_HEAL_VALUE, m_PlayerHeal);

        }
        m_IsDamaging = true;
        if (m_PlayerHeal <= 0)
        {
            m_IsPlayerDeath = true;
            PlayerManager.instance.playerAnim.GetAnimator().Play("Death");
        }
        else
        {
            StartCoroutine(ResetDamaging());
        }    
    }

    public bool GetPlayerDeath() => m_IsPlayerDeath;
    public IEnumerator ResetDamaging()
    {
        yield return new WaitForSeconds(0.8f);
        m_IsDamaging = false;
    }   
    
    //private void ReceiverHealValue(object value)
    //{
    //    if(value != null)
    //    {
    //        if(value is PlayerStatSO playerStatSO)
    //        {
    //            m_PlayerHeal = playerStatSO.m_PlayerHeal;
    //        }
    //    }
    //}
}
