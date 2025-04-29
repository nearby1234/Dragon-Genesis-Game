using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterStatsPoint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_StatsPointText;
    [SerializeField] private int m_CurrentPoint; // Điểm hiện tại của realtime của Statpoint
    [SerializeField] private int m_PendingPoint; // Điểm tạm thời đang chờ "Accept" trên StatPoint
    [SerializeField] private int m_MaxPoint;
    [SerializeField] private int m_DefaultPoint;
    [SerializeField] private Button m_PlusBtn;
    [SerializeField] private Button m_MinusBtn;
    public Button MinusBtn => m_MinusBtn;
    public Button PlusBtn => m_PlusBtn;
    public int CurrentPoint => m_CurrentPoint;

    public int PendingPoint => m_PendingPoint;

    private void Awake()
    {
        m_StatsPointText = GetComponent<TextMeshProUGUI>();
    }
    public void SetDefaultPoint(int defaultPoint) => m_DefaultPoint = defaultPoint;
    public void SetMaxPoint(int maxPoint) => m_MaxPoint = maxPoint;
    public void UpdateCurrentPlusPoint()
    {
        if (m_PendingPoint < m_MaxPoint - m_DefaultPoint)
        {
            m_PendingPoint++;
        }
    }
    public void UpdateCurrentMinusPoint()
    {
        if (m_PendingPoint == 0) return;
        
            m_PendingPoint--;
        
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
        m_CurrentPoint += m_PendingPoint;
        m_PendingPoint = 0;

        if (ListenerManager.HasInstance)
        {
            var payload = new StatPointUpdateData(this.gameObject.name, m_CurrentPoint);
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_POINT, payload);
        }
    }
    public void ResetPendingPoints()
    {
        m_PendingPoint = 0;
    }
    private void CreateStatsPointText(string name, int value, int maxvalue)
    {
        // Chỉ hiển thị điểm mặc định + tạm thời
        int displayValue = value + m_PendingPoint;
        m_StatsPointText.text = $"{name} : {displayValue} / {maxvalue}";
    }
}
