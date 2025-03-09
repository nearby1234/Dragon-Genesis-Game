using System.Collections;
using UnityEngine;
using static MiniBoss;

public class IdleStandState : BaseState
{
    public IdleStandState(MiniBoss MiniBoss, FSM finiteSM) : base(MiniBoss, finiteSM)
    {
    }

    public override void Enter()
    {
        miniBoss.ChangeStateCurrent(ENEMYSTATE.IDLESTAND);
        miniBoss.Animator.SetBool("IsRun", false);
        miniBoss.Animator.SetBool("IsMove", false);
        miniBoss.Animator.Play("IdleBattle");
        miniBoss.StartCoroutine(DelayRequetsState());
    }

    public override void Updates()
    {

    }

    public override void Exit()
    {

    }
    private IEnumerator DelayRequetsState()
    {
        yield return new WaitForSeconds(4f);
        miniBoss.m_IsMiniBossAttacked = false;
        miniBoss.RequestStateTransition(ENEMYSTATE.RUN);
        Debug.Log("aaa");
    }
}
