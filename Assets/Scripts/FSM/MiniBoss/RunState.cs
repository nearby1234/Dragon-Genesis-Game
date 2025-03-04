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
        miniBoss.NavmeshAgent.speed = miniBoss.RunSpeed;
        miniBoss.NavmeshAgent.stoppingDistance = miniBoss.StopDistance;
    }

    public override void Executed()
    {
        miniBoss.NavmeshAgent.SetDestination(miniBoss.Player.transform.position);
        if(miniBoss.MoveTarget())
        {
            Debug.Log(miniBoss.MoveTarget());
            fSM.ChangeState(new AttackState(miniBoss, fSM));
        }

    }

    public override void Exit()
    {
        Debug.Log($"Exit {GetType().Name}");
    }
}
