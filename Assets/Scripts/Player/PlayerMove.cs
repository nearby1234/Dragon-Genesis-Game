using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private Vector2 inputVector;
    [SerializeField] private Vector2 smoothInputVector;
    public Vector2 SmoothInputVector => smoothInputVector;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private Vector3 rootMotion;
    public Vector3 RootMotion => rootMotion;

    [SerializeField] private bool isPressLeftShift;
    [SerializeField] private bool isMove;
    [SerializeField] private bool isPressButtonSwap = false;

    [Header("Speed Settings")]
    [SerializeField] private float speedMultiplier = 1.0f; // Biến điều chỉnh tốc độ
    [SerializeField] private float dodgeMultiplier = 1.0f; // Hệ số nhân riêng cho dodge


    [Header("Button Settings")]
    [SerializeField] private InputAction buttonLeftShift;
    [SerializeField] private KeyCode buttonSwap;

    [Header("Blend Tree Stats")]
    [SerializeField] private float walkSpeed = 0.3f;
    [SerializeField] private float joggingSpeed = 0.6f;
    [SerializeField] private float runSpeed = 1.0f;
    [SerializeField] private float speedBlendTree;

    [Header("Rotation Settings")]
    [SerializeField] private float turnSpeed = 5f;

    private Camera mainCamera;
    private Animator animator;
    private CharacterController characterController;
    public Vector2 CurrentInput => inputVector;
    public float SpeedMultiplier => speedMultiplier;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        buttonLeftShift.Enable();
        buttonLeftShift.performed += OnLeftShiftPerformed;
        buttonLeftShift.canceled += OnLeftShiftCancel;
    }

    private void OnDestroy()
    {
        buttonLeftShift.performed -= OnLeftShiftPerformed;
        buttonLeftShift.canceled -= OnLeftShiftCancel;
        buttonLeftShift.Disable();
    }

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    private void OnLeftShiftPerformed(InputAction.CallbackContext context)
    {
        isPressLeftShift = true;
        isMove = true;
    }

    private void OnLeftShiftCancel(InputAction.CallbackContext context)
    {
        isPressLeftShift = false;
    }

    private void Update()
    {
        PlayerMovement();
    }

    public void PlayerMovement()
    {
        if (inputVector.magnitude != 0)
        {
            isMove = true;
            animator.SetBool("IsMove", true);
        }
        else
        {
            isMove = false;
            animator.SetBool("IsMove", false);
        }

        smoothInputVector = inputVector.magnitude > Mathf.Epsilon
            ? Vector2.Lerp(smoothInputVector, inputVector, Time.deltaTime / smoothTime)
            : Vector2.zero;

        if (Input.GetKeyDown(buttonSwap))
        {
            isPressButtonSwap = !isPressButtonSwap;
        }

        float maxSpeed = isPressButtonSwap ? joggingSpeed : walkSpeed;
        if (isPressButtonSwap && isPressLeftShift)
            maxSpeed = runSpeed;

        smoothInputVector = Vector2.ClampMagnitude(smoothInputVector, maxSpeed);
        speedBlendTree = smoothInputVector.magnitude;

        animator.SetFloat("MoveX", smoothInputVector.x, 0.2f, Time.deltaTime);
        animator.SetFloat("MoveY", smoothInputVector.y, 0.2f, Time.deltaTime);
        animator.SetFloat("Speed", speedBlendTree, 0.2f, Time.deltaTime);

        if (smoothInputVector.magnitude > Mathf.Epsilon)
        {
            float targetYAngle = mainCamera.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetYAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void OnAnimatorMove()
    {
        // Nếu đang nhảy
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump_Loop"))
        {
            Vector3 localJumpMovement = new Vector3(smoothInputVector.x, 0, smoothInputVector.y) * 5 * Time.deltaTime;
            Vector3 jumpMovement = transform.TransformDirection(localJumpMovement);
            characterController.Move(jumpMovement);
        }
        else if (animator && characterController)
        {
            bool isDodging = animator.GetBool("IsDodge");
            // Nếu player đang di chuyển, áp dụng speedMultiplier (hoặc dodgeMultiplier khi dodge)
            // Nếu không, giữ giá trị root motion mặc định (nhân với 1)
            float multiplier = isMove ? (isDodging ? dodgeMultiplier : speedMultiplier) : 1f;
            rootMotion = animator.deltaPosition * multiplier;
            rootMotion.y = 0;
            characterController.Move(rootMotion);
        }
    }

}


