using System.Collections;
using UnityEngine;
using static MiniBoss;

public class IdleState : BaseState
{
    public IdleState(MiniBoss miniBoss, FSM fSM) : base(miniBoss, fSM) { }

    public override void Enter()
    {
        Debug.Log("Enter IdleState");
        miniBoss.ChangeStateCurrent(ENEMYSTATE.IDLE);
        miniBoss.Animator.SetBool("IsMove", false);
        // Bắt đầu đếm thời gian idle 3 giây
        miniBoss.StartCoroutine(IdleDelay());
    }

    private IEnumerator IdleDelay()
    {
        yield return new WaitForSeconds(3f);
        // Sau khi chờ đủ, báo hiệu chuyển sang Walk
        miniBoss.RequestStateTransition(ENEMYSTATE.WALK);
    }

    public override void Executed() { }

    public override void Exit()
    {
        Debug.Log("Exit IdleState");
    }
}
