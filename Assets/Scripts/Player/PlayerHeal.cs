using System.Collections;
using UnityEngine;

public class PlayerHeal : MonoBehaviour
{
    [SerializeField] private int m_PlayerCurrentHeal;
    [SerializeField] private int m_PlayerHealBase; // Giá trị hồi máu cơ bản
    [SerializeField] private int m_PlusHealValue; // Giá trị hồi máu cộng thêm
    [SerializeField] private int m_PlayerMaxHeal; // Máu tối đa của player
    [SerializeField] private bool m_IsPlayerDeath;
    public bool m_IsDamaging = false;
    public int PlayerHealValue
    {
        get => m_PlayerCurrentHeal;
        set
        {
            if (m_PlayerCurrentHeal == value) return;
            m_PlayerCurrentHeal = value;
            UpdateHealUI(); // gửi UI khi set
        }
    }
    public int PlusHealValue => m_PlusHealValue;

    private readonly int hashNameBigHit = Animator.StringToHash("Damage_Front_Big_ver_B");
    private readonly int hashNameHit = Animator.StringToHash("Damage_Front_Small_ver_A");


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
            ListenerManager.Instance.Register(ListenType.CHEAT_PLAYER_HEAL, OnEventCheatPlayerHeal);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.ITEM_USE_DATA_IS_HEAL, ReceriverValueItemUseHeal);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_POINT, ReceiverPoint);
            ListenerManager.Instance.Unregister(ListenType.CHEAT_PLAYER_HEAL, OnEventCheatPlayerHeal);
        }
    }
    public void ReducePlayerHeal(int Enemydamage, TypeCollider typeCollider)
    {
        if (m_IsPlayerDeath) return;
        m_PlayerCurrentHeal -= Enemydamage;

        m_IsDamaging = true;
        UpdateHealUI(); // gửi sự kiện cho UI cập nhật current heal

        if (m_PlayerCurrentHeal <= 0)
        {
            m_PlayerCurrentHeal = 0; // Đảm bảo current heal không âm
            m_IsPlayerDeath = true;
            if (ListenerManager.HasInstance)
            {
                Debug.Log($"m_IsPlayerDeath :{m_IsPlayerDeath}");
                ListenerManager.Instance.BroadCast(ListenType.PLAYER_DIE, m_IsPlayerDeath);
            }
            PlayerManager.instance.playerAnim.GetAnimator().Play("Death");
        }
        else
        {
            switch (typeCollider)
            {
                case TypeCollider.Leg:
                    PlayAnimationBigHit();
                    break;
                case TypeCollider.Axe:
                    PlayAnimationHit();
                    break;
                case TypeCollider.ThrowAxe:
                    PlayAnimationHit();
                    break;
                case TypeCollider.ThrowAxeFx:
                    PlayAnimationHit();
                    break;
            }
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
            m_PlayerCurrentHeal += (int)(m_PlayerMaxHeal * valuePercent);
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
            int newMax = m_PlayerHealBase + (data.Point * m_PlusHealValue); // tính toán điểm được cộng
            m_PlayerMaxHeal = newMax; // gán điểm đã cộng vào player max heal

            // Broadcast max heal mới cho UI
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.SEND_HEAL_VALUE, m_PlayerMaxHeal);
            }
        }
    }

    public void ResetPlayerHeal()
    {
        m_PlayerCurrentHeal = m_PlayerMaxHeal; // Khởi tạo player heal với giá trị cơ bản
        m_IsPlayerDeath = false;
        UpdateHealUI();
        Debug.Log($"m_PlayerCurrentHeal : {m_PlayerCurrentHeal}");
    }

    public void PlayAnimationBigHit() => PlayerManager.instance.playerAnim.GetAnimator().CrossFade(hashNameBigHit, 0.2f);
    public void PlayAnimationHit() => PlayerManager.instance.playerAnim.GetAnimator().CrossFade(hashNameHit, 0.2f);

    private void OnEventCheatPlayerHeal(object value)
    {
        if (value is int healValue)
        {
            m_PlayerMaxHeal = healValue;
            m_PlayerCurrentHeal = m_PlayerMaxHeal; // Đặt current heal bằng max heal
            ListenerManager.Instance.BroadCast(ListenType.SEND_HEAL_VALUE, m_PlayerMaxHeal);
            UpdateHealUI(); // gửi UI sau khi cheat hồi máu
        }
    }
}


