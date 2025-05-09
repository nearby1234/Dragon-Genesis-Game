using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private InputAction m_ButtonAttackLeftMouse;
    [SerializeField] private InputAction m_ButtonAttackRightMouse;
    //[SerializeField] private bool m_IsPressLeftMouse;
    //[SerializeField] private bool m_IsPressRightMouse;
    [SerializeField] private string[] m_AttackNameAnim; // Danh sách tên animation
    [SerializeField] private int[] m_AttackAnimStringToHash; // Mã hash animation
    [SerializeField] private int m_AttackAnimIndex = 0; // Chỉ số animation hiện tại
    [SerializeField] private int m_PlayerDamage;
    [SerializeField] private int m_PlayerDamageBase; // Giá trị damage cơ bản
    [SerializeField] private int m_PlusDamageValue;
    public int PlusDamageValue => m_PlusDamageValue; // Giá trị damage cộng thêm

    private Animator playerAnimator; // Animator của player

    void Start()
    {
        playerAnimator = PlayerManager.instance.playerAnim.GetAnimator();
        m_PlayerDamageBase = PlayerManager.instance.PlayerStatSO.m_PlayerDamage;
        m_PlayerDamage = m_PlayerDamageBase; // Khởi tạo player damage với giá trị cơ bản

        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_DAMAGE_VALUE, m_PlayerDamage);
        }

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

        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_SEND_POINT, ReceiverPoint);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_SEND_POINT, ReceiverPoint);
        }
        DegreeEventClickMouse();
    }
    private void OnCancelAttackRightMouse(InputAction.CallbackContext context)
    {
        //m_IsPressRightMouse = false;
    }
    private void OnPerformedAttackRightMouse(InputAction.CallbackContext context)
    {
        //m_IsPressRightMouse = true;


        // Nếu animation Heavy Attack chưa đang chạy thì play nó
        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Heavy Attack"))
        {
            playerAnimator.Play("Heavy Attack");
        }
        PlayerManager.instance.ChangeStatePlayer(PlayerManager.PlayerState.attack);
    }
    private void OnPerformedAttackLeftMouse(InputAction.CallbackContext context)
    {
        //m_IsPressLeftMouse = true;

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
        //m_IsPressLeftMouse = false;
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
    public void RegisterEventAttack()
    {
        m_ButtonAttackLeftMouse.performed += OnPerformedAttackLeftMouse;
        m_ButtonAttackLeftMouse.canceled += OnCancelAttackLeftMouse;
        m_ButtonAttackLeftMouse.Enable();
        m_ButtonAttackRightMouse.performed += OnPerformedAttackRightMouse;
        m_ButtonAttackRightMouse.canceled += OnCancelAttackRightMouse;
        m_ButtonAttackRightMouse.Enable();
    }
    public int GetPlayerDamage() => m_PlayerDamage;
    private void ReceiverPoint(object value)
    {
        if (value is StatPointUpdateData data && data.StatName == "StrengthStatsTxt")
        {
            m_PlayerDamage = m_PlayerDamageBase + (data.Point * m_PlusDamageValue);

            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.PLAYER_SEND_DAMAGE_VALUE, m_PlayerDamage);
                Debug.Log($"m_PlayerDamage" + m_PlayerDamage);
            }
        }
    }
    public void SwordFlashSound()
    {
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("SwordFlashSound");
        }
    }
}
