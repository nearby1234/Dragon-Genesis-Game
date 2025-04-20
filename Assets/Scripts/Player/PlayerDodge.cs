using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Sirenix.OdinInspector;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] private float dodgeDuration = 0.5f; // Thời gian dodge tối thiểu
    [SerializeField] private InputAction dodgeAction;
    [SerializeField] private bool isDodging = false;
    [OnValueChanged("OnStaminaPercentChanged", true)]
    [SerializeField] private float StaminaConsumptionPercent; // Tỉ lệ tiêu hao stamina khi dodge
    private float previousPercent; // Giá trị cũ, để so sánh

    private float dodgeTimer = 0f;
    private PlayerMove playerMove;
    private Animator playerAnimator;

    // Biến theo dõi: nếu true -> stamina cạn, không cho chạy được
    private bool isStaminaEmpty;


    private void Start()
    {
        playerMove = PlayerManager.instance.playerMove;
        playerAnimator = PlayerManager.instance.playerAnim.GetAnimator();
        dodgeAction.Enable();
        dodgeAction.performed += OnDodgePerformed;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_DODGE_STOP, ReceiverStateDodgeEvent);
        }
        BroadCastStaminaPercent();
    }

    private void OnDestroy()
    {
        dodgeAction.performed -= OnDodgePerformed;
        dodgeAction.Disable();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_DODGE_STOP, ReceiverStateDodgeEvent);
        }
    }
    private void Update()
    {
        if (isDodging)
        {
            HandleDodge();
        }
    }

    private void OnDodgePerformed(InputAction.CallbackContext context)
    {
        // Nếu đang dodge rồi, không làm gì thêm
        if (!isDodging)
        {
            // Kiểm tra trạng thái stamina; nếu cạn thì không cho dodge
            if (isStaminaEmpty)
            {
                Debug.Log("[PlayerDodge] Không dodge vì stamina cạn.");
                return;
            }
            StartDodge();
        }
    }
    private void StartDodge()
    {
        isDodging = true;
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_PRESS_V_DODGE, StaminaConsumptionPercent);
        }
        dodgeTimer = dodgeDuration;

        // Disable movement and set dodge state in Animator
        playerMove.canMove = false;
        if(isStaminaEmpty)
        {
            playerAnimator.SetBool("IsDodge", false);
            return;
        }
        playerAnimator.SetBool("IsDodge", true);
    }
    private void HandleDodge()
    {
        dodgeTimer -= Time.deltaTime;

        if (dodgeTimer <= 0f)
        {
            EndDodge();
        }
    }
    private void EndDodge()
    {
        isDodging = false;

        // Re-enable movement and reset dodge state in Animator
        playerMove.canMove = true;
        playerAnimator.SetBool("IsDodge", false);
    }
    private void OnStaminaPercentChanged()
    {
        if (!Mathf.Approximately(previousPercent, StaminaConsumptionPercent))
        {
            previousPercent = StaminaConsumptionPercent;
            BroadCastStaminaPercent();
        }
    }
    private void BroadCastStaminaPercent()
    {
        ListenerManager.Instance?.BroadCast(ListenType.PLAYER_DODGE_STAMINA_CONSUMPTION, StaminaConsumptionPercent);
    }
    // Nhận sự kiện cập nhật trạng thái stamina dodge (true: stamina cạn, false: đủ)
    private void ReceiverStateDodgeEvent(object value)
    {
        if (value is bool empty)
        {
            isStaminaEmpty = empty;
        }
    }
    public void DodgeSound()
    {
        if (AudioManager.HasInstance )
        {
            AudioManager.Instance.PlaySE("DodgeSound");
        }
    }
    public void DodgeGroundSound()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("RunSound");
        }
    }
}
