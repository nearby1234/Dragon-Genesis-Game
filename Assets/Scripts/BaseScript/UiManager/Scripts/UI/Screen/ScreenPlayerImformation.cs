using Microlight.MicroBar;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScreenPlayerImformation : BaseScreen
{
    [SerializeField] private Slider m_ExpBar;
    [SerializeField] private Button m_CharacterStatsBtn;
    [SerializeField] private InputAction m_ButtonPress;
    [SerializeField] private TextMeshProUGUI m_LevelTxt;
    [SerializeField] private TextMeshProUGUI m_HealValueTxt;
    [SerializeField] private TextMeshProUGUI m_ManaValueTxt;
    [SerializeField] private TextMeshProUGUI m_StaminaValueTxt;
    [SerializeField] private TextMeshProUGUI m_ExpValueTxt;
    [SerializeField] private MicroBar m_HealBarValue;
    [SerializeField] private MicroBar m_ManaBarValue;
    [SerializeField] private MicroBar m_StaminaBarValue;
    private float m_HealValueMax;
    private float m_ManaValueMax;
    private float m_StaminaValueMax;
    private float m_HealValueUpdate;
    private float m_ManaValueUpdate;
    private float m_StaminaValueUpdate;
    private float m_ExpValueMax;
    private float m_ExpValueUpdate;
    private bool m_IsOpen;

    private void Start()
    {
        m_ButtonPress.Enable();
        m_ButtonPress.performed += OnClickButton;
        RegisterListeners();
        Initialized();
    }
    private void OnDestroy()
    {
        m_ButtonPress.performed -= OnClickButton;
        UnRegisterListeners();
        m_ButtonPress.Disable();
    }
    private void Update()
    {
        m_ExpValueMax = PlayerLevelManager.Instance.CurrentLevelUp.expNeedLvup;
        m_ExpValueUpdate = PlayerLevelManager.Instance.DisPlayExp;
        UpdateText(m_HealValueTxt, $"{(int)m_HealValueUpdate} / {m_HealValueMax} ");
        UpdateText(m_ManaValueTxt, $"{(int)m_ManaValueUpdate} / {m_ManaValueMax} ");
        UpdateText(m_StaminaValueTxt, $"{(int)m_StaminaValueUpdate} / {m_StaminaValueMax} ");
        UpdateText(m_ExpValueTxt, $"{(int)m_ExpValueUpdate} / {m_ExpValueMax} ");
        UpdateValue();
    }
    private void Initialized()
    {
        m_ExpBar.maxValue = PlayerLevelManager.Instance.CurrentLevelUp.expNeedLvup;
        UpdateText(m_HealValueTxt, $"{(int)m_HealValueUpdate} / {m_HealValueMax} ");
        UpdateText(m_ManaValueTxt, $"{(int)m_ManaValueUpdate} / {m_ManaValueMax} ");
        UpdateText(m_StaminaValueTxt, $"{(int)m_StaminaValueUpdate} / {m_StaminaValueMax} ");
        UpdateText(m_ExpValueTxt, $"{(int)m_ExpValueUpdate} / {m_ExpValueMax} ");
        m_ExpValueMax = PlayerLevelManager.Instance.CurrentLevelUp.expNeedLvup;
        m_ExpValueUpdate = PlayerLevelManager.Instance.DisPlayExp;

    }
    private void UpdateValue()
    {
        if (PlayerLevelManager.Instance == null) return;
        m_ExpBar.value = PlayerLevelManager.Instance.DisPlayExp;
        m_ExpBar.maxValue = PlayerLevelManager.Instance.CurrentLevelUp.expNeedLvup;
        m_LevelTxt.text = PlayerLevelManager.Instance.CurrentLevel.ToString();
    }
    private void RegisterListeners()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.UI_SEND_SCREEN_SLIDER_EXP, m_ExpBar);
            ListenerManager.Instance.Register(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_HEAL_VALUE, UpdatePlayerHealValue);
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_STAMINA_VALUE, ReceiverPlayerStaminaValue);
            ListenerManager.Instance.Register(ListenType.PLAYER_UPDATE_STAMINA_VALUE, UpdatePlayerStaminaValue);
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_MANA_VALUE, ReceiverPlayerManaValue);
            ListenerManager.Instance.Register(ListenType.PLAYER_UPDATE_MANA_VALUE, UpdatePlayerManaValue);

        }
    }
    private void UnRegisterListeners()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_HEAL_VALUE, UpdatePlayerHealValue);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_STAMINA_VALUE, ReceiverPlayerStaminaValue);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_UPDATE_STAMINA_VALUE, UpdatePlayerStaminaValue);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_MANA_VALUE, ReceiverPlayerManaValue);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_UPDATE_MANA_VALUE, UpdatePlayerManaValue);
        }
    }
    public void OnClickButton(InputAction.CallbackContext callback)
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupCharacterPanel>();
            UIManager.Instance.SetStatePopup<PopupCharacterPanel>(StateUi.Opening);
            StateUi popupInventory = UIManager.Instance.GetStatePopup<PopupInventory>();
            if (popupInventory.Equals(StateUi.Opening))
            {
                PopupCharacterPanel popupCharacterPanel = UIManager.Instance.GetComponentbase<PopupCharacterPanel>();
                popupCharacterPanel.SetPositionMove();
                PopupInventory popupInventorySlot = UIManager.Instance.GetComponentbase<PopupInventory>();
                popupInventorySlot.SetPositionMove();
            }
           var popup = UIManager.Instance.GetComponentbase<PopupCharacterPanel> ();
           if (popup != null)
            {
                UIManager.Instance.AddStateInDict(popup);
            }
        }
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ClickSound");
        }
        if (GameManager.HasInstance)
        {
            GameManager.Instance.ShowCursor();
        }
    }

    public void ReceiverPlayerHealValue(object value)
    {
        if (value != null && value is int maxHeal)
        {
            m_HealValueMax = maxHeal;
            float ratio = m_HealValueUpdate / m_HealValueMax;
            m_HealBarValue.Initialize(ratio); // Thiết lập max cho thanh
        }
    }
    private void UpdatePlayerHealValue(object value)
    {
        if (value != null && value is int currentHeal)
        {
            m_HealValueUpdate = currentHeal;
            float ratio = (float)m_HealValueUpdate / m_HealValueMax;
            m_HealBarValue.UpdateBar(ratio);
        }
    }
    private void ReceiverPlayerStaminaValue(object value)
    {
        if (value != null && value is float maxStamina)
        {
            m_StaminaValueMax = maxStamina;
            float ratio = m_StaminaValueUpdate / m_StaminaValueMax;
            m_StaminaBarValue.Initialize(ratio); // Thiết lập max cho thanh

        }
    }
    private void UpdatePlayerStaminaValue(object value)
    {
        if (value != null && value is float currentStamina)
        {
            m_StaminaValueUpdate = currentStamina;
            float ratio = m_StaminaValueUpdate / m_StaminaValueMax;
            m_StaminaBarValue.UpdateBar(ratio, true);

        }
    }
    private void ReceiverPlayerManaValue(object value)
    {
        if (value != null && value is float maxMana)
        {
            m_ManaValueMax = maxMana;
            float ratio = m_ManaValueUpdate / m_ManaValueMax;
            m_ManaBarValue.Initialize(ratio); // Thiết lập max cho thanh

        }
    }
    private void UpdatePlayerManaValue(object value)
    {
        if (value != null && value is float currentMana)
        {
            m_ManaValueUpdate = currentMana;
            float ratio = m_ManaValueUpdate / m_ManaValueMax;
            m_ManaBarValue.UpdateBar(ratio);

        }
    }
    private void UpdateText(TextMeshProUGUI textMeshProUGUI, string content)
    {
        textMeshProUGUI.text = content;
    }
}
