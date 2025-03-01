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
    [SerializeField] private bool IsPressLeftShift;
    [SerializeField] private bool isMove;
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

    // Tham chiếu đến camera chính
    private Camera mainCamera;

    // Cờ cho phép di chuyển (có thể bị vô hiệu hóa trong dodge)
    public bool canMove = true;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        // Bật input cho phím Left Shift
        m_ButtonLeftShift.Enable();
        m_ButtonLeftShift.performed += OnLeftShiftPerformed;
        m_ButtonLeftShift.canceled += OnLeftShiftCancel;
    }

    private void OnDestroy()
    {
        m_ButtonLeftShift.performed -= OnLeftShiftPerformed;
        m_ButtonLeftShift.canceled -= OnLeftShiftCancel;
        m_ButtonLeftShift.Disable();
    }

    // Nhận input di chuyển (được gọi bởi Player Input component)
    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    private void OnLeftShiftPerformed(InputAction.CallbackContext context)
    {
        IsPressLeftShift = true;
        isMove = true;
    }

    private void OnLeftShiftCancel(InputAction.CallbackContext context)
    {
        IsPressLeftShift = false;
    }

    public void PlayerMovement()
    {
        // Nếu không được phép di chuyển (ví dụ như khi dodge) thì bỏ qua xử lý
        if (!canMove)
            return;

        // Cập nhật trạng thái di chuyển cho Animator
        if (inputVector.magnitude != 0)
        {
            isMove = true;
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsMove", true);
        }
        else
        {
            isMove = false;
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsMove", false);
        }

        // Lissage (mềm hóa) input
        if (inputVector.magnitude > Mathf.Epsilon)
        {
            smoothInputVector = Vector2.Lerp(smoothInputVector, inputVector, Time.deltaTime / smoothTime);
        }
        else
        {
            smoothInputVector = Vector2.zero;
        }

        // Xử lý tốc độ di chuyển theo các trạng thái khác nhau
        // Mặc định tốc độ là tốc độ đi bộ (walk)
        float maxSpeed = m_WalkSpeed;

        // Khi nhấn nút swap (R) thì đảo trạng thái: bật/tắt chế độ tăng tốc
        if (Input.GetKeyDown(m_ButtonSwap))
        {
            m_IsPressButtonSwap = !m_IsPressButtonSwap;
        }

        if (m_IsPressButtonSwap)
        {
            // Khi bật chế độ swap, tăng tốc độ cơ bản lên 2 đơn vị (cho trạng thái jogging)
            m_CurrentSpeed = m_SpeedMove + 2f;
            maxSpeed = m_JoggingSpeed;
        }
        else
        {
            m_CurrentSpeed = m_SpeedMove;
        }

        // Nếu đang bật swap và nhấn Left Shift, chuyển sang trạng thái chạy (run)
        if (m_IsPressButtonSwap && IsPressLeftShift)
        {
            m_CurrentSpeed = m_SpeedMove + 4f;
            maxSpeed = m_RunSpeed;
        }

        // Giới hạn vector input theo tốc độ tối đa của trạng thái hiện tại
        smoothInputVector = Vector2.ClampMagnitude(smoothInputVector, maxSpeed);
        m_SpeedBlendTree = smoothInputVector.magnitude;

        // Cập nhật các tham số animation cho blend tree
        if (smoothInputVector.magnitude > Mathf.Epsilon)
        {
            PlayerManager.instance.playerAnim.GetAnimator().SetFloat("MoveX", smoothInputVector.x, 0.2f, Time.deltaTime);
            PlayerManager.instance.playerAnim.GetAnimator().SetFloat("MoveY", smoothInputVector.y, 0.2f, Time.deltaTime);
            PlayerManager.instance.playerAnim.GetAnimator().SetFloat("Speed", m_SpeedBlendTree, 0.2f, Time.deltaTime);
        }
        else
        {
            PlayerManager.instance.playerAnim.GetAnimator().SetFloat("MoveX", 0);
            PlayerManager.instance.playerAnim.GetAnimator().SetFloat("MoveY", 0);
            PlayerManager.instance.playerAnim.GetAnimator().SetFloat("Speed", 0);
        }

        // Tính toán vector vận tốc dựa trên hướng di chuyển của player
        velocity = (transform.forward * smoothInputVector.y + transform.right * smoothInputVector.x).normalized;

        // Nếu có input, xoay player theo hướng của camera và di chuyển theo hướng đã tính
        if (inputVector.magnitude > Mathf.Epsilon)
        {
            // Lấy góc yaw (trục Y) của camera
            float targetYAngle = mainCamera.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetYAngle, 0);
            // Xoay player mượt dần về hướng camera
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_TurnSpeed * Time.deltaTime);
            // Di chuyển player dựa trên tốc độ hiện tại (m_CurrentSpeed)
            characterController.Move(m_CurrentSpeed * Time.deltaTime * velocity);
        }
    }
}
