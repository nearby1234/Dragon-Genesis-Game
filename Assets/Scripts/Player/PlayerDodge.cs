using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] private float dodgeDuration = 0.5f; // Th?i gian dodge t?i thi?u
    [SerializeField] private InputAction dodgeAction;
    private bool isDodging = false;

    private void Start()
    {
        dodgeAction.Enable();
        dodgeAction.performed += DodgeAction_performed;
    }

    private void OnDestroy()
    {
        dodgeAction.performed -= DodgeAction_performed;
        dodgeAction.Disable();
    }

    private void DodgeAction_performed(InputAction.CallbackContext obj)
    {
        // N?u ?ang không dodge, b?t ??u dodge
        if (!isDodging)
        {
            StartCoroutine(DodgeCoroutine());
        }
    }
    private IEnumerator DodgeCoroutine()
    {
        isDodging = true;
        // Set tr?ng thái dodge cho Animator ?? OnAnimatorMove nhân v?i dodgeMultiplier
        PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsDodge", true);

        // Ch? trong kho?ng th?i gian dodgeDuration (?i?u ch?nh theo dodge animation)
        yield return new WaitForSeconds(dodgeDuration);

        // K?t thúc dodge
        PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsDodge", false);
        isDodging = false;
    }
}
