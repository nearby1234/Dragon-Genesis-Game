using System.Collections;
using UnityEngine;
using static MiniBoss;

public class IdleState : BaseState
{
    private float idleTimer = 0f;       // Bộ đếm thời gian idle
    private float idleDuration = 3f;    // Thời gian boss đứng yên trước khi chuyển sang walk (có thể điều chỉnh)

    public IdleState(MiniBoss miniBoss, FSM fSM) : base(miniBoss, fSM)
    {
    }

    public override void Enter()
    {
        Debug.Log($"Enter {GetType().Name}");
        miniBoss.ChangeStateCurrent(ENEMYSTATE.IDLE);
        idleTimer = 0f; // Reset bộ đếm khi bước vào IdleState
    }

    public override void Executed()
    {
        // Đảm bảo animator không di chuyển
        miniBoss.Animator.SetBool("IsMove", false);

        // Cộng dồn thời gian đứng yên
        idleTimer += Time.deltaTime;

        if (miniBoss.PlayerInRange(miniBoss.Range))
        {
            fSM.ChangeState(new DetecState(miniBoss, fSM));
        }
        else 
        {
            if (idleTimer >= idleDuration) fSM.ChangeState(new WalkState(miniBoss, fSM));
        }
    }

    public override void Exit()
    {
        Debug.Log($"Exit {GetType().Name}");
    }
}
