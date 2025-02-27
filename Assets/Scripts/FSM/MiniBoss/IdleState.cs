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
        miniBoss.NavmeshAgent.isStopped = true;
        miniBoss.Animator.SetBool("IsMove", false);
    }

    public override void Executed()
    {
        if (miniBoss.PlayerInRange())
        {
            fSM.ChangeState(new WalkState(miniBoss, fSM));
        }
      
    }

    public override void Exit()
    {
        Debug.Log("Exit Idle");
    }
}
