using System.Collections;
using UnityEngine;

public class WormIdleState : BaseState<WormBoss,WORMSTATE>
{
    private Coroutine timerChangeUnderGroundState;
    public WormIdleState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.IDLE);
        boss.Animator.Play("IdleNormal");
        timerChangeUnderGroundState = boss.StartCoroutine(TimerChangeUndergroundState());

    }

    public override void Updates()
    {
        //if (!boss.PlayerInRange())
        //{
        //    boss.RequestStateTransition(WORMSTATE.UNDERGROUND);
        //}
    }

    public override void Exit()
    {
       boss.ChangeBeforeState(WORMSTATE.IDLE);
        boss.StopCoroutine(timerChangeUnderGroundState);
    }

    IEnumerator TimerChangeUndergroundState()
    {
        yield return new WaitForSeconds(2f);
        boss.RequestStateTransition(WORMSTATE.UNDERGROUND);
    }
}
