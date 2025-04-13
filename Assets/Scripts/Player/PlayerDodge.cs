using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] private float dodgeDuration = 0.5f; // Thời gian dodge tối thiểu
    [SerializeField] private InputAction dodgeAction;
    [SerializeField] private bool isDodging = false;
   
    private float dodgeTimer = 0f;

    private PlayerMove playerMove;
    private Animator playerAnimator;
    private void Start()
    {
        playerMove = PlayerManager.instance.playerMove;
        playerAnimator = PlayerManager.instance.playerAnim.GetAnimator();
        dodgeAction.Enable();
        dodgeAction.performed += OnDodgePerformed;
    }

    private void OnDestroy()
    {
        dodgeAction.performed -= OnDodgePerformed;
        dodgeAction.Disable();
    }
    private void Update()
    {
        if (isDodging)
        {
            HandleDodge();
        }
    }

    private void OnDodgePerformed(InputAction.CallbackContext context)
    {
        if (!isDodging)
        {
            StartDodge();
        }
    }
    private void StartDodge()
    {
        isDodging = true;
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_PRESS_V_DODGE, isDodging);
        }
        dodgeTimer = dodgeDuration;

        // Disable movement and set dodge state in Animator
        playerMove.canMove = false;
        playerAnimator.SetBool("IsDodge", true);
    }
    private void HandleDodge()
    {
        dodgeTimer -= Time.deltaTime;

        if (dodgeTimer <= 0f)
        {
            EndDodge();
        }
    }
    private void EndDodge()
    {
        isDodging = false;

        // Re-enable movement and reset dodge state in Animator
        playerMove.canMove = true;
        playerAnimator.SetBool("IsDodge", false);
    }

    //private IEnumerator DodgeCoroutine()
    //{
    //    isDodging = true;

    //    // Vô hiệu hóa di chuyển khi dodge bắt đầu
    //    playerMove.canMove = false;

    //    // Set trạng thái dodge cho Animator
    //    PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsDodge", true);

    //    // Chờ trong khoảng thời gian dodgeDuration
    //    yield return new WaitForSeconds(dodgeDuration);

    //    // Kết thúc dodge: reset trạng thái của Animator và bật lại di chuyển
    //    PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsDodge", false);
    //    playerMove.canMove = true;
    //    isDodging = false;
    //}
}
