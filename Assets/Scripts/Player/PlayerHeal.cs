using UnityEngine;

public class PlayerHeal : MonoBehaviour
{
    [SerializeField] private int m_PlayerHeal;
    [SerializeField] private bool m_IsPlayerDeath;
   
    public void ReducePlayerHeal(int Enemydamage)
    {
        m_PlayerHeal -= Enemydamage;
        if(m_PlayerHeal <= 0)
        {
            m_IsPlayerDeath = true;
            PlayerManager.instance.playerAnim.GetAnimator().SetTrigger("Death");
            
        }
    }
}
