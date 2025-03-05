using System.Threading;
using UnityEngine;
using static MiniBoss;

public class RunState : BaseState
{
    public RunState(MiniBoss MiniBoss, FSM FSM) : base(MiniBoss, FSM)
    {
    }

    public override void Enter()
    {
        Debug.Log($"Enter {GetType().Name}");
        miniBoss.ChangeStateCurrent(ENEMYSTATE.RUN);
        miniBoss.Animator.SetBool("IsRun", true);
        miniBoss.Animator.Play("Run");
        miniBoss.NavmeshAgent.speed = miniBoss.RunSpeed;
        miniBoss.NavmeshAgent.stoppingDistance = miniBoss.StopDistance;
    }

    public override void Executed()
    {
        miniBoss.MoveToPlayer();
        if(miniBoss.PlayerInStopRange())
        {
            miniBoss.RequestStateTransition(ENEMYSTATE.ATTACK);
        }

    }

    public override void Exit()
    {
        Debug.Log($"Exit {GetType().Name}");
    }
}
