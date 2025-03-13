using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DetecState : BaseState<BlackKnightBoss, ENEMYSTATE>
{
    private Coroutine moveSideCoroutine;

    public DetecState(BlackKnightBoss miniBoss, FSM<BlackKnightBoss, ENEMYSTATE> FSM) : base(miniBoss, FSM) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(ENEMYSTATE.DETEC);
        boss.Animator.SetBool("IsDetec", true);
        boss.NavmeshAgent.isStopped = true;
        moveSideCoroutine = boss.StartCoroutine(TimerExecuteBossMoveLeftOrRighto());
    }

    public override void Updates()
    {
        boss.Rotation();
        if (boss.PlayerInStopRange())
        {
            boss.RequestStateTransition(ENEMYSTATE.ATTACK);
        }
        else if (boss.IsMoveWayPoint())
        {
            boss.RequestStateTransition(ENEMYSTATE.RUN);
        }

    }
    private IEnumerator TimerExecuteBossMoveLeftOrRighto()
    {
        yield return new WaitForSeconds(2f);
        boss.NavmeshAgent.isStopped = false;
        boss.NavmeshAgent.updateRotation = false;
        boss.BossMoveLeftOrRighto();
    }

    public override void Exit()
    {
        boss.Animator.SetBool("IsDetec", false);
        if (moveSideCoroutine != null)
        {
            boss.StopCoroutine(moveSideCoroutine);
            moveSideCoroutine = null;
        }
        boss.NavmeshAgent.updateRotation = true;
        boss.BeforState = ENEMYSTATE.DETEC;
    }
}

