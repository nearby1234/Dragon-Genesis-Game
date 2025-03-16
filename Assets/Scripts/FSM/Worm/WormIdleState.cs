using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WormIdleState : BaseState<WormBoss,WORMSTATE>
{
    private Coroutine timerChangeState;
    public WormIdleState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.IDLE);
        boss.Animator.Play("IdleNormal");
        timerChangeState = boss.StartCoroutine(TimerChangeState());

    }

    public override void Updates()
    {
        //if (boss.PlayerInRange())
        //{
        //    boss.RequestStateTransition(WORMSTATE.DETEC);
        //}
    }

    public override void Exit()
    {
       boss.ChangeBeforeState(WORMSTATE.IDLE);
        boss.StopCoroutine(timerChangeState);
    }

    //IEnumerator TimerChangeUndergroundState()
    //{
    //    yield return new WaitForSeconds(2f);
    //    boss.RequestStateTransition(WORMSTATE.UNDERGROUND);
    //}

    //IEnumerator TimerChangeDetecState()
    //{
    //    yield return new WaitForSeconds(2f);
    //    boss.RequestStateTransition(WORMSTATE.DETEC);
    //}

    IEnumerator TimerChangeState()
    {
        while (boss.PlayerInRange())
        {
            yield return new WaitForSeconds(3f);
            boss.RequestStateTransition(WORMSTATE.DETEC);
            yield break;
        }
        yield return new WaitForSeconds(2f);
        boss.RequestStateTransition(WORMSTATE.UNDERGROUND);
    }
}
