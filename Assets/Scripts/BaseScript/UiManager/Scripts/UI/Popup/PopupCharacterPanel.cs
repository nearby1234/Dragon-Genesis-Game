using Sirenix.OdinInspector;
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
    [SerializeField] private List<TextMeshProUGUI> m_CharacterStatsTxt;
    private int m_HealValueMax; // giá trị máu tối đa (cập nhật khi cộng điểm stat)
    private int m_HealValueCurrent; // giá trị máu hiện tại (cập nhật realtime khi nhận sát thương/hồi máu)
    private int m_ManaValueMax;
    private int m_StaminaValueMax;
    [InlineEditor]
    [SerializeField] private List<CharacterStatsPoint> m_CharacterStatsPointTxt;


    private void Awake()
    {

    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.UI_SEND_VALUE_LEVEL, UpdateCharacterLevelText);
            ListenerManager.Instance.Register(ListenType.UI_SEND_VALUE_LEVEL, ReceiverValuePoint);
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_HEAL_VALUE, UpdateHealValueCharacterText);
            ListenerManager.Instance.Register(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_DAMAGE_VALUE, UpdateDamageValueText);
        }
        if (m_ExitBtn == null)
        {
            Debug.LogError("m_ExitBtn chưa được gán trong Inspector!");
        }
        else
        {
            m_ExitBtn.onClick.AddListener(OnExitButton);
        }

        m_CharacterLevelTxt.text = "Level " + PlayerLevelManager.Instance.CurrentLevel.ToString();
        m_PointTxt.text = $"Điểm : {m_PointDefaultValue}";
        SetValueDefaultAndMaxPointText();
        m_HealValueMax = 100;
        m_HealValueCurrent = 100;
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.UI_SEND_VALUE_LEVEL, UpdateCharacterLevelText);
            ListenerManager.Instance.Unregister(ListenType.UI_SEND_VALUE_LEVEL, ReceiverValuePoint);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_HEAL_VALUE, UpdateHealValueCharacterText);
            ListenerManager.Instance.Unregister(ListenType.SEND_HEAL_VALUE, ReceiverPlayerHealValue);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_DAMAGE_VALUE, UpdateDamageValueText);
        }
    }
    private void Update()
    {
        UpdateValuePoint();
    }
    public void OnExitButton()
    {
        this.Hide();
    }

    private void UpdateCharacterLevelText(object value)
    {
        if (value is int level)
        {
            m_CharacterLevelTxt.text = "Level " + level.ToString();
        }
    }

    private void ReceiverValuePoint(object value)
    {
        if (value is int level)
        {
            // Mỗi lần lên cấp, cộng thêm 5 điểm vào số điểm hiện có
            m_PointCurrentValue += 5;
            m_PointTxt.text = $"Điểm : {m_PointCurrentValue}";
        }
    }
    private void UpdateValuePoint()
    {
        m_PointTxt.text = $"Điểm : {m_PointCurrentValue}";
    }

    private TextMeshProUGUI GetObjectFormListText(string name)
    {
        foreach (var item in m_CharacterStatsTxt)
        {
            if (item.name.Equals(name))
            {
                return item;
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy object có tên '{name}' trong m_CharacterStatsTxt.");
            }
        }
        return null;
    }
    private CharacterStatsPoint GetObjectFormListStatsPoint(string name)
    {
        foreach (var item in m_CharacterStatsPointTxt)
        {
            if (item.name.Equals(name))
            {
                return item;
            }
        }
        return null;
    }
    private void SetValueDefaultAndMaxPointText()
    {
        foreach (var item in m_CharacterStatsPointTxt)
        {
            item.SetDefaultPoint(0);
            item.SetMaxPoint(100);
            switch (item.name)
            {
                case "StrengthStatsTxt":
                    item.SetStatsPointText("Sức Mạnh");
                    item.PlusBtn.onClick.AddListener(() => UpdatePlusPointsAndStatsText(item, "Sức Mạnh"));
                    break;
                case "HealStatsTxt":
                    item.SetStatsPointText("Máu");
                    item.PlusBtn.onClick.AddListener(() => UpdatePlusPointsAndStatsText(item, "Máu"));
                    break;
                default:
                    Debug.LogWarning($"Không tìm thấy object có tên '{item.name}' trong m_CharacterStatsTxt.");
                    break;
            }
        }
    }

    // Sự kiện cập nhật current heal realtime
    private void UpdateHealValueCharacterText(object value)
    {
        if (value is int currentHeal)
        {
            m_HealValueCurrent = currentHeal;
            // Cập nhật text theo giá trị nhận được và m_HealValueMax (đã được khởi tạo từ sự kiện khác)
            TextMeshProUGUI healText = GetObjectFormListText("HealthTxt");
            if (healText != null)
            {
                healText.text = $"Máu : {m_HealValueCurrent} / {m_HealValueMax}";
            }
            else
            {
                Debug.LogWarning("Không tìm thấy object có tên 'HealTxt' trong m_CharacterStatsTxt.");
            }
        }
    }

    // Sự kiện cập nhật max heal (khi điểm stat máu được cộng)
    private void ReceiverPlayerHealValue(object value)
    {
        if (value is int maxHeal)
        {
            m_HealValueMax = maxHeal;
            TextMeshProUGUI healText = GetObjectFormListText("HealthTxt");
            if (healText != null) healText.text = $"Máu : {m_HealValueCurrent} / {m_HealValueMax}";
            else Debug.LogWarning("Không tìm thấy object có tên 'HealthTxt' trong m_CharacterStatsTxt.");
        }
    }
    private void UpdateDamageValueText(object value)
    {
        if (value is int damageValue)
        {
            TextMeshProUGUI damageText = GetObjectFormListText("DamageTxt");
            if (damageText != null)
            {
                damageText.text = $"Tấn Công : {damageValue}";
            }
            else
            {
                Debug.LogWarning("Không tìm thấy object có tên 'DamageTxt' trong m_CharacterStatsTxt.");
            }
        }
    }
    private void UpdatePlusPointsAndStatsText(CharacterStatsPoint item, string name)
    {
        if (m_PointCurrentValue <= 0)
        {
            return;
        }
        item.UpdateCurrentPoint();
        m_PointCurrentValue--;
        item.UpdateStatsPointText(name);
    }
}
