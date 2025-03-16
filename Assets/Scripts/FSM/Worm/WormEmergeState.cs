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
        //boss.Animator.Play(boss.emergeAnimation, 0, 0f);
        boss.Animator.SetTrigger("Emege");
        waitPlayAnimationEmerge = boss.StartCoroutine(WaitPlayAnimationEmerge());
       
    }

    private IEnumerator WaitForEmerge()
    {
        yield return new WaitForSeconds(1f); // gi? s? animation tr?i lên kéo dài 1s
        boss.RequestStateTransition(WORMSTATE.ATTACK);

    }

    private IEnumerator WaitPlayAnimationEmerge()
    {
        boss.m_StopDistance = boss.wormAttackDatas[boss.currentAttackIndex].stopDistance;
        yield return new WaitUntil(() =>
         boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("GroundBreakThrough") &&
        boss.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        while (boss.PlayerInRange())
        {
           
            if (boss.PlayerInAttackRange())
            {
                
                Debug.Log("aa");
                boss.RequestStateTransition(WORMSTATE.ATTACK);
                yield break;
            }
            yield return new WaitForSeconds(0.2f);

        }
        boss.RequestStateTransition(WORMSTATE.IDLE);

    }

    public override void Updates()
    {
        if(boss.PlayerInRange())
        {
            boss.Rotation();
        }    
    }

    public override void Exit()
    {
        boss.Animator.ResetTrigger("Emege");
        boss.StopCoroutine(waitPlayAnimationEmerge);
    }
}
