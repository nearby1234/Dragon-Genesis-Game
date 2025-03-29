using Microlight.MicroBar;
using UnityEngine;

public class BatPanel : MonoBehaviour
{
    [SerializeField] private MicroBar m_BatMicroBar;
    [SerializeField] private EnemyHeal m_EnemyHeal;

    private void Awake()
    {
        m_BatMicroBar = GetComponentInChildren<MicroBar>();
        m_EnemyHeal = GetComponentInParent<EnemyHeal>();
       
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CREEP_SEND_HEAL_VALUE, SetHealValue);
            ListenerManager.Instance.Register(ListenType.CREEP_UPDATE_HEAL_VALUE, UpdateHealValue);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CREEP_SEND_HEAL_VALUE, SetHealValue);
            ListenerManager.Instance.Unregister(ListenType.CREEP_UPDATE_HEAL_VALUE, UpdateHealValue);
        }
    }
    public void SetHealValue(object value)
    {
        if (value != null && value is (GameObject senderObj, EnemyStatSO statSO))
        {
            // So sánh với GameObject của EnemyHeal (chứa EnemyController)
            if (senderObj.Equals(m_EnemyHeal.gameObject))
            {
                m_BatMicroBar.Initialize((float)statSO.heal);
            }
        }
    }
    public void UpdateHealValue(object value)
    {
        if (value != null && value is (GameObject senderObj, int enemyHealValue))
        {
            if (senderObj.Equals(m_EnemyHeal.gameObject))
            {
                m_BatMicroBar.UpdateBar((float)enemyHealValue);
            }
        }
    }
}
