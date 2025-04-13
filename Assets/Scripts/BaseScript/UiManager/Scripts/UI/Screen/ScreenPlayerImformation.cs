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
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_STAMINA_VALUE, ReceiverPlayerStaminaValue);
            ListenerManager.Instance.Register(ListenType.PLAYER_UPDATE_STAMINA_VALUE, UpdatePlayerStaminaValue);
            ListenerManager.Instance.BroadCast(ListenType.UI_SEND_SCREEN_SLIDER_EXP, m_ExpBar);
        }
        m_ExpBar.maxValue = PlayerLevelManager.Instance.CurrentLevelUp.expNeedLvup;
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_HEAL_VALUE, UpdatePlayerHealValue);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_STAMINA_VALUE, ReceiverPlayerStaminaValue);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_UPDATE_STAMINA_VALUE, UpdatePlayerStaminaValue);
        }
    }
  
    private void Update()
    {
        m_HealValueTxt.text = $"{m_HealValueUpdate} / {m_HealValueMax} ";
        m_StaminaValueTxt.text = $"{(int)m_StaminaValueUpdate} / {m_StaminaValueMax} ";
        m_ExpBar.value = PlayerLevelManager.Instance.DisPlayExp;
        m_ExpBar.maxValue = PlayerLevelManager.Instance.CurrentLevelUp.expNeedLvup;
        m_LevelTxt.text = PlayerLevelManager.Instance.CurrentLevel.ToString();
    }
    public void ReceiverPlayerHealValue(object value)
    {
        if (value != null && value is int maxHeal)
        {
            m_HealBarValue.Initialize(maxHeal); // Thiết lập max cho thanh
            m_HealValueMax = maxHeal;
        }
    }
    private void UpdatePlayerHealValue(object value)
    {
        if (value != null && value is int currentHeal)
        {
            m_HealBarValue.UpdateBar(currentHeal);
            m_HealValueUpdate = currentHeal;
        }
    }
    private void ReceiverPlayerStaminaValue(object value)
    {
        if (value != null && value is float maxStamina)
        {
            m_StaminaBarValue.Initialize(maxStamina); // Thiết lập max cho thanh
            m_StaminaValueMax = maxStamina;
        }
    }
    private void UpdatePlayerStaminaValue(object value)
    {
        if (value != null && value is float currentStamina)
        {
            m_StaminaBarValue.UpdateBar(currentStamina,true);
            m_StaminaValueUpdate = currentStamina;
        }
    }
}
