using System.Collections;
using UnityEngine;
using static MiniBoss;

public class WalkState : BaseState
{
    public WalkState(MiniBoss miniBoss, FSM fSM) : base(miniBoss, fSM) { }

    public override void Enter()
    {
        Debug.Log("Enter WalkState");
        miniBoss.ChangeStateCurrent(ENEMYSTATE.WALK);
        miniBoss.Animator.SetBool("IsMove", true);
    }

    public override void Executed()
    {
        // Nếu đã đến đích và chưa bắt đầu coroutine chuyển state
        if (miniBoss.IsMoveWayPoint())
        {
            miniBoss.RequestStateTransition(ENEMYSTATE.IDLE);
        }
        else
        {
            if(miniBoss.PlayerInRange())
            {
                miniBoss.RequestStateTransition(ENEMYSTATE.DETEC);
            }
        }
    }
    public override void Exit()
    {
        Debug.Log("Exit WalkState");
    }
}
