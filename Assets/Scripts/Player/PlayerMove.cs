using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


public enum MOVESTATE
{
    DEFAULT = 0,
    WALKING,
    RUNNING,
    JOGGING
}
public class PlayerMove : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private Vector2 inputVector;
    [SerializeField] private Vector2 smoothInputVector;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private MOVESTATE moveState;
    public Vector3 Velocity => velocity;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float m_SpeedMove = 5f;  // Tốc độ cơ bản khi di chuyển
    [SerializeField] private float m_CurrentSpeed;    // Tốc độ di chuyển hiện tại được cập nhật theo trạng thái
    [SerializeField] private float smoothTime = 0.1f;

    // Tỉ lệ tiêu hao stamina khi di chuyển, sử dụng Odin để trigger thay đổi ngay trong Play Mode
    [OnValueChanged("OnStaminaPercentChanged", true)]
    [SerializeField] private float StaminaConsumptionPercent;
    private float previousPercent; // Giá trị cũ, để so sánh

    [SerializeField] private bool IsPressLeftShift;
    // Nút Swap (R) chuyển sang chế độ đi bộ (tốc độ thấp) - toggle
    [SerializeField] private bool m_IsPressButtonSwap = false;

    [Header("Button Settings")]
    [SerializeField] private InputAction m_ButtonLeftShift;
    [SerializeField] private KeyCode m_ButtonSwap = KeyCode.R; // Nút chuyển đổi chế độ (R)

    [Header("Blend Tree Stats")]
    [SerializeField] private float m_WalkSpeed = 0.3f;      // Giới hạn tốc độ khi ở chế độ đi bộ
    [SerializeField] private float m_JoggingSpeed = 0.6f;     // Giới hạn tốc độ khi jogging (stamina cạn)
    [SerializeField] private float m_RunSpeed = 1.0f;         // Giới hạn tốc độ khi chạy nhanh

    [Header("Rotation Settings")]
    [SerializeField] private float m_TurnSpeed = 5f; // Tốc độ xoay của player

    private Camera mainCamera;

    // Cho phép di chuyển hay không (có thể dùng để freeze khi cần)
    public bool canMove = true;
    // Biến theo dõi: nếu true -> stamina cạn, không cho chạy được
    private bool isStaminaEmpty;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        m_ButtonLeftShift.Enable();
        m_ButtonLeftShift.performed += OnLeftShiftPerformed;
        m_ButtonLeftShift.canceled += OnLeftShiftCancel;

        // Lưu lại giá trị ban đầu cho stamina percent
        previousPercent = StaminaConsumptionPercent;
        // Gửi sự kiện khởi tạo ban đầu
        BroadCastStaminaPercent();

        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_MOVE_STOP, ReceiverStateMove);
        }
    }

    private void Update()
    {
        // Toggle chế độ Swap (đi bộ) khi nhấn phím Swap (R)
        if (Input.GetKeyDown(m_ButtonSwap))
        {
            m_IsPressButtonSwap = !m_IsPressButtonSwap;
        }

        // Nếu nhấn LeftShift, gửi sự kiện cập nhật (nếu cần hiển thị UI thông tin)
        if (m_ButtonLeftShift.IsPressed()&& inputVector.magnitude > Mathf.Epsilon)
        {
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.PLAYER_PRESS_LEFT_SHIFT, StaminaConsumptionPercent);
            }
        }
    }

    private void OnDestroy()
    {
        m_ButtonLeftShift.performed -= OnLeftShiftPerformed;
        m_ButtonLeftShift.canceled -= OnLeftShiftCancel;
        m_ButtonLeftShift.Disable();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_MOVE_STOP, ReceiverStateMove);
        }
    }
    public void PlayerMovement()
    {
        if (!canMove) return;

        bool isMoving = inputVector.magnitude > Mathf.Epsilon;
        PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsMove", isMoving);

        smoothInputVector = Vector2.Lerp(smoothInputVector, isMoving ? inputVector : Vector2.zero, Time.deltaTime / smoothTime);
        
        // Set to exactly zero if very close to zero
        if (!isMoving && smoothInputVector.magnitude < 0.01f)
        {
            smoothInputVector = Vector2.zero;
        }

        UpdateSpeedAndBlendTree();

        if (smoothInputVector.magnitude > Mathf.Epsilon)
        {
            UpdateAnimatorParameters();
        }
        else
        {
            ResetAnimatorParameters();
        }

        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * smoothInputVector.y + camRight * smoothInputVector.x;
        velocity = moveDir;
        // Lấy input



        if (isMoving)
        {
            RotateTowardsCamera();
            characterController.Move(m_CurrentSpeed * Time.deltaTime * velocity.normalized);

        }
    }
    private void UpdateSpeedAndBlendTree()
    {
        float maxSpeed;

        if (m_IsPressButtonSwap)
        {
            // Nếu bật chế độ swap → chuyển sang đi bộ (tốc độ thấp)
            m_CurrentSpeed = m_SpeedMove;
            maxSpeed = m_WalkSpeed;
        }
        else
        {
            if (IsPressLeftShift)
            {
                PlayerStatSO playerStatSO = DataManager.Instance.GetData<PlayerStatSO, PlayerType>(PlayerType.Player);
                if (!isStaminaEmpty)
                {
                    // Stamina đủ: cho phép chạy nhanh
                    m_CurrentSpeed = m_SpeedMove + playerStatSO.speedRun;
                    maxSpeed = m_RunSpeed;
                }
                else
                {
                    // Stamina cạn: chỉ tăng nhẹ (jogging)
                    m_CurrentSpeed = m_SpeedMove + playerStatSO.speedJogging;
                    maxSpeed = m_JoggingSpeed;
                }
            }
            else
            {
                // Không nhấn LeftShift → tốc độ cơ bản (với blend tree có thể dựa vào jogging speed)
                m_CurrentSpeed = m_SpeedMove + 2f;
                maxSpeed = m_JoggingSpeed;
            }
        }

        smoothInputVector = Vector2.ClampMagnitude(smoothInputVector, maxSpeed);
        // Cập nhật moveState tương ứng
        if (m_IsPressButtonSwap)
        {
            moveState = MOVESTATE.WALKING;
        }
        else if (IsPressLeftShift && !isStaminaEmpty)
        {
            moveState = MOVESTATE.RUNNING;
        }
        else if (!IsPressLeftShift || isStaminaEmpty)
        {
            moveState = MOVESTATE.JOGGING;
        }
        else
        {
            moveState = MOVESTATE.DEFAULT;
        }
    }

    private void UpdateAnimatorParameters()
    {
        Vector2 dir = smoothInputVector.normalized;
        float speed = smoothInputVector.magnitude;
        var animator = PlayerManager.instance.playerAnim.GetAnimator();
        
        // Apply smoothing to MoveX and MoveY
        float currentMoveX = animator.GetFloat("MoveX");
        float currentMoveY = animator.GetFloat("MoveY");
        
        float smoothMoveX = Mathf.Lerp(currentMoveX, dir.x, Time.deltaTime / smoothTime);
        float smoothMoveY = Mathf.Lerp(currentMoveY, dir.y, Time.deltaTime / smoothTime);
        
        animator.SetFloat("MoveX", smoothMoveX, 0f, 0f);
        animator.SetFloat("MoveY", smoothMoveY, 0f, 0f);
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }

    private void ResetAnimatorParameters()
    {
        var animator = PlayerManager.instance.playerAnim.GetAnimator();
        
        // Get current values
        float currentMoveX = animator.GetFloat("MoveX");
        float currentMoveY = animator.GetFloat("MoveY");
        float currentSpeed = animator.GetFloat("Speed");
        
        // Smoothly lerp to zero
        float smoothMoveX = Mathf.Lerp(currentMoveX, 0f, Time.deltaTime / smoothTime);
        float smoothMoveY = Mathf.Lerp(currentMoveY, 0f, Time.deltaTime / smoothTime);
        float smoothSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime / smoothTime);
        
        // Set the smoothed values
        animator.SetFloat("MoveX", smoothMoveX, 0f, 0f);
        animator.SetFloat("MoveY", smoothMoveY, 0f, 0f);
        animator.SetFloat("Speed", smoothSpeed, 0f, 0f);
        
        // If values are very close to zero, set them to exactly zero
        if (Mathf.Abs(smoothMoveX) < 0.01f && Mathf.Abs(smoothMoveY) < 0.01f && smoothSpeed < 0.01f)
        {
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 0f);
            animator.SetFloat("Speed", 0f);
        }
    }

    private void RotateTowardsCamera()
    {
        float targetYAngle = mainCamera.transform.rotation.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetYAngle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_TurnSpeed * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    private void OnLeftShiftPerformed(InputAction.CallbackContext context)
    {
        IsPressLeftShift = true;
    }

    private void OnLeftShiftCancel(InputAction.CallbackContext context)
    {
        IsPressLeftShift = false;
    }

    public void ChangeIsCanMove(int move) // Được gọi bởi animation event
    {
        canMove = (move == 1);
    }
  
    private void BroadCastStaminaPercent()
    {
        if (ListenerManager.HasInstance)
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_MOVE_STAMINA_CONSUMPTION, StaminaConsumptionPercent);
    }

    // Khi nhận được sự kiện liên quan đến trạng thái stamina từ hệ thống (ví dụ hàm tính tiêu hao stamina)
    private void ReceiverStateMove(object value)
    {
        if (value is bool empty)
        {
            // Nếu empty == true → stamina cạn
            isStaminaEmpty = empty;
        }
    }
    public void MoveSound()
    {
        if (AudioManager.HasInstance && moveState == MOVESTATE.JOGGING)
        {
            AudioManager.Instance.PlayPlayerSound("MoveSound", 0.3f);
        }
    }

    public void RunningSound()
    {
        if (AudioManager.HasInstance && moveState == MOVESTATE.RUNNING)
        {
            AudioManager.Instance.PlayPlayerSound("RunSound", 0.5f);
        }
    }
    public void ResetInput()
    {
        // Xóa hết dư input cũ để blend tree không kẹt
        inputVector = Vector2.zero;
        smoothInputVector = Vector2.zero;
        velocity = Vector3.zero;

        // (Nếu bạn có parameter blend "Speed/X/Y",
        //  cũng reset lại animator params)
        var anim = PlayerManager.instance.playerAnim.GetAnimator();
        anim.SetFloat("MoveX", 0f);
        anim.SetFloat("MoveY", 0f);
        anim.SetFloat("Speed", 0f);
    }

}
