using System.Collections;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float m_MaxStamina; // Maximum stamina
    [SerializeField] private float m_CurrentStamina; // Current stamina
    [SerializeField] private float m_StaminaBase; // Base stamina
    [SerializeField] private float m_PlusStaminaValue; // Additional stamina value
    [SerializeField] private float m_StaminaRegenRate; // Stamina regeneration rate
    [SerializeField] private float m_StaminaDrainRate; // Stamina drain rate when sprinting
    [SerializeField] private bool m_StaminaFull; // Check if stamina is full
    [SerializeField] private float m_RegenDelay; // Delay before stamina starts regenerating
    [SerializeField] private float m_PlayerMoveConsumption; // Player movement consumption
    [SerializeField] private float m_PlayerDodgeConsumption; // Player dodge consumption
    [SerializeField] private GameObject m_effectStamina; // Stamina effect prefab

    private Coroutine regenCoroutine;

    public int PlusStaminaValue => (int)m_PlusStaminaValue; // Property to access the additional stamina value
    private void Start()
    {
        m_StaminaBase = PlayerManager.instance.PlayerStatSO.m_PlayerStamina;
        m_MaxStamina = m_StaminaBase;
        m_CurrentStamina = m_MaxStamina; // Initialize current stamina to max stamina
        Debug.Log($"m_CurrentStamina : {m_CurrentStamina}");

        if (m_effectStamina == null && EffectManager.HasInstance)
        {
            GameObject staminaPrefab = EffectManager.Instance.GetPrefabs("Stamina");
            m_effectStamina = Instantiate(staminaPrefab, this.transform); // Set parent ngay lúc Instantiate
            m_effectStamina.SetActive(false); // Tắt hiệu ứng ngay sau khi tạo
        }
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_PRESS_LEFT_SHIFT, UpdatePlayerPressLeftShift);
            ListenerManager.Instance.Register(ListenType.PLAYER_PRESS_V_DODGE, ReceiverStateDodge);
            ListenerManager.Instance.Register(ListenType.PLAYER_MOVE_STAMINA_CONSUMPTION, ReceiverPlayerMoveConsumption);
            ListenerManager.Instance.Register(ListenType.PLAYER_DODGE_STAMINA_CONSUMPTION, ReceiverPlayerDodgeConsumption);
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_POINT, ReceiverPoint);
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_STAMINA_VALUE, m_MaxStamina);
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_STAMINA_VALUE, m_CurrentStamina);
        }
    }
    private void Update()
    {
        StaminaFull();
        SendEventStaminaEmpty();
        CalcuMoveStaminaConsumption();
        CalcuDodgeStaminaConsumption();
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_PRESS_LEFT_SHIFT, UpdatePlayerPressLeftShift);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_PRESS_V_DODGE, ReceiverStateDodge);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_MOVE_STAMINA_CONSUMPTION, ReceiverPlayerMoveConsumption);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_DODGE_STAMINA_CONSUMPTION, ReceiverPlayerDodgeConsumption);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_POINT, ReceiverPoint);
        }
    }

    /// <summary>
    /// Gọi hàm này khi có tiêu hao stamina (ví dụ: dodge, sprint,…)
    /// </summary>
    public void StopRegen()
    {
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
        SetParticle(false);
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.StopLoopSE();
        }

    }
    /// <summary>
    /// Gọi khi người chơi ngừng tiêu hao stamina để bắt đầu hồi phục sau delay.
    /// </summary>
    public void StartRegen()
    {
        // Nếu coroutine đang chạy thì không cần bắt đầu lại
        regenCoroutine ??= StartCoroutine(RegenCoroutine());
    }
    /// <summary>
    /// Hàm tiêu hao stamina, ví dụ dodge tiêu hao một phần % của max stamina.
    /// </summary>
    /// <param name="percentConsumption">Phần trăm tiêu hao (0-1)</param>
    public void ConsumeStamina(float percentConsumption, bool IsPercent)
    {
        StopRegen();  // Dừng hồi phục khi tiêu hao mới xảy ra
        float amount;
        if (IsPercent)
        {
            amount = m_MaxStamina * percentConsumption * Time.deltaTime;
        }
        else
        {
            amount = percentConsumption;
        }

        m_CurrentStamina -= amount;
        m_CurrentStamina = Mathf.Max(0f, m_CurrentStamina);
        //Thông báo cập nhật stamina
         ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_STAMINA_VALUE, m_CurrentStamina);
    }

    private IEnumerator RegenCoroutine()
    {
        // Đợi delay trước khi hồi phục
        yield return new WaitForSeconds(m_RegenDelay);

        if (m_CurrentStamina < m_MaxStamina)
        {
            m_effectStamina.SetActive(true);
            if (AudioManager.HasInstance)
            {
                AudioManager.Instance.PlayLoopSE("RegenSound");
            }
            SetParticle(true); ;

            while (m_CurrentStamina < m_MaxStamina)
            {
                m_CurrentStamina += m_StaminaRegenRate * Time.deltaTime;


                m_CurrentStamina = Mathf.Min(m_CurrentStamina, m_MaxStamina);
                //Broadcast giá trị mới nếu cần
                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_STAMINA_VALUE, m_CurrentStamina);
                }
                yield return null;
                if (m_CurrentStamina >= m_MaxStamina)
                {
                    SetParticle(false);
                    if (AudioManager.HasInstance)
                    {
                        AudioManager.Instance.StopLoopSE();
                    }

                    break;
                }
            }
        }
        regenCoroutine = null;
    }

    private void UpdatePlayerPressLeftShift(object value)
    {
        if (value is float percentConsumption)
        {
            ConsumeStamina(percentConsumption, true);
            StartRegen();
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_STAMINA_VALUE, m_CurrentStamina);
            }
        }
    }
    public void ResetStamina()
    {
        m_CurrentStamina = m_MaxStamina; // Initialize current stamina to max stamina
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
    private void SendEventStaminaEmpty()
    {
        if (m_CurrentStamina <= 0)
        {
            m_StaminaFull = false;
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.PLAYER_STAMINA_EMPTY, m_StaminaFull);
            }
            if(UIManager.HasInstance)
            {
                NotifyMessageMission<PlayerStamina> notifyMessage = new()
                {
                    uiElement = this,
                    message = "Hết thể lực",
                };
                UIManager.Instance.ShowNotify<NotifySystem>(notifyMessage,true);
            }

        }
    }
    private void ReceiverStateDodge(object value)
    {
        if (value is float percentConsumption)
        {
            ConsumeStamina(percentConsumption, false);
            StartRegen();
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.PLAYER_UPDATE_STAMINA_VALUE, m_CurrentStamina);
            }
        }
    }
    private void ReceiverPlayerMoveConsumption(object value)
    {
        if (value is float percentConsumption)
        {
            m_PlayerMoveConsumption = percentConsumption;
        }
    }
    private void ReceiverPlayerDodgeConsumption(object value)
    {
        if (value is float percentConsumption)
        {
            m_PlayerDodgeConsumption = percentConsumption;
        }
    }
    private void CalcuMoveStaminaConsumption()
    {
        float amount = m_MaxStamina * m_PlayerMoveConsumption;
        // Nếu current stamina đủ để move, thì move được (false – không cạn)
        // Nếu m_CurrentStamina < amount, thì không đủ và là stamina cạn (true)
        bool isStaminaEmpty = m_CurrentStamina < amount;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_MOVE_STOP, isStaminaEmpty);
        }

    }
    private void CalcuDodgeStaminaConsumption()
    {
        float amount = m_PlayerDodgeConsumption;
        // Nếu current stamina đủ để dodge, thì dodge được (false – không cạn)
        // Nếu m_CurrentStamina < amount, thì không đủ và là stamina cạn (true)
        bool dodgeNotPossible = m_CurrentStamina < amount;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_DODGE_STOP, dodgeNotPossible);
        }
    }
    private void ReceiverPoint(object value)
    {
        if (value is StatPointUpdateData data && data.StatName == "StaminaStatsTxt")
        {
            // Cập nhật max heal mới dựa trên điểm stat
            int newMax = (int)m_StaminaBase + (data.Point * (int)m_PlusStaminaValue);
            m_MaxStamina = newMax;


            if (ListenerManager.HasInstance)
            {
                // Cập nhật max stamina mới tới UI
                ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_STAMINA_VALUE, m_MaxStamina);
            }
        }
    }
    private void SetParticle(bool isPlay)
    {
        if (m_effectStamina.TryGetComponent<ParticleSystem>(out var particle))
        {
            if (isPlay)
            {
                particle.Play();
            }
            else
            {
                particle.Stop();
            }
        }



    }

}
