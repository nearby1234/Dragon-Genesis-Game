using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PopupCharacterPanel : BasePopup
{
    [SerializeField] private TextMeshProUGUI m_CharacterLevelTxt;
    [SerializeField] private TextMeshProUGUI m_PointTxt;
    [SerializeField] private int m_PointDefaultValue = 0;
    [SerializeField] private int m_PointCurrentValue;
    [SerializeField] private Button m_ExitBtn;
    [SerializeField] private Button m_GearBtn;
    [SerializeField] private List<TextMeshProUGUI> m_CharacterStatsTxt;
    private int m_HealValueMax; // giá trị máu tối đa (cập nhật khi cộng điểm stat)
    private int m_HealValueCurrent; // giá trị máu hiện tại (cập nhật realtime khi nhận sát thương/hồi máu)
    private int m_ManaValueMax;
    private int m_ManaValueCurrent; // giá trị mana hiện tại (cập nhật realtime khi tiêu hao mana)
    private int m_StaminaValueMax;
    private int m_StaminaValueCurrent; // giá trị thể lực hiện tại (cập nhật realtime khi tiêu hao thể lực)
    [InlineEditor]
    [SerializeField] private List<CharacterStatsPoint> m_CharacterStatsPointTxt;
    private void Start()
    {
        RegisterListeners();
        InitializeUI();
        InitializeStats();
    }
    private void OnDestroy()
    {
        UnregisterListeners();
    }
    private void Update()
    {
        UpdateValuePoint();
    }
    public void OnExitButton()
    {
        if(GameManager.HasInstance)
        {
            GameManager.Instance.HideCursor();
        }
        this.Hide();
    }
    private void RegisterListeners()
    {
        if (!ListenerManager.HasInstance) return;

        ListenerManager.Instance.Register(ListenType.UI_SEND_VALUE_LEVEL, UpdateCharacterLevelText);
        ListenerManager.Instance.Register(ListenType.UI_SEND_VALUE_LEVEL, ReceiverValuePoint);
        ListenerManager.Instance.Register(ListenType.PLAYER_SEND_HEAL_VALUE, UpdateHealValueCharacterText);
        ListenerManager.Instance.Register(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
        ListenerManager.Instance.Register(ListenType.PLAYER_SEND_DAMAGE_VALUE, UpdateDamageValueText);
        ListenerManager.Instance.Register(ListenType.PLAYER_SEND_STAMINA_VALUE, ReceiverPlayerStaminaValue);
        ListenerManager.Instance.Register(ListenType.PLAYER_UPDATE_STAMINA_VALUE, UpdateStaminaValueCharacterText);
        ListenerManager.Instance.Register(ListenType.PLAYER_SEND_MANA_VALUE, ReceiverPlayerManaValue);
        ListenerManager.Instance.Register(ListenType.PLAYER_UPDATE_MANA_VALUE, UpdateManaValueCharacterText);
    }
    private void UnregisterListeners()
    {
        if (!ListenerManager.HasInstance) return;

        ListenerManager.Instance.Unregister(ListenType.UI_SEND_VALUE_LEVEL, UpdateCharacterLevelText);
        ListenerManager.Instance.Unregister(ListenType.UI_SEND_VALUE_LEVEL, ReceiverValuePoint);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_HEAL_VALUE, UpdateHealValueCharacterText);
        ListenerManager.Instance.Unregister(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_DAMAGE_VALUE, UpdateDamageValueText);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_STAMINA_VALUE, ReceiverPlayerStaminaValue);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_UPDATE_STAMINA_VALUE, UpdateStaminaValueCharacterText);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_MANA_VALUE, ReceiverPlayerManaValue);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_UPDATE_MANA_VALUE, UpdateManaValueCharacterText);
    }
    private void InitializeUI()
    {
        if (m_ExitBtn != null)
        {
            m_ExitBtn.onClick.AddListener(() => HandlerExitSoundFx(OnExitButton));
        }
        if(m_GearBtn != null)
        {
            m_GearBtn.onClick.AddListener(() => HandlerClickSoundFx(OnClickGearButton));
        }
        m_CharacterLevelTxt.text = $"Level {PlayerLevelManager.Instance.CurrentLevel}";
        m_PointCurrentValue = m_PointDefaultValue;
        m_PointTxt.text = $"Points: {m_PointDefaultValue}";
    }
    private void InitializeStats()
    {
        foreach (var item in m_CharacterStatsPointTxt)
        {
            item.SetDefaultPoint(0);
            item.SetMaxPoint(100);

            switch (item.name)
            {
                case "StrengthStatsTxt":
                    ConfigureStatPoint(item, "Sức Mạnh");
                    break;
                case "HealStatsTxt":
                    ConfigureStatPoint(item, "Máu");
                    break;
                case "IntelligentStatsTxt":
                    ConfigureStatPoint(item, "Năng Lượng");
                    break;
                case "StaminaStatsTxt":
                    ConfigureStatPoint(item, "Thể Lực");
                    break;
                case "AmorStatsTxt":
                    ConfigureStatPoint(item, "Phòng Ngự");
                    break;
                default:
                    Debug.LogWarning($"Object with name '{item.name}' not found in m_CharacterStatsTxt.");
                    break;
            }
        }
    }
    private void ConfigureStatPoint(CharacterStatsPoint item, string statName)
    {
        item.SetStatsPointText(statName);
        item.PlusBtn.onClick.AddListener(() => UpdatePlusPointsAndStatsText(item, statName));
    }

    private void UpdateCharacterLevelText(object value)
    {
        if (value is int level)
        {
            m_CharacterLevelTxt.text = $"Level {level}";
        }
    }

    private void ReceiverValuePoint(object value)
    {
        if (value is int )
        {
            // Mỗi lần lên cấp, cộng thêm 5 điểm vào số điểm hiện có
            m_PointCurrentValue += 5;
            UpdateValuePoint();
        }
    }
    
    private void UpdateValuePoint()
    {
        m_PointTxt.text = $"Điểm : {m_PointCurrentValue}";
    }

    private TextMeshProUGUI GetTextObjectFromList(string name)
    {
        return m_CharacterStatsTxt.Find(item => item.name.Equals(name));
    }
    private CharacterStatsPoint GetStatsPointObjectFromList(string name)
    {
        return m_CharacterStatsPointTxt.Find(item => item.name.Equals(name));
    }
    private void UpdateDamageValueText(object value)
    {
        if (value is int damageValue)
        {
            UpdateStatText("DamageTxt", $"Sát Thương: {damageValue}");
        }
    }
    private void UpdatePlusPointsAndStatsText(CharacterStatsPoint item, string name)
    {
        if (m_PointCurrentValue <= 0) return;

        item.UpdateCurrentPoint();
        m_PointCurrentValue--;
        item.UpdateStatsPointText(name);
    }
    #region Heal
    // Sự kiện cập nhật current heal realtime
    private void UpdateHealValueCharacterText(object value)
    {
        if (value is int currentHeal)
        {
            m_HealValueCurrent = currentHeal;
            UpdateStatText("HealthTxt", $"Máu: {m_HealValueCurrent} / {m_HealValueMax}");
        }
    }

    // Sự kiện cập nhật max heal (khi điểm stat máu được cộng)
    private void ReceiverPlayerHealValue(object value)
    {
        if (value is int maxHeal)
        {
            m_HealValueMax = maxHeal;
            UpdateStatText("HealthTxt", $"Máu: {m_HealValueCurrent} / {m_HealValueMax}");
        }
    }
    #endregion 
    
    #region Stamina

    private void UpdateStaminaValue(object value ,Action<float> action)
    {
        if(value is float staminaValue)
        {
            int newValue = (int)(staminaValue);
            action(newValue);
            UpdateStatText("StaminaTxt", $"Thể Lực: {m_StaminaValueCurrent} / {m_StaminaValueMax}");
        }
    
    }
    private void ReceiverPlayerStaminaValue(object value)
    {
        UpdateStaminaValue(value,newValue => m_StaminaValueMax = (int)newValue);
    }
    private void UpdateStaminaValueCharacterText(object value)
    {
        UpdateStaminaValue(value, newValue => m_StaminaValueCurrent = (int)newValue);
    }
    #endregion
    #region Mana
    private void UpdateManaValue(object value, Action<float> action)
    {
        if (value is float manaValue)
        {
            int newValue = (int)(manaValue);
            action(newValue);
            UpdateStatText("ManaTxt", $"Năng Lượng: {m_ManaValueCurrent} / {m_ManaValueMax}");
        }
    }
    private void ReceiverPlayerManaValue(object value)
    {
        UpdateManaValue(value, newValue => m_ManaValueMax = (int)newValue);
    }
    private void UpdateManaValueCharacterText(object value)
    {
        UpdateManaValue(value, newValue => m_ManaValueCurrent = (int)newValue);
    }

    #endregion

    private void UpdateStatText(string statName, string text)
    {
        var statText = GetTextObjectFromList(statName);
        if (statText != null)
        {
            statText.text = text;
        }
    }
    private void HandlerClickSoundFx(Action action)
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ClickSound");
            action?.Invoke();
        }
    }
    private void HandlerExitSoundFx(Action action)
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ExitSound");
            action?.Invoke();
        }
    }

    private void OnClickGearButton()
    {
        if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupSettingBoxImg>();
        }
    }
}
