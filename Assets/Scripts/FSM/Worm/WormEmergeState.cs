using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class WormEmergeState : BaseState<WormBoss, WORMSTATE>
{
    private Coroutine waitPlayAnimationEmerge;
    public WormEmergeState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.EMERGE);
        // Lựa chọn danh sách attack dựa trên phase hiện tại
        List<WormAttackData> attackDataList = boss.IsRageState ? boss.wormAttackDatasPhase2 : boss.wormAttackDatasPhase1;
        boss.m_StopDistance = attackDataList[boss.currentAttackIndex].stopDistance;
        boss.Animator.SetTrigger("Emege");
        waitPlayAnimationEmerge = boss.StartCoroutine(WaitPlayAnimationEmerge());

    }
    public override void Updates()
    {
        if (boss.PlayerInRange())
        {
            boss.Rotation();
        }
    }
    public override void Exit()
    {
        boss.Animator.ResetTrigger("Emege");
        if (waitPlayAnimationEmerge != null)
        {
            boss.StopCoroutine(waitPlayAnimationEmerge);
            waitPlayAnimationEmerge = null;
        }
    }
    private IEnumerator WaitPlayAnimationEmerge()
    {
        yield return new WaitUntil(() =>
         boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundBreakThrough") &&
        boss.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        while (boss.PlayerInRange())
        {
            if (boss.PlayerInAttackRange())
            {
                boss.RequestStateTransition(WORMSTATE.ATTACK);
                yield break;
            }
            else
            {
                boss.RequestStateTransition(WORMSTATE.DETEC);
            }
            yield return new WaitForSeconds(0.2f);

        }
        boss.RequestStateTransition(WORMSTATE.IDLE);
    }
}
