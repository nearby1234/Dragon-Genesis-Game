using System.Collections;
using UnityEngine;

public class PlayerHeal : MonoBehaviour
{
    [SerializeField] private int m_PlayerHeal;
    [SerializeField] private bool m_IsPlayerDeath;
    public bool m_IsDamaging = false;
    public int PlayerHealValue
    {
        get => m_PlayerHeal;
        set
        {
            m_PlayerHeal = value;
            UpdateHealUI(); // gửi UI khi set
        }
    }

    private void Start()
    {
        m_PlayerHeal = PlayerManager.instance.PlayerStatSO.m_PlayerHeal;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.SEND_HEAL_VALUE, m_PlayerHeal);
            ListenerManager.Instance.Register(ListenType.ITEM_USE_DATA_IS_HEAL, ReceriverValueItemUseHeal);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.ITEM_USE_DATA_IS_HEAL, ReceriverValueItemUseHeal);
        }
    }
    public void ReducePlayerHeal(int Enemydamage)
    {
        if (m_IsPlayerDeath) return;
        m_PlayerHeal -= Enemydamage;
       
        m_IsDamaging = true;
        UpdateHealUI(); // gửi sự kiện cho UI
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
    private void UpdateHealUI()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_HEAL_VALUE, m_PlayerHeal);
        }
    }
    private void ReceriverValueItemUseHeal(object value)
    {
        if (value != null && value is float valuePercent)
        {
            m_PlayerHeal += (int)(m_PlayerHeal * valuePercent);
            int maxHeal = PlayerManager.instance.PlayerStatSO.m_PlayerHeal;
            m_PlayerHeal = Mathf.Min(m_PlayerHeal, maxHeal);

            UpdateHealUI(); // gửi UI sau khi hồi máu
        }
    }
}


