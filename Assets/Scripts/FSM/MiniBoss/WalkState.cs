using System.Collections;
using UnityEngine;
using static MiniBoss;

public class WalkState : BaseState
{
    public WalkState(MiniBoss miniBoss, FSM fSM) : base(miniBoss, fSM) { }

    public override void Enter()
    {
        miniBoss.ChangeStateCurrent(ENEMYSTATE.WALK);
        miniBoss.MoveToRandomPosition();
        miniBoss.Animator.SetBool("IsMove", true);
        
    }

    public override void Updates()
    {
        if (miniBoss.IsMoveWayPoint())
        {
            miniBoss.RequestStateTransition(ENEMYSTATE.IDLE);
        }
        else if (miniBoss.PlayerInRange())
        {
            miniBoss.RequestStateTransition(ENEMYSTATE.DETEC);
        }
    }

    public override void Exit()
    {
        miniBoss.Animator.SetBool("IsMove", false);
        miniBoss.beforState = ENEMYSTATE.WALK;
    }
}

