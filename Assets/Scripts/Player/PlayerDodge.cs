using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] private float dodgeDuration = 0.5f; // Thời gian dodge tối thiểu
    [SerializeField] private InputAction dodgeAction;
    private bool isDodging = false;

    private PlayerMove playerMove;

    private void Start()
    {
        playerMove = PlayerManager.instance.playerMove;
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
        if (!isDodging)
        {
            StartCoroutine(DodgeCoroutine());
        }
    }

    private IEnumerator DodgeCoroutine()
    {
        isDodging = true;

        // Vô hiệu hóa di chuyển khi dodge bắt đầu
        playerMove.canMove = false;

        // Set trạng thái dodge cho Animator
        PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsDodge", true);

        // Chờ trong khoảng thời gian dodgeDuration
        yield return new WaitForSeconds(dodgeDuration);

        // Kết thúc dodge: reset trạng thái của Animator và bật lại di chuyển
        PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsDodge", false);
        playerMove.canMove = true;
        isDodging = false;
    }
}
