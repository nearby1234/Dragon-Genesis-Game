using Microlight.MicroBar;
using TMPro;
using UnityEngine;

public class ScreenPlayerImformation: BaseScreen
{
    [SerializeField] private TextMeshProUGUI m_LevelTxt;
    [SerializeField] private MicroBar m_HealBarValue;
    [SerializeField] private MicroBar m_ManaBarValue;
    [SerializeField] private MicroBar m_StaminaBarValue;

  
    private void OnEnable()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_HEAL_VALUE, UpdatePlayerHealValue);
        }
    }
    private void OnDisable()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_HEAL_VALUE, UpdatePlayerHealValue);
        }
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

            }
        }
    }
}
