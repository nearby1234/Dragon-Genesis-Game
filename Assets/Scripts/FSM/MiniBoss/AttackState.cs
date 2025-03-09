using System.Collections;
using UnityEngine;
using static MiniBoss;

public class AttackState : BaseState
{
    public AttackState(MiniBoss miniBoss, FSM fsm) : base(miniBoss, fsm) { }

    public override void Enter()
    {
        miniBoss.ChangeStateCurrent(MiniBoss.ENEMYSTATE.ATTACK);
        miniBoss.NavmeshAgent.isStopped = true;
        //miniBoss.Rotation();
        miniBoss.StartCoroutine(miniBoss.PlaySingleAttackAnimation());
    }

    public override void Updates()
    {
        if (!miniBoss.PlayerInStopRange() )
        {
            miniBoss.StartCoroutine(miniBoss.WaitFinishAttack());
        }
        //if (miniBoss.m_IsMiniBossAttacked)
        //{
        //    // Chuyển về Idle sau khi attack xong
        //    miniBoss.RequestStateTransition(MiniBoss.ENEMYSTATE.IDLE);
        //    return;
        //}
        //if (!miniBoss.PlayerInStopRange() && miniBoss.currentState.Equals(MiniBoss.ENEMYSTATE.ATTACK))
        //{
        //    miniBoss.RequestStateTransition(MiniBoss.ENEMYSTATE.RUN);
        //}
    }

    public override void Exit()
    {
        miniBoss.beforState = ENEMYSTATE.ATTACK;
        miniBoss.StopCoroutine(miniBoss.WaitFinishAttack());
        miniBoss.NavmeshAgent.isStopped = false;
        miniBoss.Animator.CrossFade("Run", 0.2f);
       
        //// Reset trạng thái animation attack về trạng thái trung tính
        //miniBoss.Animator.CrossFade("IdleBattle", 0.2f);
        //miniBoss.m_IsMiniBossAttacked = false;
    }

  
}

