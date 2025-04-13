using UnityEngine;
using UnityEngine.SearchService;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float m_MaxStamina; // Maximum stamina
    [SerializeField] private float m_CurrentStamina; // Current stamina
    [SerializeField] private float m_StaminaBase; // Base stamina
    [SerializeField] private float m_StaminaRegenRate; // Stamina regeneration rate
    [SerializeField] private float m_StaminaDrainRate; // Stamina drain rate when sprinting
    [SerializeField] private bool m_StaminaFull; // Check if stamina is full
    [SerializeField] private float m_StaminaConsumptionPercent;// Stamina consumption percentage for dodge

    private void Start()
    {
        m_StaminaBase = PlayerManager.instance.PlayerStatSO.m_PlayerStamina;
        m_MaxStamina = m_StaminaBase;
        m_CurrentStamina = m_MaxStamina; // Initialize current stamina to max stamina
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_PRESS_LEFT_SHIFT, UpdatePlayerPressLeftShift);
            ListenerManager.Instance.Register(ListenType.PLAYER_REGEN_STAMINA, RegenStamina);
            ListenerManager.Instance.Register(ListenType.PLAYER_PRESS_V_DODGE, ReceiverStateDodge);
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_STAMINA_VALUE, m_MaxStamina);
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_STAMINA_VALUE, m_CurrentStamina);
        }
    }
    private void Update()
    {
        StaminaFull();
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_PRESS_LEFT_SHIFT, UpdatePlayerPressLeftShift);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_REGEN_STAMINA, RegenStamina);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_PRESS_V_DODGE, ReceiverStateDodge);
        }
    }

    private void UpdatePlayerPressLeftShift(object value)
    {
        m_CurrentStamina -= Time.deltaTime * m_StaminaDrainRate;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_STAMINA_VALUE, m_CurrentStamina);
        }
    }
    private void StaminaFull()
    {
        if (m_CurrentStamina == m_MaxStamina)
        {
            m_StaminaFull = true;
        }
        else
        {
            m_StaminaFull = false;
        }
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_FULL_STAMINA, m_StaminaFull);
        }
    }
    private void RegenStamina(object value)
    {
        if (value is bool isRegen)
        {
            if (isRegen)
            {
                m_CurrentStamina += Time.deltaTime * m_StaminaRegenRate;
                m_CurrentStamina = Mathf.Min(m_CurrentStamina, m_MaxStamina); // Ensure stamina does not exceed max
                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_STAMINA_VALUE, m_CurrentStamina);
                }
            }
        }
    }
    private void ReceiverStateDodge(object value)
    {
        if (value is bool isDodge)
        {
            if (isDodge)
            {
                float m_StaminaConsumption = m_MaxStamina * m_StaminaConsumptionPercent;
                m_CurrentStamina -= m_StaminaConsumption;
                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_STAMINA_VALUE, m_CurrentStamina);
                }
            }
        }
    }
}
