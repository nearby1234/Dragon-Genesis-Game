using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static MiniBoss;

public class DetecState : BaseState
{
    private Coroutine moveSideCoroutine;

    public DetecState(MiniBoss miniBoss, FSM FSM) : base(miniBoss, FSM) { }

    public override void Enter()
    {
        miniBoss.ChangeStateCurrent(ENEMYSTATE.DETEC);
        miniBoss.Animator.SetBool("IsDetec", true);
        miniBoss.NavmeshAgent.isStopped = true;
        moveSideCoroutine = miniBoss.StartCoroutine(TimerExecuteBossMoveLeftOrRighto());
    }

    public override void Updates()
    {
        miniBoss.Rotation();
        if (miniBoss.PlayerInStopRange())
        {
            miniBoss.RequestStateTransition(ENEMYSTATE.ATTACK);
        }
        else if (miniBoss.IsMoveWayPoint())
        {
            miniBoss.RequestStateTransition(ENEMYSTATE.RUN);
        }

    }
    private IEnumerator TimerExecuteBossMoveLeftOrRighto()
    {
        yield return new WaitForSeconds(2f);
        miniBoss.NavmeshAgent.isStopped = false;
        miniBoss.NavmeshAgent.updateRotation = false;
        miniBoss.BossMoveLeftOrRighto();
    }

    public override void Exit()
    {
        miniBoss.Animator.SetBool("IsDetec", false);
        if (moveSideCoroutine != null)
        {
            miniBoss.StopCoroutine(moveSideCoroutine);
            moveSideCoroutine = null;
        }
        miniBoss.NavmeshAgent.updateRotation = true;
        miniBoss.beforState = ENEMYSTATE.DETEC;
    }
}

