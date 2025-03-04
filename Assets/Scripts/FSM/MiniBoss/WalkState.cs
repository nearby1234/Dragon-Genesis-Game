using UnityEngine;
using static MiniBoss;

public class WalkState : BaseState
{
    public WalkState(MiniBoss MiniBoss, FSM FSM) : base(MiniBoss, FSM)
    {
    }

    public override void Enter()
    {
        Debug.Log($"Enter {GetType().Name}");
        miniBoss.ChangeStateCurrent(ENEMYSTATE.WALK);
        miniBoss.Animator.SetBool("IsMove", true);
        miniBoss.MoveToRandomPosition();
    }

    public override void Executed()
    {
        if(miniBoss.IsMoveTarget()) fSM.ChangeState(new IdleState(miniBoss, fSM));
        if (miniBoss.PlayerInRange(miniBoss.Range))
        {
            fSM.ChangeState(new DetecState(miniBoss,fSM));
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit Walk");
    }
}
