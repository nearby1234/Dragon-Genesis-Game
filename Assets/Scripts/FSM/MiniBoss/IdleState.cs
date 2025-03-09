using System.Collections;
using UnityEngine;
using static MiniBoss;

public class IdleState : BaseState
{
    private Coroutine idleCoroutine;

    public IdleState(MiniBoss miniBoss, FSM fSM) : base(miniBoss, fSM) { }

    public override void Enter()
    {
        miniBoss.ChangeStateCurrent(ENEMYSTATE.IDLE);
        //miniBoss.Animator.SetBool("IsMove", false);
        idleCoroutine = miniBoss.StartCoroutine(IdleDelay());
    }

    public override void Updates() 
    {
        if(miniBoss.PlayerInRange())
        {
            miniBoss.RequestStateTransition(ENEMYSTATE.DETEC);
        }
    }

    public override void Exit()
    {
        if (idleCoroutine != null)
            miniBoss.StopCoroutine(idleCoroutine);
        miniBoss.beforState = ENEMYSTATE.IDLE;
    }

    private IEnumerator IdleDelay()
    {
        yield return new WaitForSeconds(2f);
        // Kiểm tra lại state trước khi chuyển
        if (miniBoss.currentState == ENEMYSTATE.IDLE)
        {
            miniBoss.RequestStateTransition(ENEMYSTATE.WALK);
        }
    }
}
