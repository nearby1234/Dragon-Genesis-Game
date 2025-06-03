using Microlight.MicroBar;
using Mono.Cecil.Cil;
using TMPro;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ScreenHealBarBoss : BaseScreen
{
    [SerializeField] private TextMeshProUGUI m_HealBarText;
    [SerializeField] private MicroBar m_HealBarMicroBar;
    [SerializeField] private Vector2 m_offset;


    public override void Show(object data)
    {
        base.Show(data);
        if (data != null)
        {
            if (data is DataBullTank dataBullTank)
            {
                m_HealBarText.text = dataBullTank.FirstPhase;
                Debug.Log($"m_HealBarText.text {m_HealBarText.text}");
                Debug.Log($"dataBullTank.FirstPhase {dataBullTank.FirstPhase}");
            }
        }

    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.BOSS_SEND_HEAL_VALUE, InitBossHealValue);
            ListenerManager.Instance.Register(ListenType.BOSS_UPDATE_HEAL_VALUE, UpdateBossHealValue);
            ListenerManager.Instance.Register(ListenType.BOSS_STATE_CURRENT, SetOffScreenHealBarBoss);
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_PLAYAGAIN, ReceiverClickPlayAgain);
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
            ListenerManager.Instance.Register(ListenType.BOSS_PHASESTATE_CURRENT, ReceiverEventPhaseState);
            ListenerManager.Instance.Register(ListenType.BOSSTYPE_SEND_HEAL_VALUE, InitHealBossType);
            ListenerManager.Instance.Register(ListenType.BOSSTYPE_UPDATE_HEAL_VALUE, UpdateHealBossType);
        }
        if (TryGetComponent<RectTransform>(out var transform))
        {
            transform.anchoredPosition = m_offset;
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.BOSS_SEND_HEAL_VALUE, InitBossHealValue);
            ListenerManager.Instance.Unregister(ListenType.BOSS_UPDATE_HEAL_VALUE, UpdateBossHealValue);
            ListenerManager.Instance.Unregister(ListenType.BOSS_STATE_CURRENT, SetOffScreenHealBarBoss);
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_PLAYAGAIN, ReceiverClickPlayAgain);
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverEventClickMainMenu);
            ListenerManager.Instance.Unregister(ListenType.BOSS_PHASESTATE_CURRENT, ReceiverEventPhaseState);
            ListenerManager.Instance.Unregister(ListenType.BOSSTYPE_SEND_HEAL_VALUE, InitHealBossType);
            ListenerManager.Instance.Unregister(ListenType.BOSSTYPE_UPDATE_HEAL_VALUE, UpdateHealBossType);
        }

    }
    private void InitBossHealValue(object value)
    {
        if (value != null)
        {
            if (value is float healValueBoss)
            {
                m_HealBarMicroBar.Initialize(healValueBoss);
            }
        }
    }
    private void UpdateBossHealValue(object value)
    {
        if (value != null)
        {
            if (value is float healValueBoss)
            {
                m_HealBarMicroBar.UpdateBar(healValueBoss);
            }
        }
    }
    private void SetOffScreenHealBarBoss(object value)
    {
        if (value != null)
        {
            if (value is WORMSTATE wormState)
            {
                if (wormState.Equals(WORMSTATE.DIE))
                {
                    this.Hide();
                }
            }
        }
    }
    private void ReceiverEventClickMainMenu(object value)
    {
        this.Hide();
    }
    private void ReceiverClickPlayAgain(object value)
    {
        this.Hide();
    }

    private void ReceiverEventPhaseState(object value)
    {
        if (value != null)
        {
            if (value is PhaseState blackboardVariable)
            {
                var newPhase = blackboardVariable;
                Debug.Log($"newPhase : {newPhase}");
                var dataBullTank = new DataBullTank();
                switch (newPhase)
                {
                    case PhaseState.First:
                        {
                            m_HealBarText.text = dataBullTank.FirstPhase;
                        }
                        break;
                    case PhaseState.Second:
                        m_HealBarText.text = dataBullTank.SecondPhase;
                        break;
                    case PhaseState.Third:
                        m_HealBarText.text = dataBullTank.ThirdPhase;
                        break;
                }

            }else
            {
                Debug.LogWarning("không có kiểu dữ liệu này");
            }    
        }
        else
        {
            Debug.LogWarning("không có sự kiện");
        }
    }
    private void InitHealBossType(object value)
    {
        if (value != null)
        {
            if (value is DataBullTankBoss dataBullTankBoss)
            {
                if (dataBullTankBoss.creepType == CreepType.BullTank)
                {
                    m_HealBarMicroBar.Initialize(dataBullTankBoss.Heal);
                }

            }
        }
    }
    private void UpdateHealBossType(object value)
    {
        if (value != null)
        {
            if (value is DataBullTankBoss dataBullTankBoss)
            {
                if (dataBullTankBoss.creepType == CreepType.BullTank)
                {
                    m_HealBarMicroBar.UpdateBar(dataBullTankBoss.Heal);
                }

            }
        }
    }

}
