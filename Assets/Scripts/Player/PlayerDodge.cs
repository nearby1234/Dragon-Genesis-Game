using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] private bool m_IsPressDodge;
    [SerializeField] private InputAction dodgeAction;

    private void Start()
    {
        dodgeAction.Enable();
        dodgeAction.performed += DodgeAction_performed; ;
        dodgeAction.canceled += DodgeAction_canceled;
    }

    private void OnDestroy()
    {
        dodgeAction.performed -= DodgeAction_performed;
        dodgeAction.canceled -= DodgeAction_canceled;
        dodgeAction.Disable();
    }
    private void DodgeAction_performed(InputAction.CallbackContext obj)
    {
        m_IsPressDodge = true;
        PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsDodge", true);
    }
    private void DodgeAction_canceled(InputAction.CallbackContext obj)
    {
        m_IsPressDodge = false;
        PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsDodge", false);
    }
}
