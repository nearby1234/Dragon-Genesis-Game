using UnityEngine;
using System.Collections;

public class WormAttackState : BaseState<WormBoss, WORMSTATE>
{
    private Coroutine waitForAttack;
    public WormAttackState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.ATTACK);
        boss.Animator.Play(boss.wormAttackDatas[boss.currentAttackIndex].animationName);
        waitForAttack = boss.StartCoroutine(WaitForAttack());
    }

    

    public override void Updates() { }

    public override void Exit()
    {
        boss.ChangeBeforeState(WORMSTATE.ATTACK);
        boss.StopCoroutine(waitForAttack);
    }

    private IEnumerator WaitForAttack()
    {
        yield return new WaitForSeconds(boss.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        boss.RequestStateTransition(WORMSTATE.IDLE);
    }
}
