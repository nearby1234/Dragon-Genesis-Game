using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class WormEmergeState : BaseState<WormBoss, WORMSTATE>
{
    private Coroutine waitPlayAnimationEmerge;
    public WormEmergeState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.EMERGE);
        boss.Animator.SetTrigger("Emege");
        waitPlayAnimationEmerge = boss.StartCoroutine(WaitPlayAnimationEmerge());
        //if (!boss.PlayerInRange())
        //{
        //    boss.RequestStateTransition(WORMSTATE.UNDERGROUND);
        //}

        //// Ch?n m?t ki?u t?n công ng?u nhiên (0 ??n 4)
        //int attackIndex = Random.Range(0, boss.wormAttackDatas.Count);
        //boss.currentAttackIndex = attackIndex;
        //float requiredDistance = boss.wormAttackDatas[attackIndex].stopDistance;

        //Debug.Log(requiredDistance);
        //// L?y v? trí xu?t hi?n ng?u nhiên ??m b?o cách player ít nh?t requiredDistance
        ////Vector3 emergencePos = boss.GetRandomEmergencePosition();
        //// Di chuy?n ngay ??n v? trí ?ó (có th? dùng Warp ho?c SetDestination)
        ////boss.NavMeshAgent.Warp(emergencePos);

        //// Phát animation tr?i lên m?t ??t
        //boss.Animator.Play(boss.emergeAnimation);

        //boss.StartCoroutine(WaitForEmerge());
    }

    private IEnumerator WaitForEmerge()
    {
        yield return new WaitForSeconds(1f); // gi? s? animation tr?i lên kéo dài 1s
        boss.RequestStateTransition(WORMSTATE.ATTACK);
        
    }

    private IEnumerator WaitPlayAnimationEmerge()
    {
        yield return new WaitUntil(() =>
         boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundBreakThrough") &&
        boss.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        boss.RequestStateTransition(WORMSTATE.IDLE);
    }

    public override void Updates()
    {
        
    }

    public override void Exit()
    {
        boss.Animator.ResetTrigger("Emege");
        boss.StopCoroutine(waitPlayAnimationEmerge);
    }
}
