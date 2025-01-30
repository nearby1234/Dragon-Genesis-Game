using UnityEngine;

public class PlayerHeal : MonoBehaviour
{
    [SerializeField] private int m_PlayerHeal;
    [SerializeField] private bool m_IsPlayerDeath;
   
    public void ReducePlayerHeal(int Enemydamage)
    {
        if (m_IsPlayerDeath) return;
        m_PlayerHeal -= Enemydamage;
        if(m_PlayerHeal <= 0)
        {
            m_IsPlayerDeath = true;
            PlayerManager.instance.playerAnim.GetAnimator().SetTrigger("Death");
        }
    }

    public bool GetPlayerDeath() => m_IsPlayerDeath;
}
