using System.Collections;
using UnityEngine;
using static MiniBoss;

public class IdleStandState : BaseState
{
    public IdleStandState(MiniBoss miniBoss, FSM finiteSM) : base(miniBoss, finiteSM) { }

    public override void Enter()
    {
        miniBoss.ChangeStateCurrent(ENEMYSTATE.IDLESTAND);

        // Reset các trạng thái chuyển động
        miniBoss.Animator.SetBool("IsRun", false);
        miniBoss.Animator.SetBool("IsMove", false);
        // Reset parameter lateral move
        miniBoss.Animator.SetInteger("Random", 0);
        // Dừng NavMeshAgent và reset đường đi
        miniBoss.NavmeshAgent.isStopped = true;
        miniBoss.NavmeshAgent.ResetPath();

        // Nếu bạn sử dụng transition dựa trên IsDetec, hãy đảm bảo set nó đúng (theo logic Animator của bạn)
        miniBoss.Animator.SetBool("IsDetec", true);
        miniBoss.Animator.CrossFade("IdleBattle",0.2f); // IdleBattle = IdleCombat clip của bạn

        miniBoss.StartCoroutine(DelayRequestState());
    }

    public override void Updates()
    {
        // Có thể cập nhật rotation nếu cần để boss luôn hướng về player
        miniBoss.Rotation();
    }

    public override void Exit()
    {
        miniBoss.Animator.SetBool("IsDetec", false);
    }

    private IEnumerator DelayRequestState()
    {
        yield return new WaitForSeconds(2f);
        miniBoss.m_IsMiniBossAttacked = false;
        miniBoss.RequestStateTransition(ENEMYSTATE.RUN);
    }
}
