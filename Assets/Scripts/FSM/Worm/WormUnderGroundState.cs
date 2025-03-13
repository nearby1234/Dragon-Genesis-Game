using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class WormUndergroundState : BaseState<WormBoss, WORMSTATE>
{
    private float timer;
    private Coroutine waitPlayAnimationUnderground;
    private Coroutine waitUnderground;


    public WormUndergroundState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.UNDERGROUND);
        boss.Animator.SetTrigger("Underground");
        waitUnderground = boss.StartCoroutine(WaitUnderground());
    }

    public override void Updates()
    {
        //timer += Time.deltaTime;
        //if (timer > 2f)
        //{
        //    Debug.Log(timer);
        //    boss.MoveToRandomPosition();
        //    timer = 0;
        //}
    }

    public override void Exit()
    {
        boss.ChangeBeforeState(WORMSTATE.EMERGE);
        boss.StopCoroutine(waitUnderground);
        boss.Animator.ResetTrigger("Underground");
    }

    private IEnumerator WaitUnderground()
    {
        yield return WaitPlayAnimationUnderground();
        yield return new WaitForSeconds(1f);
        boss.RequestStateTransition(WORMSTATE.EMERGE);
    }
    private IEnumerator WaitPlayAnimationUnderground()
    {
        yield return new WaitUntil(() =>
         boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundDiveIn") &&
        boss.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        boss.MoveToRandomPosition();
    }

}
