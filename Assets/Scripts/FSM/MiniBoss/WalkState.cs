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
        miniBoss.state = ENEMYSTATE.WALK;
        miniBoss.EnemyMoveTarget();
        miniBoss.Animator.SetBool("IsMove", true);
        //miniBoss.NavmeshAgent.isStopped = false;
    }

    public override void Executed()
    {
        miniBoss.EnemyMoveTarget();
        //miniBoss.MoveToPlayer();
        //if (!miniBoss.PlayerInRange())
        //{
        //    this.fSM.ChangeState(new IdleState(miniBoss, fSM));
        //}
        //if (!miniBoss.NavmeshAgent.pathPending &&
        //     miniBoss.NavmeshAgent.remainingDistance <= miniBoss.NavmeshAgent.stoppingDistance)
        //{
        //    //this.fSM.ChangeState(new AttackState(miniBoss, fSM));
        //}

    }

    public override void Exit()
    {
        Debug.Log("Exit Walk");
    }
}
