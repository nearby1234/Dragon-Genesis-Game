using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum TYPESTAT
{
    UNKOWN =0,
    STR,
    STA,
    ITE,
    HEA,
    DEF,
}
public struct StatEquipData
{
    public TYPESTAT StatType;
    public int ValueDelta;     // +2, -2, …
}
[System.Serializable]
public class CharacterStatsPoint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_StatsPointText;
    [SerializeField] private int m_CurrentPoint; // điểm gốc + đã confirm
    [SerializeField] private int m_PendingLevelPoint; // điểm level‐up đang chờ confirm (user nhấn +)
    [SerializeField] private int m_ArmorBonus;  // bonus đến từ armor (có thể + hoặc -)
    [SerializeField] private int m_MaxPoint;
    [SerializeField] private int m_DefaultPoint;
    [SerializeField] private Button m_PlusBtn;
    [SerializeField] private Button m_MinusBtn;
    [SerializeField] private TYPESTAT typeStats;
    public Button MinusBtn => m_MinusBtn;
    public Button PlusBtn => m_PlusBtn;
    public int CurrentPoint => m_CurrentPoint;
    public int ArmorBonus => m_ArmorBonus;
  

    public int PendingPoint => m_PendingLevelPoint;

    private const string StrTxt = "Sức Mạnh";
    private const string StaTxt = "Thể Lực";
    private const string IteTxt = "Trí Tuệ";
    private const string HeaTxt = "Máu ";
    private const string DefTxt = "Phòng Ngự";

    private void Awake()
    {
        m_StatsPointText = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.EQUIP_STAT_UPDATE, OnEquipStatUpdate);
            ListenerManager.Instance.Register(ListenType.UNEQUIP_STAT_UPDATE, OnEquipStatUpdate);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.EQUIP_STAT_UPDATE, OnEquipStatUpdate);
            ListenerManager.Instance.Unregister(ListenType.UNEQUIP_STAT_UPDATE, OnEquipStatUpdate);
        }
    }
    public void SetDefaultPoint(int defaultPoint) => m_DefaultPoint = defaultPoint;
    public void SetMaxPoint(int maxPoint) => m_MaxPoint = maxPoint;
    public void UpdateCurrentPlusPoint()
    {
        if (m_PendingLevelPoint < m_MaxPoint - m_CurrentPoint)
            m_PendingLevelPoint++;
        RefreshTextAndBroadcast();
    }
    public void UpdateCurrentMinusPoint()
    {
        //if (m_PendingLevelPoint == 0) return;

        //    m_PendingLevelPoint--;
        if (m_PendingLevelPoint > 0)
            m_PendingLevelPoint--;
        RefreshTextAndBroadcast();
    }
    public void SetStatsPointText(string name)
    {
        CreateStatsPointText(name, m_DefaultPoint, m_MaxPoint);
    }
    public void UpdateStatsPointText(string name)
    {
        CreateStatsPointText(name, m_CurrentPoint, m_MaxPoint);
    }
   
    public void ApplyPendingPoints()
    {
        m_CurrentPoint += m_PendingLevelPoint;
        m_PendingLevelPoint = 0;
        RefreshTextAndBroadcast();
        //if (ListenerManager.HasInstance)
        //{
        //    var payload = new StatPointUpdateData(this.gameObject.name, m_CurrentPoint);
        //    ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_POINT, payload);
        //}
    }
    public void ResetPendingPoints()
    {
        m_PendingLevelPoint = 0;
    }
    private void CreateStatsPointText(string name, int value, int maxvalue)
    {
        // Chỉ hiển thị điểm mặc định + tạm thời
        int displayValue = value + m_PendingLevelPoint;
        if(m_ArmorBonus == 0)
        {
            m_StatsPointText.text = $"{name} : {displayValue}  / {maxvalue}";
        }else
        {
            m_StatsPointText.text = $"{name} : {displayValue} <color=#04FF00>+{m_ArmorBonus}</color> / {maxvalue}";
        }
       
    }
    public void OnEquipStatUpdate(object payload)
    {
        // 1) Cast payload về StatEquipData
        if (payload is not StatEquipData d || d.StatType != this.typeStats)
            return;

        // 2) Cập nhật armor bonus
        m_ArmorBonus += d.ValueDelta;
        RefreshTextAndBroadcast();

        // 3) Broadcast đúng kiểu ValueTuple<int, StatEquipData>
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(
                ListenType.SEND_ARMOR_EUIP,
                (m_ArmorBonus, d)    // ValueTuple<int,StatEquipData>
            );
            Debug.Log($"CharacterStatsPoint: mới m_ArmorBonus = {m_ArmorBonus}");
        }
    }
    public void RefreshTextAndBroadcast()
    {
        int basePlusLevel = m_CurrentPoint + m_PendingLevelPoint;
        if (m_ArmorBonus == 0)
        {
            m_StatsPointText.text = $"{GetStatName()} : {basePlusLevel} / {m_MaxPoint}";
        }
        else
        {
            m_StatsPointText.text =
              $"{GetStatName()} : {basePlusLevel} <color=#04FF00>+{m_ArmorBonus}</color> / {m_MaxPoint}";
        }
        int finalValue = basePlusLevel + m_ArmorBonus;
        ListenerManager.Instance.BroadCast(
            ListenType.PLAYER_SEND_POINT,
            new StatPointUpdateData(name, finalValue)
        );
    }

    // Giúp lấy lại tên stat ("Strength", "Mana", ...)
    public string GetStatName()
    {
        return typeStats switch
        {
            TYPESTAT.STR => StrTxt,
            TYPESTAT.ITE => IteTxt,
            TYPESTAT.HEA => HeaTxt,
            TYPESTAT.DEF => DefTxt,
            TYPESTAT.STA => StaTxt,
            _ => typeStats.ToString()
        };
    }


    public void ReceiverEventPointArmor(object value)
    {
        if(value is QuestItemSO itemSO)
        {
            switch(typeStats)
            {
                case TYPESTAT.STR:
                    m_ArmorBonus += itemSO.questItemData.plusStrengthArmor;
                    break;
            }
        }
    }
}
