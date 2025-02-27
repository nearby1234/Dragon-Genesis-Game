using UnityEngine;
using static MiniBoss;

public class IdleState : BaseState
{
    public IdleState(MiniBoss MiniBoss, FSM FSM) : base(MiniBoss, FSM)
    {
    }

    public override void Enter()
    {
        Debug.Log("Enter Idle");
        miniBoss.state = ENEMYSTATE.IDLE;

    }

    public override void Executed()
    {
       if(miniBoss.PlayerInRange())
        {
            fSM.ChangeState(new WalkState(miniBoss,fSM));
        }    
    }

    public override void Exit()
    {
        Debug.Log("Exit Idle");
    }
}
