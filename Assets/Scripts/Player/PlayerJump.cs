using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private Vector3 velocity; // Tốc độ rơi/tăng dần
    [SerializeField] private Transform groundCheck; // Transform kiểm tra mặt đất
    [SerializeField] private bool isGrounded; // Kiểm tra nhân vật có trên mặt đất hay không
    [SerializeField] private float jumpHeight = 2f; // Chiều cao khi nhảy
    [SerializeField] private float groundDistance = 0.4f; // Bán kính kiểm tra mặt đất
    [SerializeField] private LayerMask groundMask; // Lớp mặt đất

    //[SerializeField] private Transform player;
    //[SerializeField] private Transform terrain;
    [SerializeField] private float distance;
    [SerializeField] private InputAction m_ButtonSpace;
    [SerializeField] private bool m_IsPressSpace;

    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private int jumpCount;

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
    private void Update()
    {
        //Distance();
    }

    private void OnPerformedSpace(InputAction.CallbackContext context)
    {
        m_IsPressSpace = true;
        if (isGrounded && m_IsPressSpace)
        {
            //jumpCount++;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y); // u2 = -2 * gravity * jumpHeight 
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsJump", true);
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsGround", false);
            //PlayerManager.instance.playerAnim.GetAnimator().SetTrigger("isJump");
        }
        //else if(!isGrounded && m_IsPressSpace)
        //{
        //    jumpCount++;
        //    if (jumpCount == 2)
        //    {
        //        PlayerManager.instance.playerAnim.GetAnimator().SetTrigger("DoubleJump");
        //    }
        //}

    }
    private void OnCancelSpace(InputAction.CallbackContext context)
    {
        m_IsPressSpace = false;
    }
    public void PlayerJumpUp()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //if (isGrounded)
        //{
        //    PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsGround", true);
        //}
        //else
        //{
        //    PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsGround", false);
        //}

        if (isGrounded && !m_IsPressSpace)
        {
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsGround", true);
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsJump", false);
        }
        //if (m_IsPressSpace)
        //{
        //    return;
        //}else
        //{
        //    StartCoroutine(Delay());
        //}
        // Kiểm tra nhân vật có đứng trên mặt đất không
        //if (isGrounded && velocity.y < 0)
        //{
        //    PlayerManager.instance.playerAnim.GetAnimator().SetTrigger("Endouble Jump");

        //    if (jumpCount > 0) jumpCount = 0;
        //    velocity.y = -2f; // Giữ nhân vật sát mặt đất
        //}
        // Áp dụng trọng lực
        velocity.y += Physics.gravity.y * Time.deltaTime;

        // Di chuyển theo phương Y
        PlayerManager.instance.controller.Move(velocity * Time.deltaTime);

    }
    //IEnumerator Delay()
    //{
    //    yield return new WaitForSeconds(1f);

    //}
}





