using UnityEngine;
using UnityEngine.AI;
using static MiniBoss;

public class DetecState : BaseState
{
    public DetecState(MiniBoss MiniBoss, FSM FSM) : base(MiniBoss, FSM)
    {
    }

    public override void Enter()
    {
        Debug.Log($"Enter {GetType().Name}");
        miniBoss.ChangeStateCurrent(ENEMYSTATE.DETEC);
        miniBoss.Animator.SetBool("IsDetec", true);
        miniBoss.NavmeshAgent.isStopped = true;
        //idleTimer = 0f;
    }

    public override void Executed()
    {
        miniBoss.Rotation();
        miniBoss.BossMoveLeftOrRight();
    }

    public override void Exit()
    {
        Debug.Log($"Exit {GetType().Name}");
    }

    
}
