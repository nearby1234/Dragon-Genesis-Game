using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private InputAction m_ButtonAttackLeftMouse;
    [SerializeField] private InputAction m_ButtonAttackRightMouse;
    [SerializeField] private bool m_IsPressLeftMouse;
    [SerializeField] private bool m_IsPressRightMouse;
    [SerializeField] private string[] m_AttackNameAnim; // Danh sách tên animation
    [SerializeField] private int[] m_AttackAnimStringToHash; // Mã hash animation
    [SerializeField] private int m_AttackAnimIndex = 0; // Chỉ số animation hiện tại
    [SerializeField] private int m_PlayerDamage;

    private Animator playerAnimator; // Animator của player
    private PlayerMove playerMove;   // Tham chiếu đến script PlayerMove

    void Start()
    {
        playerAnimator = PlayerManager.instance.playerAnim.GetAnimator();
        playerMove = GetComponent<PlayerMove>();

        // Khởi tạo danh sách animation hash
        m_AttackAnimStringToHash = new int[m_AttackNameAnim.Length];
        for (int i = 0; i < m_AttackNameAnim.Length; i++)
        {
            m_AttackAnimStringToHash[i] = Animator.StringToHash(m_AttackNameAnim[i]);
        }

        m_ButtonAttackLeftMouse.Enable();
        m_ButtonAttackLeftMouse.performed += OnPerformedAttackLeftMouse;
        m_ButtonAttackLeftMouse.canceled += OnCancelAttackLeftMouse;

        m_ButtonAttackRightMouse.Enable();
        m_ButtonAttackRightMouse.performed += OnPerformedAttackRightMouse;
        m_ButtonAttackRightMouse.canceled += OnCancelAttackRightMouse;
    }
    private void OnCancelAttackRightMouse(InputAction.CallbackContext context)
    {
        m_IsPressRightMouse = false;
    }
    private void OnPerformedAttackRightMouse(InputAction.CallbackContext context)
    {
        m_IsPressRightMouse = true;

        // Vô hiệu hóa di chuyển khi bắt đầu attack
        //playerMove.canMove = false;

        // Nếu animation Heavy Attack chưa đang chạy thì play nó
        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Heavy Attack"))
        {
            playerAnimator.Play("Heavy Attack");
        }
        PlayerManager.instance.ChangeStatePlayer(PlayerManager.PlayerState.attack);
    }
    private void OnPerformedAttackLeftMouse(InputAction.CallbackContext context)
    {
        m_IsPressLeftMouse = true;

        // Nếu animation attack hiện tại vẫn chưa hoàn thành, bỏ qua việc kích hoạt lại attack
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            return;
        }
        // Play attack animation theo index hiện tại
        playerAnimator.Play(m_AttackAnimStringToHash[m_AttackAnimIndex]);
        PlayerManager.instance.ChangeStatePlayer(PlayerManager.PlayerState.attack);

        // Tăng index để chuyển sang animation tiếp theo, nếu có
        m_AttackAnimIndex++;
        if (m_AttackAnimIndex >= m_AttackAnimStringToHash.Length)
        {
            m_AttackAnimIndex = 0;
        }
    }
    private void OnCancelAttackLeftMouse(InputAction.CallbackContext context)
    {
        m_IsPressLeftMouse = false;
    }
    public void DegreeEventClickMouse()
    {
        m_ButtonAttackLeftMouse.performed -= OnPerformedAttackLeftMouse;
        m_ButtonAttackLeftMouse.canceled -= OnCancelAttackLeftMouse;
        m_ButtonAttackLeftMouse.Disable();
        m_ButtonAttackRightMouse.performed -= OnPerformedAttackRightMouse;
        m_ButtonAttackRightMouse.canceled -= OnCancelAttackRightMouse;
        m_ButtonAttackRightMouse.Disable();
    }
    public int GetPlayerDamage() => m_PlayerDamage;
}
