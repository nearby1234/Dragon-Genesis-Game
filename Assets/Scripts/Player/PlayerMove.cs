using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private Vector2 inputVector;
    [SerializeField] private Vector2 smoothInputVector;
    [SerializeField] private Vector3 velocity;
    public Vector3 Velocity => velocity;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float m_SpeedMove = 5f;  // Tốc độ cơ bản khi di chuyển
    [SerializeField] private float m_CurrentSpeed;    // Tốc độ di chuyển hiện tại được cập nhật theo trạng thái
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float m_Time;
    [SerializeField] private float m_TimeRegen;
    [SerializeField] private bool IsPressLeftShift;
    [SerializeField] private bool m_IsRegen;
    [SerializeField] private bool m_StaminaFull;
    [SerializeField] private bool m_IsPressButtonSwap = false; // Toggle tốc độ khi nhấn nút R

    [Header("Button Settings")]
    [SerializeField] private InputAction m_ButtonLeftShift;
    [SerializeField] private KeyCode m_ButtonSwap = KeyCode.R; // Nút chuyển đổi tốc độ (R)

    [Header("Blend Tree Stats")]
    [SerializeField] private float m_WalkSpeed = 0.3f;
    [SerializeField] private float m_JoggingSpeed = 0.6f;
    [SerializeField] private float m_RunSpeed = 1.0f;
    [SerializeField] private float m_SpeedBlendTree;

    [Header("Rotation Settings")]
    [SerializeField] private float m_TurnSpeed = 5f; // Tốc độ xoay của player

    private Camera mainCamera;

    public bool canMove = true;

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
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_FULL_STAMINA, UpdatePlayerStaminaState);
        }
    }
    private void Update()
    {
        if (m_ButtonLeftShift.IsPressed())
        {
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.PLAYER_PRESS_LEFT_SHIFT);
            }
        }
        SetTimerRegen();
    }

    private void OnDestroy()
    {
        m_ButtonLeftShift.performed -= OnLeftShiftPerformed;
        m_ButtonLeftShift.canceled -= OnLeftShiftCancel;
        m_ButtonLeftShift.Disable();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_FULL_STAMINA, UpdatePlayerStaminaState);
        }
    }
    public void PlayerMovement()
    {
        if (!canMove) return;

        bool isMoving = inputVector.magnitude > Mathf.Epsilon;
        PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsMove", isMoving);

        smoothInputVector = isMoving
            ? Vector2.Lerp(smoothInputVector, inputVector, Time.deltaTime / smoothTime)
            : Vector2.zero;

        UpdateSpeedAndBlendTree();

        if (smoothInputVector.magnitude > Mathf.Epsilon)
        {
            UpdateAnimatorParameters();
        }
        else
        {
            ResetAnimatorParameters();
        }

        velocity = (transform.forward * smoothInputVector.y + transform.right * smoothInputVector.x).normalized;

        if (isMoving)
        {
            RotateTowardsCamera();
            characterController.Move(m_CurrentSpeed * Time.deltaTime * velocity);
        }
    }

    private void UpdateSpeedAndBlendTree()
    {
        float maxSpeed = m_WalkSpeed;

        if (Input.GetKeyDown(m_ButtonSwap))
        {
            m_IsPressButtonSwap = !m_IsPressButtonSwap;
        }

        if (m_IsPressButtonSwap)
        {
            m_CurrentSpeed = m_SpeedMove + 2f;
            maxSpeed = m_JoggingSpeed;
        }
        else
        {
            m_CurrentSpeed = m_SpeedMove;
        }

        if (m_IsPressButtonSwap && IsPressLeftShift)
        {
            m_CurrentSpeed = m_SpeedMove + 4f;
            maxSpeed = m_RunSpeed;
        }

        smoothInputVector = Vector2.ClampMagnitude(smoothInputVector, maxSpeed);
        m_SpeedBlendTree = smoothInputVector.magnitude;
    }

    private void UpdateAnimatorParameters()
    {
        var animator = PlayerManager.instance.playerAnim.GetAnimator();
        animator.SetFloat("MoveX", smoothInputVector.x, 0.2f, Time.deltaTime);
        animator.SetFloat("MoveY", smoothInputVector.y, 0.2f, Time.deltaTime);
        animator.SetFloat("Speed", m_SpeedBlendTree, 0.2f, Time.deltaTime);
    }
    private void ResetAnimatorParameters()
    {
        var animator = PlayerManager.instance.playerAnim.GetAnimator();
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", 0);
        animator.SetFloat("Speed", 0);
    }
    private void RotateTowardsCamera()
    {
        float targetYAngle = mainCamera.transform.rotation.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0, targetYAngle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_TurnSpeed * Time.deltaTime);
    }

    private void OnMove(InputValue value)
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

    public void ChangeIsCanMove(int move) // animation event
    {
         canMove = move == 1;
    }
    private void SetTimerRegen()
    {
        if (!IsPressLeftShift)
        {
            if (m_StaminaFull)
            {
                ResetRegenTimer();
                return;
            }

            m_Time += Time.deltaTime;
            if (m_Time >= m_TimeRegen)
            {
                m_Time = m_TimeRegen;
                m_IsRegen = true;

                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.PLAYER_REGEN_STAMINA, m_IsRegen);
                }
            }
        }
        else
        {
            ResetRegenTimer();
        }
    }
    private void ResetRegenTimer()
    {
        m_Time = 0;
        m_IsRegen = false;
    }
    private void UpdatePlayerStaminaState(object value)
    {
        if (value is bool staminaState)
        {
            m_StaminaFull = staminaState;
        }
    }

}
