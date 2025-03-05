using System.Collections;
using System.Threading;
using UnityEngine;
using static MiniBoss;

public class AttackState : BaseState
{
    private float m_Timer;
    private float m_AttackTime = 3f;
    public AttackState(MiniBoss MiniBoss, FSM FSM) : base(MiniBoss, FSM)
    {
    }

    public override void Enter()
    {
        Debug.Log($"Enter {GetType().Name}");
        miniBoss.state = ENEMYSTATE.ATTACK;

        //// Lưu hướng hiện tại của Boss khi bắt đầu tấn công
        //lockedRotation = miniBoss.transform.rotation;
        miniBoss.NavmeshAgent.isStopped = true;
        //// Chạy animation Attack
        //miniBoss.Animator.Play("Attack02");
        miniBoss.Attack();

    }

    public override void Executed()
    {
        miniBoss.Rotation();
        //m_Timer += Time.deltaTime;
        //if(m_Timer >= m_AttackTime)
        //{
           
        //    m_Timer = 0;
        //}
        //Debug.Log(m_Timer);
       //miniBoss.Rotation();

        //// Giữ nguyên hướng trong suốt quá trình Attack
        //miniBoss.transform.rotation = lockedRotation;

        //if (miniBoss.Distance() > miniBoss.NavmeshAgent.stoppingDistance)
        //{
        //    Debug.Log(miniBoss.Distance());
        //    if (idleTimer >= idleDuration)
        //    {
        //        fSM.ChangeState(new RunState(miniBoss, fSM));
        //    }
        //}
        //else if (miniBoss.Distance() <= miniBoss.NavmeshAgent.stoppingDistance)
        //{
        //    miniBoss.Animator.Play("Attack02");
        //}
    }

    public override void Exit()
    {
        //miniBoss.Animator.SetBool("IsRun", false);

        //// Cho phép Boss quay theo Player lại sau Attack
        //miniBoss.Rotation();
    }
}
