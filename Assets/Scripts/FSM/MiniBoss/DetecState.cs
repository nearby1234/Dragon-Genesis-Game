using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static MiniBoss;

public class DetecState : BaseState
{
    private bool isCoroutineRunning = false;

    public DetecState(MiniBoss miniBoss, FSM FSM) : base(miniBoss, FSM) { }

    public override void Enter()
    {
        miniBoss.ChangeStateCurrent(ENEMYSTATE.DETEC);
        miniBoss.Animator.SetBool("IsDetec", true);
        miniBoss.NavmeshAgent.isStopped = true;
        miniBoss.StartCoroutine(TimerExecuteBossMoveLeftOrRighto());
        // Reset lateral choice flag khi vào DETEC

        //miniBoss.ResetLateralChoice();
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




        //if (miniBoss.PlayerInRange())
        //{
        //    if (!isCoroutineRunning)
        //    {
        //        isCoroutineRunning = true;
        //        miniBoss.StartCoroutine(DelayAndResetCoroutine());
        //    }
        //    if (miniBoss.IsMoveWayPoint())
        //    {
        //        //miniBoss.RequestStateTransition(ENEMYSTATE.RUN);
        //    }
        //}
        //if (miniBoss.PlayerInStopRange())
        //{
        //    miniBoss.RequestStateTransition(ENEMYSTATE.ATTACK);
        //}
    }

    // Coroutine bọc lại để reset flag sau khi chạy xong
    //private IEnumerator DelayAndResetCoroutine()
    //{
    //    yield return miniBoss.DelayBossMoveLeftOrRighto();
    //    isCoroutineRunning = false;
    //}
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
        //miniBoss.NavmeshAgent.isStopped = false;
        isCoroutineRunning = false;
        miniBoss.NavmeshAgent.updateRotation = true;
        miniBoss.beforState = ENEMYSTATE.DETEC;
    }
}

