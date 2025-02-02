using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Vector2 inputVector;
    [SerializeField] private Vector3 velovity;
    [SerializeField] private Vector2 smoothInputVector;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float m_SpeedMove;
    [SerializeField] private float m_CurrentSpeed;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private bool IsPressLeftShift;
    [SerializeField] private bool isMove;
    [SerializeField] private bool m_IsPressButtonSwap = false;
    [Header("Button Stat")]
    [SerializeField] private InputAction m_ButtonLeftShift;
    [SerializeField] private KeyCode m_ButtonSwap;
    [Space(1)]

    [Header("Stat Blend Tree")]
    [SerializeField] private float m_WalkSpeed = 0.3f;
    [SerializeField] private float m_JoggingSpeed = 0.6f;
    [SerializeField] private float m_RunSpeed = 1.0f;
    [SerializeField] private float m_SpeedBlendTree;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void Start()
    {
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
        if (inputVector.magnitude != 0)
        {
            isMove = true;
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsMove", true);
        }
        if (inputVector.magnitude > Mathf.Epsilon)
        {
            smoothInputVector = Vector2.Lerp(smoothInputVector, inputVector, Time.deltaTime / smoothTime);
        }
        else
        {
            isMove = false;
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsMove", false);
            smoothInputVector = Vector2.zero;
        }

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
        else if (!m_IsPressButtonSwap) m_CurrentSpeed = m_SpeedMove;
        if (m_IsPressButtonSwap && IsPressLeftShift)
        {
            m_CurrentSpeed = m_SpeedMove + 4;
            maxSpeed = m_RunSpeed;
        }
        smoothInputVector = Vector2.ClampMagnitude(smoothInputVector, maxSpeed);
        m_SpeedBlendTree = smoothInputVector.magnitude;

        velovity = (transform.forward * smoothInputVector.y + transform.right * smoothInputVector.x).normalized;

        if (inputVector.magnitude > Mathf.Epsilon)
        {
            characterController.Move(m_CurrentSpeed * Time.deltaTime * velovity);
        }
        //characterController.Move(m_CurrentSpeed * Time.deltaTime * velovity);

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
    }

}


