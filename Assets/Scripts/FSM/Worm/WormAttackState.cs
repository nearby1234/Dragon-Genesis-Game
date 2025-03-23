using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormAttackState : BaseState<WormBoss, WORMSTATE>
{
    private Coroutine waitForAttack;
    public WormAttackState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.ATTACK);
        List<WormAttackData> attackDataList = boss.IsRageState ? boss.wormAttackDatasPhase2 : boss.wormAttackDatasPhase1;
        string animName = attackDataList[boss.currentAttackIndex].animationName;
        boss.Animator.Play(animName, 0, 0f);
        waitForAttack = boss.StartCoroutine(WaitForAttack(attackDataList));
    }



    public override void Updates() { }

    public override void Exit()
    {
        boss.ChangeBeforeState(WORMSTATE.ATTACK);
        if (waitForAttack != null)
        {
            boss.StopCoroutine(waitForAttack);
            waitForAttack = null;
        }
    }

    private IEnumerator WaitForAttack(List<WormAttackData> attackDataList)
    {
        bool needFollowUp = attackDataList[boss.currentAttackIndex].needFollowUp;
        if (needFollowUp)
        {
            if (boss.IsRageState)
            {
                // Phase 2: chờ chuyển từ Attack05ST sang Attack05RPTSwing (với trigger "IsRage")
                yield return new WaitUntil(() =>
                {
                    var info = boss.Animator.GetCurrentAnimatorStateInfo(0);
                    return info.IsName("Attack05RPTSwing");
                });
                yield return new WaitUntil(() =>
                {
                    var info = boss.Animator.GetCurrentAnimatorStateInfo(0);
                    return (!info.IsName("Attack05RPTSwing")) || (info.IsName("Attack05RPTSwing") && info.normalizedTime >= 1f);
                });
            }
            else
            {
                // Phase 1: chờ chuyển từ Attack05ST sang Attack05RPT
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
        }
        else
        {
            yield return new WaitForSeconds(boss.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }

        boss.RequestStateTransition(WORMSTATE.IDLE);
    }
}