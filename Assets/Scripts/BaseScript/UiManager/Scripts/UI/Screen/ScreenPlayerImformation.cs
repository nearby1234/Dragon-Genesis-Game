using Microlight.MicroBar;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenPlayerImformation: BaseScreen
{
    [SerializeField] private Slider m_ExpBar;
    [SerializeField] private TextMeshProUGUI m_LevelTxt;
    [SerializeField] private TextMeshProUGUI m_HealValueTxt;
    [SerializeField] private TextMeshProUGUI m_ManaValueTxt;
    [SerializeField] private TextMeshProUGUI m_StaminaValueTxt;
    [SerializeField] private MicroBar m_HealBarValue;
    [SerializeField] private MicroBar m_ManaBarValue;
    [SerializeField] private MicroBar m_StaminaBarValue;
    private float m_HealValueMax;
    private float m_ManaValueMax;
    private float m_StaminaValueMax;
    private float m_HealValueUpdate;
    private float m_ManaValueUpdate;
    private float m_StaminaValueUpdate;


    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_HEAL_VALUE, UpdatePlayerHealValue);
            ListenerManager.Instance.BroadCast(ListenType.UI_SEND_SCREEN_SLIDER_EXP, m_ExpBar);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_HEAL_VALUE, UpdatePlayerHealValue);
        }
    }
  
    private void Update()
    {
        m_HealValueTxt.text = $"{m_HealValueMax} / {(m_HealValueUpdate <= 0 ? 0 : m_HealValueUpdate)}";
    }

    public override void Show(object data)
    {
        base.Show(data);
    }

    public override void Hide()
    {
        base.Hide();
    }
    public override void Init()
    {
        base.Init();
    }

    public void ReceiverPlayerHealValue(object value)
    {
        if (value != null)
        {
            if (value is int playerHeal)
            {
                m_HealBarValue.Initialize((float)playerHeal);
                m_HealValueMax = playerHeal;
                m_HealValueUpdate = playerHeal;
            }
        }
    }
    private void UpdatePlayerHealValue(object value)
    {
        if (value != null)
        {
            if (value is int playerHeal)
            {
                m_HealBarValue.UpdateBar((float)playerHeal);
                m_HealValueUpdate = playerHeal;

            }
        }
    }
}
