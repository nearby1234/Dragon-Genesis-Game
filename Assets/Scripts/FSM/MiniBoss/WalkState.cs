using UnityEngine;
using static MiniBoss;

public class WalkState : BaseState
{
    public WalkState(MiniBoss MiniBoss, FSM FSM) : base(MiniBoss, FSM)
    {
    }

    public override void Enter()
    {
        miniBoss.state = ENEMYSTATE.WALK;
        miniBoss.Animator.Play("WalkFWD");
    }

    public override void Executed()
    {
        miniBoss.MoveToPlayer();
    }

    public override void Exit()
    {
        Debug.Log("Exit Walk");
    }
}
