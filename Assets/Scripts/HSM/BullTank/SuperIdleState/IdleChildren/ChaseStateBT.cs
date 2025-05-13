using System.Collections;
using UnityEngine;

public class ChaseStateBT : State<BullTankBoss>
{
    private SuperIdleState parent;
    private bool m_IsAttack1;
    private bool m_IsWaitAttackJump;
   
    
    public ChaseStateBT(BullTankBoss stateMachine, SuperIdleState parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSubStateHSM(this);
        Debug.Log("ChaseStateBT enter");
        stateMachine.Animator.SetTrigger("Charge");
        stateMachine.StartCoroutine(WaitAnimChargeFinish());
    }
    public override void Update() 
    {
        base.Update();
        //stateMachine.Rotation();
        float distance = Vector3.Distance(stateMachine.Player.transform.position, stateMachine.transform.position);
        if (distance <= stateMachine.BullTankAgent.stoppingDistance)
        {
            if(!m_IsAttack1)
            {
                stateMachine.BullTankAgent.ResetPath();
                stateMachine.Animator.SetTrigger("Attack2");
                m_IsAttack1 = true;
            }
            if(!m_IsWaitAttackJump)
            {
                stateMachine.StartCoroutine(WaitAnimAttackJumpFinish());
            }
        }
    }
    IEnumerator WaitAnimChargeFinish()
    {
        yield return new WaitUntil(() => stateMachine.Animator.GetCurrentAnimatorStateInfo(0).IsName("RunStart"));
        yield return new WaitUntil(() =>
        {
            float timeDuration = stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if(timeDuration >= 1)
            {
                return true;
            }
            return false;
        });

        stateMachine.BullTankAgent.SetDestination(stateMachine.Player.transform.position);
        stateMachine.BullTankAgent.stoppingDistance = stateMachine.m_DistanceAttackJump;
        stateMachine.BullTankAgent.speed = stateMachine.m_SpeedWalk;
        stateMachine.BullTankAgent.updateRotation = true;
    }
    IEnumerator WaitAnimAttackJumpFinish()
    {
        yield return new WaitUntil(()=> stateMachine.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack02_End"));
        yield return new WaitUntil(() =>
        {
            float timeDuration = stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (timeDuration >= 1)
            {
                return true;
            }
            return false;
        });
        stateMachine.NotifySuperIdleStateComplete();
        m_IsWaitAttackJump = true;
    }
}
