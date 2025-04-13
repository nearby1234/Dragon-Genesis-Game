using System.Collections;
using UnityEngine;

public class PlayerHeal : MonoBehaviour
{
    [SerializeField] private int m_PlayerCurrentHeal;
    [SerializeField] private int m_PlayerHealBase; // Giá trị hồi máu cơ bản
    [SerializeField] private int m_PlusHealValue; // Giá trị hồi máu cộng thêm
    [SerializeField] private int m_PlayerMaxHeal; // Giá trị hồi máu cộng thêm
    
    [SerializeField] private bool m_IsPlayerDeath;
    public bool m_IsDamaging = false;
    public int PlayerHealValue
    {
        get => m_PlayerCurrentHeal;
        set
        {
            m_PlayerCurrentHeal = value;
            UpdateHealUI(); // gửi UI khi set
        }
    }

    private void Start()
    {
        m_PlayerHealBase = PlayerManager.instance.PlayerStatSO.m_PlayerHeal;
        m_PlayerMaxHeal = m_PlayerHealBase;
        m_PlayerCurrentHeal = m_PlayerMaxHeal; // Khởi tạo player heal với giá trị cơ bản
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.SEND_HEAL_VALUE, m_PlayerMaxHeal);
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_HEAL_VALUE, m_PlayerCurrentHeal);
            ListenerManager.Instance.Register(ListenType.ITEM_USE_DATA_IS_HEAL, ReceriverValueItemUseHeal);
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_POINT, ReceiverPoint);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.ITEM_USE_DATA_IS_HEAL, ReceriverValueItemUseHeal);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_POINT, ReceiverPoint);
        }
    }
    public void ReducePlayerHeal(int Enemydamage)
    {
        if (m_IsPlayerDeath) return;
        m_PlayerCurrentHeal -= Enemydamage;

        m_IsDamaging = true;
        UpdateHealUI(); // gửi sự kiện cho UI cập nhật current heal

        if (m_PlayerCurrentHeal <= 0)
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
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_HEAL_VALUE, m_PlayerCurrentHeal);
        }
    }
    private void ReceriverValueItemUseHeal(object value)
    {
        if (value != null && value is float valuePercent)
        {
            m_PlayerCurrentHeal += (int)(m_PlayerCurrentHeal * valuePercent);
            // Không cho current heal vượt quá max heal
            m_PlayerCurrentHeal = Mathf.Min(m_PlayerCurrentHeal, m_PlayerMaxHeal);
            UpdateHealUI(); // gửi UI sau khi hồi máu
        }
    }
    private void ReceiverPoint(object value)
    {
        if (value is StatPointUpdateData data && data.StatName == "HealStatsTxt")
        {
            // Cập nhật max heal mới dựa trên điểm stat
            int newMax = m_PlayerHealBase + (data.Point * m_PlusHealValue);
            m_PlayerMaxHeal = newMax;

            // Lưu ý: Nếu người chơi đang có current heal ở mức thấp, bạn có thể giữ nguyên hoặc làm gì đó khác (ví dụ: không tự động hồi đầy)
            // Nếu bạn muốn current heal không thay đổi, hãy comment dòng sau:
            // m_PlayerCurrentHeal = newMax;

            // Broadcast max heal mới cho UI
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.SEND_HEAL_VALUE, m_PlayerMaxHeal);
            }
        }
    }
}


