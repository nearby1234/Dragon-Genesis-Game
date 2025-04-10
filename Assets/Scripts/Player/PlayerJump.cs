using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private Vector3 verticalMovement;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private InputAction m_ButtonSpace;
    //[SerializeField] private bool m_IsPressSpace;

    [SerializeField] private int maxJumpCount = 2;
    private int jumpCount = 0;

    private void Start()
    {
        m_ButtonSpace.Enable();
        m_ButtonSpace.performed += OnPerformedSpace;
        m_ButtonSpace.canceled += OnCancelSpace;
    }

    private void OnDestroy()
    {
        m_ButtonSpace.performed -= OnPerformedSpace;
        m_ButtonSpace.canceled -= OnCancelSpace;
        m_ButtonSpace.Disable();
    }

    private void OnPerformedSpace(InputAction.CallbackContext context)
    {
        //m_IsPressSpace = true;

        // Cập nhật trạng thái mặt đất ngay khi nhận input
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded)
        {
            jumpCount = 0;
        }

        // Nếu còn lượt nhảy, thực hiện nhảy
        if (jumpCount < maxJumpCount)
        {
            // Chỉ set lại vận tốc theo trục Y, để không can thiệp chuyển động ngang
            velocity = new Vector3(0, Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), 0);
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsJump", true);
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsGround", false);
            jumpCount++;
        }
    }

    private void OnCancelSpace(InputAction.CallbackContext context)
    {
        //m_IsPressSpace = false;
    }

    public void PlayerJumpUp()
    {
        // Cập nhật lại trạng thái mặt đất
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y <= 0)
        {
            // Khi thật sự chạm đất, reset vận tốc dọc và lượt nhảy
            velocity.y = -2f;
            jumpCount = 0;
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsGround", true);
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsJump", false);
        }
        else
        {
            // Áp dụng trọng lực lên vận tốc dọc
            velocity.y += Physics.gravity.y * Time.deltaTime;
             verticalMovement = new Vector3(0, velocity.y, 0) * Time.deltaTime;
        }

        PlayerManager.instance.controller.Move(verticalMovement);
    }
}
