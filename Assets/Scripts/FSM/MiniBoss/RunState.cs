using System.Threading;
using UnityEngine;
using static MiniBoss;

public class RunState : BaseState
{
    public RunState(MiniBoss miniBoss, FSM FSM) : base(miniBoss, FSM) { }

    public override void Enter()
    {
        miniBoss.ChangeStateCurrent(ENEMYSTATE.RUN);
        miniBoss.Animator.SetTrigger("IsRun");
        //miniBoss.Animator.Play("run");
        miniBoss.NavmeshAgent.speed = miniBoss.RunSpeed;
        miniBoss.NavmeshAgent.stoppingDistance = miniBoss.StopDistance;
        //miniBoss.NavmeshAgent.isStopped = false;

        
    }

    public override void Updates()
    {
        // Kiểm tra khoảng cách: nếu player trong khoảng dừng thì khóa hướng, ngược lại chỉ cập nhật destination
        if (miniBoss.PlayerInStopRange())
        {
            miniBoss.SetDestinationToPlayerPosition(true);  // khóa hướng
            miniBoss.RequestStateTransition(ENEMYSTATE.ATTACK);

        }
        else
        {
            miniBoss.SetDestinationToPlayerPosition(false); // không khóa hướng
        }
    }

    public override void Exit()
    {
        miniBoss.beforState = ENEMYSTATE.RUN;
    }
}

