using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterStatsPoint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_StatsPointText;
    public TextMeshProUGUI StatsPointText => m_StatsPointText;
    [SerializeField] private int m_CurrentPoint;
    public int CurrentPoint => m_CurrentPoint;

    [SerializeField] private int m_MaxPoint;
    public int MaxPoint => m_MaxPoint;

    [SerializeField] private int m_DefaultPoint;
    public int DefaultPoint => m_DefaultPoint;
    [SerializeField] private Button m_PlusBtn;
    public Button PlusBtn => m_PlusBtn;
    [SerializeField] private Button m_MinusBtn;
    public Button MinusBtn => m_MinusBtn;

    private void Awake()
    {
        m_StatsPointText = GetComponent<TextMeshProUGUI>();
    }
    public void SetDefaultPoint(int defaultPoint) => m_DefaultPoint = defaultPoint;
    public void SetMaxPoint(int maxPoint) => m_MaxPoint = maxPoint;
    public void UpdateCurrentPoint()
    {
        m_CurrentPoint++;
        if (ListenerManager.HasInstance)
        {
            // S? d?ng tên object (ví d?: "HealStatsTxt" hay "StrengthStatsTxt")
            var payload = new StatPointUpdateData(this.gameObject.name, m_CurrentPoint);
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_POINT, payload);
        }
    }    
    public void SetStatsPointText(string name)
    {
        CreateStatsPointText(name, m_DefaultPoint, m_MaxPoint);
    }
    public void UpdateStatsPointText(string name)
    {
        CreateStatsPointText(name, m_CurrentPoint, m_MaxPoint);
    }
    private void CreateStatsPointText(string name,int value , int maxvalue)
    {
        m_StatsPointText.text = $"{name} : {value} / {maxvalue}";
    }    
}
