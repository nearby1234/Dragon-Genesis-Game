using System.Collections;
using UnityEngine;

public class IdleStandState : BaseState<BlackKnightBoss, ENEMYSTATE>
{
    public IdleStandState(BlackKnightBoss miniBoss, FSM<BlackKnightBoss, ENEMYSTATE> finiteSM) : base(miniBoss, finiteSM) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(ENEMYSTATE.IDLESTAND);

        // Reset các trạng thái chuyển động
        boss.Animator.SetBool("IsRun", false);
        boss.Animator.SetBool("IsMove", false);
        // Reset parameter lateral move
        boss.Animator.SetInteger("Random", 0);
        // Dừng NavMeshAgent và reset đường đi
        boss.NavmeshAgent.isStopped = true;
        boss.NavmeshAgent.ResetPath();

        // Nếu bạn sử dụng transition dựa trên IsDetec, hãy đảm bảo set nó đúng (theo logic Animator của bạn)
        boss.Animator.SetBool("IsDetec", true);
        boss.Animator.CrossFade("IdleBattle",0.2f); // IdleBattle = IdleCombat clip của bạn

        boss.StartCoroutine(DelayRequestState());
    }

    public override void Updates()
    {
        // Có thể cập nhật rotation nếu cần để boss luôn hướng về player
        boss.Rotation();
    }

    public override void Exit()
    {
        boss.Animator.SetBool("IsDetec", false);
    }

    private IEnumerator DelayRequestState()
    {
        yield return new WaitForSeconds(2f);
        boss.m_IsMiniBossAttacked = false;
        boss.RequestStateTransition(ENEMYSTATE.RUN);
    }
}
