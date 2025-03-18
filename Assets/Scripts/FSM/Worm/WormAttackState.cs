using UnityEngine;
using System.Collections;

public class WormAttackState : BaseState<WormBoss, WORMSTATE>
{
    private Coroutine waitForAttack;
    public WormAttackState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.ATTACK);
        boss.Animator.Play(boss.wormAttackDatasPhase1[boss.currentAttackIndex].animationName);
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
        bool needFollowUp = boss.wormAttackDatasPhase1[boss.currentAttackIndex].needFollowUp;
        if (needFollowUp)
        {
            yield return new WaitUntil(() =>
            {
                var info = boss.Animator.GetCurrentAnimatorStateInfo(0);
                return info.IsName("Attack05RPT");
            });

            yield return new WaitUntil(() =>
            {
                var info = boss.Animator.GetCurrentAnimatorStateInfo(0);
                return (!info.IsName("Attack05RPT")) || (info.IsName("Attack05RPT") && info.normalizedTime >= 1f);
            });
        }
        else
        {
            //yield return new WaitUntil(() =>
            //{
            //    var info = boss.Animator.GetCurrentAnimatorStateInfo(0);
            //    return (!info.IsName(boss.wormAttackDatasPhase1[boss.currentAttackIndex].animationName)) || (info.normalizedTime >= 1f);
            //});
            yield return new WaitForSeconds(boss.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
        
        boss.RequestStateTransition(WORMSTATE.IDLE);
    }
}
