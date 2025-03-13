using System.Collections;
using UnityEngine;

public class IdleState : BaseState<BlackKnightBoss, ENEMYSTATE>
{
    private Coroutine idleCoroutine;

    public IdleState(BlackKnightBoss miniBoss, FSM<BlackKnightBoss, ENEMYSTATE> fSM) : base(miniBoss, fSM)
    {
        
    }

    public override void Enter()
    {
        boss.ChangeStateCurrent(ENEMYSTATE.IDLE);
        //miniBoss.Animator.SetBool("IsMove", false);
        idleCoroutine = boss.StartCoroutine(IdleDelay());
    }

    public override void Updates() 
    {
        if(boss.PlayerInRange())
        {
            boss.RequestStateTransition(ENEMYSTATE.DETEC);
        }
    }

    public override void Exit()
    {
        if (idleCoroutine != null)
            boss.StopCoroutine(idleCoroutine);
        boss.BeforState = ENEMYSTATE.IDLE;
    }

    private IEnumerator IdleDelay()
    {
        yield return new WaitForSeconds(2f);
        // Kiểm tra lại state trước khi chuyển
        if (boss.CurrentState == ENEMYSTATE.IDLE)
        {
            boss.RequestStateTransition(ENEMYSTATE.WALK);
        }
    }
}
