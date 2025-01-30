using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private InputAction m_ButtonAttackLeftMouse;
    [SerializeField] private InputAction m_ButtonAttackRightMouse;
    [SerializeField] private bool m_IsPressLeftMouse;
    [SerializeField] private bool m_IsPressRightMouse;
    [SerializeField] private string[] m_AttackNameAnim;
    [SerializeField] private int[] m_AttackAnimStringToHash;
    [SerializeField] private int m_AttackAnimIndex;
    [SerializeField] private int m_PlayerDamage;
    void Start()
    {
        m_AttackAnimStringToHash = new int[m_AttackNameAnim.Length];
        m_ButtonAttackLeftMouse.Enable();
        m_ButtonAttackLeftMouse.performed += OnPerformedAttackLeftMouse;
        m_ButtonAttackLeftMouse.canceled += OnCancelAttackLeftMouse;
        m_ButtonAttackRightMouse.Enable();
        m_ButtonAttackRightMouse.performed += OnPerformedAttackRightMouse;
        m_ButtonAttackRightMouse.canceled += OnCancelAttackRightMouse;

    }
    private void OnDisable()
    {
        m_ButtonAttackLeftMouse.performed -= OnPerformedAttackLeftMouse;
        m_ButtonAttackLeftMouse.canceled -= OnCancelAttackLeftMouse;
        m_ButtonAttackLeftMouse.Disable();
        m_ButtonAttackRightMouse.performed -= OnPerformedAttackRightMouse;
        m_ButtonAttackRightMouse.canceled -= OnCancelAttackRightMouse;
        m_ButtonAttackRightMouse.Disable();
    }
    private void OnCancelAttackRightMouse(InputAction.CallbackContext context)
    {

    }    

    private void OnPerformedAttackRightMouse(InputAction.CallbackContext context)
    {
        m_IsPressRightMouse = true;
        PlayerManager.instance.playerAnim.GetAnimator().Play("Heavy Attack");
        PlayerManager.instance.ChangeStatePlayer(PlayerManager.PlayerState.attack);
    }    

    private void OnPerformedAttackLeftMouse(InputAction.CallbackContext context)
    {
        m_IsPressLeftMouse = true;
        if(m_IsPressLeftMouse && m_AttackAnimIndex < m_AttackAnimStringToHash.Length)
        {
            PlayerManager.instance.playerAnim.GetAnimator().Play(m_AttackAnimStringToHash[m_AttackAnimIndex]);
            PlayerManager.instance.ChangeStatePlayer(PlayerManager.PlayerState.attack);
            m_AttackAnimIndex++;
        }
    }
    private void OnCancelAttackLeftMouse(InputAction.CallbackContext context)
    {
        m_IsPressLeftMouse = false;
    }
    public void Attack()
    {
        for (int i = 0; i < m_AttackNameAnim.Length; i++)
        {
            if (m_AttackAnimStringToHash != null)
            {
                m_AttackAnimStringToHash[i] = Animator.StringToHash(m_AttackNameAnim[i]);
            }
        }
        if (m_AttackAnimIndex == m_AttackAnimStringToHash.Length)
        {
            m_AttackAnimIndex = 0;
        }
    }
    public int GetPlayerDamage() => m_PlayerDamage;
       
}
