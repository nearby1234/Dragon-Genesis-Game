using Microlight.MicroBar;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ScreenHealBarBoss : BaseScreen
{
    [SerializeField] private TextMeshProUGUI m_HealBarText;
    [SerializeField] private MicroBar m_HealBarMicroBar;
    [SerializeField] private Vector2 m_offset;

    

    public override void Init()
    {
        base.Init();
    }
    public override void Hide()
    {
        base.Hide();
    }
    public override void Show(object data)
    {
        base.Show(data);
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
        }
        RectTransform transform = GetComponent<RectTransform>();
        if (transform != null)
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
        }
    }
    private void InitBossHealValue(object value)
    {
        if(value !=null)
        {
            if(value is float healValueBoss)
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
}
