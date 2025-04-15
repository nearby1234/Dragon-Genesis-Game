using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [SerializeField] private float m_MaxMana; // Maximum mana
    [SerializeField] private float m_CurrentMana; // Current mana
    [SerializeField] private float m_ManaBase; // Base mana
    [SerializeField] private float m_PlusHealValue; // Additional mana value

    private void Start()
    {
        m_ManaBase = PlayerManager.instance.PlayerStatSO.m_PlayerMana;
        m_MaxMana = m_ManaBase;
        m_CurrentMana = m_MaxMana; // Initialize current mana to max mana

        // Các broadcast và đăng ký khác
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_MANA_VALUE, m_MaxMana);
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_MANA_VALUE, m_CurrentMana);
            ListenerManager.Instance.Register(ListenType.PLAYER_SKILL_CONSUMPTION_MANA, ReceiverPlayerSkillConsumptionMana);
            ListenerManager.Instance.Register(ListenType.ITEM_USE_DATA_IS_MANA, RegenMana);

            // Đăng ký nhận sự kiện kiểm tra mana cho skill
            ListenerManager.Instance.Register(ListenType.PLAYER_SKILL_KEYDOWN, OnSkillKeyDown);
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_POINT, ReceiverPoint);
        }
    }

    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SKILL_CONSUMPTION_MANA, ReceiverPlayerSkillConsumptionMana);
            ListenerManager.Instance.Unregister(ListenType.ITEM_USE_DATA_IS_MANA, RegenMana);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SKILL_KEYDOWN, OnSkillKeyDown);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_POINT, ReceiverPoint);
        }
    }


    private void UpdateMana(float manaValue)
    {
        m_CurrentMana -= manaValue;
        m_CurrentMana = Mathf.Max(m_CurrentMana, 0); // Ensure current mana does not go below zero
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_MANA_VALUE, m_CurrentMana);
        }
        CalcuManaNextTo(manaValue);
    }
    private void RegenMana(object percentManaRegen)
    {
        if(percentManaRegen is float percentManaRegenValue)
        {
            m_CurrentMana += m_MaxMana * percentManaRegenValue;
            m_CurrentMana = Mathf.Min(m_CurrentMana, m_MaxMana); // Ensure current mana does not exceed max mana
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_MANA_VALUE, m_CurrentMana);
            }
        }
        
    }
    private void ReceiverPlayerSkillConsumptionMana(object data)
    {
        if (data is float manaValue)
        {
            UpdateMana(manaValue);
        }
    }

    private void CalcuManaNextTo(float manaValue)
    {
        bool IsManaEmpty = m_CurrentMana < manaValue;
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_MANA_EMPTY, IsManaEmpty);
        }
    }
    private void OnSkillKeyDown(object data)
    {
        // Định nghĩa một ngưỡng tiêu hao cho skill, ví dụ 20% của max mana
        float requiredMana = m_MaxMana * 0.2f; // Bạn có thể thay đổi theo ý bạn

        // Nếu current mana nhỏ hơn requiredMana thì báo mana không đủ
        bool isManaNotEnough = m_CurrentMana < requiredMana;

        // Broadcast kết quả cho các lớp khác (như PivotScaleWeapon) qua sự kiện PLAYER_MANA_EMPTY
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_MANA_EMPTY, isManaNotEnough);
        }
    }
    private void ReceiverPoint(object value)
    {
        if (value is StatPointUpdateData data && data.StatName == "IntelligentStatsTxt")
        {
            // Cập nhật max heal mới dựa trên điểm stat
            int newMax =(int) m_ManaBase + (data.Point *(int) m_PlusHealValue);
            m_MaxMana = newMax;

            
            // Cập nhật max mana mới tới UI
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_MANA_VALUE, m_MaxMana);
            }
        }
    }

}
