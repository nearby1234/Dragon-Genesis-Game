using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormDetecState : BaseState<WormBoss, WORMSTATE>
{
    private bool IsTaunting;
    private Coroutine waitChangeStateUnderground;
    private Coroutine checkTauntingCoroutine;
    public WormDetecState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm)
    {
    }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.DETEC);
        IsTaunting = false;
        checkTauntingCoroutine = boss.StartCoroutine(CheckTaunting());
    }

    public override void Exit()
    {
        boss.ChangeBeforeState(WORMSTATE.DETEC);
        if (checkTauntingCoroutine != null)
        {
            boss.StopCoroutine(checkTauntingCoroutine);
            checkTauntingCoroutine = null;
        }
        if (waitChangeStateUnderground != null)
        {
            boss.StopCoroutine(waitChangeStateUnderground);
            waitChangeStateUnderground = null;
        }
    }
    public override void Updates()
    {
    }
    private IEnumerator CheckTaunting()
    {
        while (true)
        {
            // Xoay boss v? phía player m?i frame
            boss.Rotation();

            if (!IsTaunting)
            {
                float angle = Vector3.Angle(boss.transform.forward, boss.DistanceToPlayerNormalized());
                if (angle < 5f)
                {
                    boss.Animator.Play("Taunting");
                    IsTaunting = true;
                    waitChangeStateUnderground = boss.StartCoroutine(WaitChangeStateUnderground());
                }
            }
            yield return null; // Ch? 1 frame
        }
    }
    private IEnumerator WaitChangeStateUnderground()
    {
        yield return new WaitForSeconds(boss.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        Debug.Log("aaa");
        boss.RequestStateTransition(WORMSTATE.UNDERGROUND);
    }
}
