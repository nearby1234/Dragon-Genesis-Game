using UnityEngine;
using static MiniBoss;

public class IdleState : BaseState
{
    public IdleState(MiniBoss MiniBoss, FSM FSM) : base(MiniBoss, FSM)
    {
    }

    public override void Enter()
    {
        Debug.Log($"Enter {GetType().Name}");
        miniBoss.state = ENEMYSTATE.IDLE;
        miniBoss.GetRandomPointInVolume();
    }

    public override void Executed()
    {
        fSM.ChangeState(new WalkState(miniBoss, fSM));

        //if (miniBoss.PlayerInRange())
        //{
        //    fSM.ChangeState(new WalkState(miniBoss, fSM));
        //}

    }

    public override void Exit()
    {
        Debug.Log($"Exit {GetType().Name}");
    }
}
