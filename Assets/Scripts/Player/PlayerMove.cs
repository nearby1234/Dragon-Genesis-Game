using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private Vector2 inputVector;
    [SerializeField] private Vector2 smoothInputVector;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float m_SpeedMove;
    [SerializeField] private float m_CurrentSpeed;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private bool IsPressLeftShift;
    [SerializeField] private bool isMove;
    [SerializeField] private bool m_IsPressButtonSwap = false;

    [Header("Button Settings")]
    [SerializeField] private InputAction m_ButtonLeftShift;
    [SerializeField] private KeyCode m_ButtonSwap;

    [Header("Blend Tree Stats")]
    [SerializeField] private float m_WalkSpeed = 0.3f;
    [SerializeField] private float m_JoggingSpeed = 0.6f;
    [SerializeField] private float m_RunSpeed = 1.0f;
    [SerializeField] private float m_SpeedBlendTree;

    [Header("Rotation Settings")]
    [SerializeField] private float m_TurnSpeed = 5f; // tốc độ xoay của player

    // Tham chiếu đến camera chính
    private Camera mainCamera;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        // Enable input cho phím Left Shift
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

    // Hàm nhận input di chuyển
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
        // Xét trạng thái di chuyển để cập nhật animation "IsMove"
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

        // Smoothing input
        if (inputVector.magnitude > Mathf.Epsilon)
        {
            smoothInputVector = Vector2.Lerp(smoothInputVector, inputVector, Time.deltaTime / smoothTime);
        }
        else
        {
            smoothInputVector = Vector2.zero;
        }

        // Xử lý tốc độ di chuyển theo các trạng thái khác nhau
        float maxSpeed = m_WalkSpeed;

        if (Input.GetKeyDown(m_ButtonSwap))
        {
            m_IsPressButtonSwap = !m_IsPressButtonSwap;
        }

        if (m_IsPressButtonSwap)
        {
            m_CurrentSpeed = m_SpeedMove + 2;
            maxSpeed = m_JoggingSpeed;
        }
        else
        {
            m_CurrentSpeed = m_SpeedMove;
        }

        if (m_IsPressButtonSwap && IsPressLeftShift)
        {
            m_CurrentSpeed = m_SpeedMove + 4;
            maxSpeed = m_RunSpeed;
        }

        // Giới hạn vector input theo tốc độ tối đa
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

        // Di chuyển player nếu có input
        if (inputVector.magnitude > Mathf.Epsilon)
        {
            //Lấy góc yaw(trục Y) của camera
            float targetYAngle = mainCamera.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetYAngle, 0);
            // Xoay player mượt dần về hướng của camera
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_TurnSpeed * Time.deltaTime);
            characterController.Move(m_CurrentSpeed * Time.deltaTime * velocity);
        }

        // CHỈ XOAY PLAYER THEO HƯỚNG CAMERA KHI NHẤN PHÍM W (inputVector.y > 0)
        //if (inputVector.y > 0)
        //{
        //    Lấy góc yaw(trục Y) của camera
        //    float targetYAngle = mainCamera.transform.rotation.eulerAngles.y;
        //    Quaternion targetRotation = Quaternion.Euler(0, targetYAngle, 0);
        //    // Xoay player mượt dần về hướng của camera
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_TurnSpeed * Time.deltaTime);
        //}
    }
}
