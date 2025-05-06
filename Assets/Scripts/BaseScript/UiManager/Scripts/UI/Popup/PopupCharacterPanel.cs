using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PopupCharacterPanel : BasePopup, IStateUi
{
    [SerializeField] private TextMeshProUGUI m_CharacterLevelTxt;
    [SerializeField] private TextMeshProUGUI m_PointTxt;
    //[SerializeField] private int m_PointDefaultValue = 0;
    [SerializeField] private int m_PointCurrentValue;
    [SerializeField] private Button m_ExitBtn;
    [SerializeField] private Button m_GearBtn;
    [SerializeField] private Button m_AcceptBtn;
    [SerializeField] private Button m_DenyBtn;
    [SerializeField] private int originalPointBackup;
    [SerializeField] private Vector2 m_PosMove;
    private Dictionary<TYPESTAT, int> _armorBonuses = new();
    [ShowInInspector]
    private Dictionary<string, int> originalPointBackupDict = new();
    private int m_HealValueMax; // giá trị máu tối đa (cập nhật khi cộng điểm stat)
    private int m_HealValueCurrent; // giá trị máu hiện tại (cập nhật realtime khi nhận sát thương/hồi máu)
    private int m_PlusHealValue; // giá trị hồi máu cộng thêm (cập nhật realtime khi cộng điểm stat)
    private int m_ManaValueMax;
    private int m_ManaValueCurrent; // giá trị mana hiện tại (cập nhật realtime khi tiêu hao mana)
    private int m_PlusManaValue; // giá trị hồi mana cộng thêm (cập nhật realtime khi cộng điểm stat)
    private int m_StaminaValueMax;
    private int m_StaminaValueCurrent; // giá trị thể lực hiện tại (cập nhật realtime khi tiêu hao thể lực)
    private int m_PlusStaminaValue; // giá trị hồi thể lực cộng thêm (cập nhật realtime khi cộng điểm stat)
    private int m_PlusDamageValue; // giá trị sát thương cộng thêm (cập nhật realtime khi cộng điểm stat)
    private int m_DamageValueCurrent; // giá trị sát thương hiện tại (cập nhật realtime khi cộng điểm stat)
    [InlineEditor]
    [SerializeField] private List<TextMeshProUGUI> m_CharacterStatsTxt;
    [SerializeField] private List<CharacterStatsPoint> m_CharacterStatsPointTxt;
    [SerializeField] private List<StatConfigSO> statConfigs;
    private Dictionary<string, StatConfigSO> configMap;



    public StateUi StateUi { get; private set; }

    private void Start()
    {
        InitializeStats();
        RegisterListeners();
        InitializeUI();
       
        SetStateUi(StateUi.Opening);

        //if (UIManager.HasInstance)
        //{
        //    PopupCharacterPanel popup = GetComponent<PopupCharacterPanel>();
        //    UIManager.Instance.AddStateInDict(popup);
        //}
        foreach (var statKey in Enum.GetValues(typeof(TYPESTAT)).Cast<TYPESTAT>())
            _armorBonuses[statKey] = 0;
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
            if (UIManager.Instance.GetObjectInDict<PopupInventory>())
            {
                GameManager.Instance.ShowCursor();
            }else
            {
                GameManager.Instance.HideCursor();
            }
        }
       
        
        if (PlayerManager.HasInstance)
        {
            PlayerManager.instance.isInteractingWithUI = false;
        }

        if (UIManager.HasInstance)
        {
            UIManager.Instance.SetStatePopup<PopupCharacterPanel>(StateUi.closing);
            UIManager.Instance.RemoverStateInDict<PopupCharacterPanel>();
        }
        this.Hide();
    }
    private void RegisterListeners()
    {
        if (!ListenerManager.HasInstance) return;

        ListenerManager.Instance.Register(ListenType.UI_SEND_LEVEL_PLAYER, UpdateCharacterLevelText);
        ListenerManager.Instance.Register(ListenType.UI_SEND_VALUE_LEVEL, ReceiverValuePoint);
        ListenerManager.Instance.Register(ListenType.PLAYER_SEND_HEAL_VALUE, UpdateHealValueCharacterText);
        ListenerManager.Instance.Register(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
        ListenerManager.Instance.Register(ListenType.PLAYER_SEND_DAMAGE_VALUE, UpdateDamageValueText);
        ListenerManager.Instance.Register(ListenType.PLAYER_SEND_STAMINA_VALUE, ReceiverPlayerStaminaValue);
        ListenerManager.Instance.Register(ListenType.PLAYER_UPDATE_STAMINA_VALUE, UpdateStaminaValueCharacterText);
        ListenerManager.Instance.Register(ListenType.PLAYER_SEND_MANA_VALUE, ReceiverPlayerManaValue);
        ListenerManager.Instance.Register(ListenType.PLAYER_UPDATE_MANA_VALUE, UpdateManaValueCharacterText);
        ListenerManager.Instance.Register(ListenType.SEND_ARMOR_EUIP, ReveiverEventArmor);

    }
    private void UnregisterListeners()
    {
        if (!ListenerManager.HasInstance) return;

        ListenerManager.Instance.Unregister(ListenType.UI_SEND_LEVEL_PLAYER, UpdateCharacterLevelText);
        ListenerManager.Instance.Unregister(ListenType.UI_SEND_VALUE_LEVEL, ReceiverValuePoint);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_HEAL_VALUE, UpdateHealValueCharacterText);
        ListenerManager.Instance.Unregister(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_DAMAGE_VALUE, UpdateDamageValueText);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_STAMINA_VALUE, ReceiverPlayerStaminaValue);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_UPDATE_STAMINA_VALUE, UpdateStaminaValueCharacterText);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_MANA_VALUE, ReceiverPlayerManaValue);
        ListenerManager.Instance.Unregister(ListenType.PLAYER_UPDATE_MANA_VALUE, UpdateManaValueCharacterText);
        ListenerManager.Instance.Unregister(ListenType.SEND_ARMOR_EUIP, ReveiverEventArmor);

    }
    private void InitializeUI()
    {
        InitStatTxt();
        UpdateValuePoint();

        foreach (var item in m_CharacterStatsPointTxt) originalPointBackupDict[item.name] = item.CurrentPoint;

        m_AcceptBtn.onClick.AddListener(OnAcceptButton);
        m_DenyBtn.onClick.AddListener(OnDenyButton);
        if (m_ExitBtn != null) m_ExitBtn.onClick.AddListener(() => HandlerExitSoundFx(OnExitButton));
        if (m_GearBtn != null) m_GearBtn.onClick.AddListener(() => HandlerClickSoundFx(OnClickGearButton));
        m_CharacterLevelTxt.text = $"Level {PlayerLevelManager.Instance.CurrentLevel}";
        originalPointBackup = PlayerLevelManager.Instance.TotalStatPoints;
        m_PointCurrentValue = originalPointBackup;
        if (PlayerManager.HasInstance)
        {
            if (PlayerManager.Instance.TryGetComponent<PlayerHeal>(out var playerHeal)) m_PlusHealValue = playerHeal.PlusHealValue;
            if (PlayerManager.Instance.TryGetComponent<PlayerMana>(out var playerMana)) m_PlusManaValue = playerMana.PlusManaValue;
            if (PlayerManager.Instance.TryGetComponent<PlayerStamina>(out var playerStamina)) m_PlusStaminaValue = playerStamina.PlusStaminaValue;
            if (PlayerManager.Instance.TryGetComponent<PlayerDamage>(out var playerDamage)) m_PlusDamageValue = playerDamage.PlusDamageValue;
        }
    }
    private void InitializeStats()
    {
        configMap = statConfigs.ToDictionary(cfg => cfg.statKey, cfg => cfg);
        if (PlayerManager.HasInstance)
        {
            foreach (var cfg in statConfigs)
            {
                switch (cfg.statKey)
                {
                    case "HealStatsTxt": PlayerManager.Instance.TryGetComponent<PlayerHeal>(out var ph); cfg.multiplier = ph?.PlusHealValue ?? 1; break;
                    case "IntelligentStatsTxt": PlayerManager.Instance.TryGetComponent<PlayerMana>(out var pm); cfg.multiplier = pm?.PlusManaValue ?? 1; break;
                    case "StaminaStatsTxt": PlayerManager.Instance.TryGetComponent<PlayerStamina>(out var ps); cfg.multiplier = ps?.PlusStaminaValue ?? 1; break;
                    case "StrengthStatsTxt": PlayerManager.Instance.TryGetComponent<PlayerDamage>(out var pd); cfg.multiplier = pd?.PlusDamageValue ?? 1; break;
                    default: cfg.multiplier = 1; break;
                }

                // Set up each stat point component
                if (configMap.TryGetValue(cfg.statKey, out var _cfg))
                {
                    var item = m_CharacterStatsPointTxt.FirstOrDefault(x => x.name == cfg.statKey);
                    if (item == null) continue;

                    item.SetDefaultPoint(_cfg.defaultPoint);
                    item.SetMaxPoint(_cfg.maxPoint);
                    string displayName = item.GetStatName();
                    item.SetStatsPointText(displayName);

                    // Hook buttons passing statKey
                    item.PlusBtn.onClick.AddListener(() => UpdatePlusPointsAndStatsText(item, cfg.statKey));
                    item.MinusBtn.onClick.AddListener(() => UpdateMinusPointsAndStatsText(item, cfg.statKey));
                }
            }
        }
    }
    private void UpdateCharacterLevelText(object value)
    {
        if (value is int level)
        {
            m_CharacterLevelTxt.text = $"Level {PlayerLevelManager.Instance.CurrentLevel}";
        }
    }
    private void ReceiverValuePoint(object value)
    {
        if (value is int pointvalue)
        {
            m_PointCurrentValue = (int)pointvalue;
            originalPointBackup = m_PointCurrentValue;

            UpdateValuePoint();
        }
    }
    private void UpdateValuePoint()
    {
        m_PointTxt.text = $"Điểm : {m_PointCurrentValue}";
    }
    private TextMeshProUGUI GetTextObjectFromList(string key)
        => m_CharacterStatsTxt.FirstOrDefault(x => x.name == key);
    private CharacterStatsPoint GetStatsPointObjectFromList(string key)
        => m_CharacterStatsPointTxt.FirstOrDefault(x => x.name == key);
    private void UpdateDamageValueText(object value)
    {
        if (value is int damageValue)
        {
            m_DamageValueCurrent = damageValue;
        }
    }
    private void UpdatePlusPointsAndStatsText(CharacterStatsPoint item, string statKey)
    {
        if (m_PointCurrentValue <= 0) return;
        item.UpdateCurrentPlusPoint();
        m_PointCurrentValue--;
        item.UpdateStatsPointText(configMap[statKey].displayName);
        UpdateStatPreview(statKey);
        UpdateValuePoint();
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("EquipItem");
        }
    }
    private void UpdateMinusPointsAndStatsText(CharacterStatsPoint item, string statKey)
    {
        if (item.PendingPoint <= 0 || m_PointCurrentValue >= originalPointBackup) return;
        item.UpdateCurrentMinusPoint();
        m_PointCurrentValue++;
        item.UpdateStatsPointText(configMap[statKey].displayName);
        UpdateStatPreview(statKey);
        UpdateValuePoint();
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("EquipItem");
        }
    }
    #region Heal
    // Sự kiện cập nhật current heal realtime
    private void UpdateHealValueCharacterText(object value)
    {
        if (value is int currentHeal)
        {
            m_HealValueCurrent = currentHeal;
            UpdateStatPreview("HealStatsTxt");
        }
    }
    // Sự kiện cập nhật max heal (khi điểm stat máu được cộng)
    private void ReceiverPlayerHealValue(object value)
    {
        if (value is int maxHeal)
        {
            m_HealValueMax = maxHeal;
            UpdateStatPreview("HealStatsTxt");
        }
    }
    #endregion

    #region Stamina
    private void UpdateStaminaValue(object value, Action<float> action)
    {
        if (value is float staminaValue)
        {
            int newValue = (int)(staminaValue);
            action(newValue);
            UpdateStatPreview("StaminaTxt");
        }
    }
    private void ReceiverPlayerStaminaValue(object value)
    {
        UpdateStaminaValue(value, newValue => m_StaminaValueMax = (int)newValue);
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
            UpdateStatPreview("StaminaTxt");
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
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupSettingBoxImg>();
        }
    }
    private void OnAcceptButton()
    {
        foreach (var statPoint in m_CharacterStatsPointTxt)
            if (statPoint.PendingPoint > 0) statPoint.ApplyPendingPoints();
        originalPointBackup = m_PointCurrentValue;
        PlayerLevelManager.Instance.UpdateTotalPoint(originalPointBackup);
        InitStatTxt(); UpdateValuePoint();
        foreach (var cfg in statConfigs)
        {
            // cfg.statKey tương ứng với tên trong m_CharacterStatsPointTxt
            UpdateStatPreview(cfg.statKey);
        }
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("EquipItem");
        }
    }

    private void OnDenyButton()
    {
        // 1) Reset pending level‐up cho mỗi stat và bảo toàn armor bonus
        foreach (var statPoint in m_CharacterStatsPointTxt)
        {
            if (statPoint.PendingPoint > 0)
                statPoint.ResetPendingPoints();

            // Sau khi reset pendingLevel, vẽ lại text (baseLevel + armorBonus)
            statPoint.RefreshTextAndBroadcast();
        }

        // 2) Phục hồi lại tổng điểm user có thể phân bổ
        m_PointCurrentValue = originalPointBackup;
        UpdateValuePoint();

        foreach (var cfg in statConfigs)
        {
            // cfg.statKey tương ứng với tên trong m_CharacterStatsPointTxt
            UpdateStatPreview(cfg.statKey);
        }
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("EquipItem");
        }
    }
    private void UpdateStatPreview(string statKey)
    {
        if (!configMap.TryGetValue(statKey, out var cfg))
            return;

        var statPoint = GetStatsPointObjectFromList(statKey);
        var ui = GetTextObjectFromList(cfg.previewUITextKey);
        if (statPoint == null || ui == null) return;

        // Pending point bonus
        int added = statPoint.PendingPoint;
        int gain = added * cfg.multiplier;

        // Armor bonus riêng theo stat
        // map statKey -> TYPESTAT
        TYPESTAT type = statKey switch
        {
            "StrengthStatsTxt" => TYPESTAT.STR,
            "HealStatsTxt" => TYPESTAT.HEA,
            "IntelligentStatsTxt" => TYPESTAT.ITE,
            "StaminaStatsTxt" => TYPESTAT.STA,
            _ => TYPESTAT.UNKOWN
        };
        int armorBonus = _armorBonuses.TryGetValue(type, out var b) ? b * cfg.multiplier : 0;

        string txt;
        if (type == TYPESTAT.STR)
        {
            // Strength: tính base gốc
            int totalCurrent = m_DamageValueCurrent;
            int baseValue = Mathf.Max(totalCurrent - gain - armorBonus, 0);

            txt = $"{cfg.displayName}: {baseValue}";
            if (gain > 0) txt += $" <color=#04FF00>+{gain}</color>";
            if (armorBonus > 0) txt += $" <color=#0010FF>+{armorBonus}</color>";
        }
        else
        {
            // Các stat có current/max
            var (current, max) = GetStatValues(statKey);

            txt = $"{cfg.displayName}: {current} / {max - armorBonus}";
            if (gain > 0) txt += $" <color=#04FF00>+{gain}</color>";
            if (armorBonus > 0) txt += $" <color=#0010FF>+{armorBonus}</color>";
        }

        ui.text = txt;
    }






    private void ReveiverEventArmor(object value)
    {
        if (value is ValueTuple<int, StatEquipData> tuple)
        {
            var (bonus, data) = tuple;

            // Lưu riêng
            _armorBonuses[data.StatType] = bonus;

            // Debug
            Debug.Log($"PopupCharacterPanel: armorBonus[{data.StatType}] = {bonus}");

            // Tìm statKey để vẽ lại đúng chỉ số
            string statKey = data.StatType switch
            {
                TYPESTAT.STR => "StrengthStatsTxt",
                TYPESTAT.HEA => "HealStatsTxt",
                TYPESTAT.ITE => "IntelligentStatsTxt",
                TYPESTAT.STA => "StaminaStatsTxt",
                _ => null
            };
            if (statKey != null)
                UpdateStatPreview(statKey);
        }
    }
    private void InitStatTxt()
    {
        // reset preview text to current values
        foreach (var cfg in statConfigs)
        {
            var ui = GetTextObjectFromList(cfg.previewUITextKey);
            var (cur, max) = GetStatValues(cfg.statKey);
            if (cfg.statKey == "StrengthStatsTxt")
            {
                ui.text = $"{cfg.displayName}: {cur}";
            }
            else
            {
                ui.text = $"{cfg.displayName}: {cur} / {max}";
            }

        }
    }
    private (int current, int max) GetStatValues(string key)
    {
        return key switch
        {
            "HealStatsTxt" => (m_HealValueCurrent, m_HealValueMax),
            "IntelligentStatsTxt" => (m_ManaValueCurrent, m_ManaValueMax),
            "StaminaStatsTxt" => (m_StaminaValueCurrent, m_StaminaValueMax),
            "StrengthStatsTxt" => (m_DamageValueCurrent, 0), // nếu có max damage, thêm biến tương ứng
            _ => (0, 0)
        };
    }

    public void SetStateUi(StateUi value)
    {
        StateUi = value;
    }

    public StateUi GetStateUi()
    {
        return StateUi;
    }
    public void SetPositionMove()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = m_PosMove;
    }
}
