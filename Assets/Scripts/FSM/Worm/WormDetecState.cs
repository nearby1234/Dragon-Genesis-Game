using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormDetecState : BaseState<WormBoss, WORMSTATE>
{
    private bool IsTaunting;
    private Coroutine waitChangeStateUnderground;
    public WormDetecState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm)
    {
    }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.DETEC);
        IsTaunting = false;
        waitChangeStateUnderground = boss.StartCoroutine(WaitChangeStateUnderground());
        
    }

    public override void Exit()
    {
        boss.ChangeBeforeState(WORMSTATE.DETEC);
        boss.StopCoroutine(waitChangeStateUnderground);
    }

    public override void Updates()
    {
        boss.Rotation();
        if (!IsTaunting)
        {
            float angle = Vector3.Angle(boss.transform.forward, boss.DistanceToPlayerNormalized());
            if(angle <5f)
            {
                boss.Animator.Play("Taunting");
                IsTaunting = true;
                waitChangeStateUnderground = boss.StartCoroutine(WaitChangeStateUnderground());
            }
        }
    }
    private IEnumerator WaitChangeStateUnderground()
    {
        yield return new WaitForSeconds(boss.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        boss.RequestStateTransition(WORMSTATE.UNDERGROUND);
    }
}
