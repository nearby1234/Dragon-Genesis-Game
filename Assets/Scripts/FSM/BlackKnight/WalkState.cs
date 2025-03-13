using System.Collections;
using UnityEngine;

public class WalkState : BaseState<BlackKnightBoss, ENEMYSTATE>
{
    public WalkState(BlackKnightBoss miniBoss, FSM<BlackKnightBoss, ENEMYSTATE> fSM) : base(miniBoss, fSM) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(ENEMYSTATE.WALK);
        boss.MoveToRandomPosition();
        boss.Animator.SetBool("IsMove", true);
        
    }

    public override void Updates()
    {
        if (boss.IsMoveWayPoint())
        {
            Debug.Log(boss.IsMoveWayPoint());
            boss.RequestStateTransition(ENEMYSTATE.IDLE);
        }
        else if (boss.PlayerInRange())
        {
            boss.RequestStateTransition(ENEMYSTATE.DETEC);
        }
    }

    public override void Exit()
    {
        boss.Animator.SetBool("IsMove", false);
        boss.BeforState = ENEMYSTATE.WALK;
    }
}

