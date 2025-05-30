using System.Collections;
using UnityEngine;

public class ChaseStateBT : State<BullTankBoss>
{
    private SuperIdleState parent;
    private bool m_IsAttack1;

    public ChaseStateBT(BullTankBoss stateMachine, SuperIdleState parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSubStateHSM(this);
        Debug.Log("ChaseStateBT enter");
       
        stateMachine.BullTankAgent.stoppingDistance = stateMachine.m_DistanceAttackJump;
        stateMachine.BullTankAgent.SetDestination(stateMachine.Player.transform.position);
        stateMachine.BullTankAgent.speed = stateMachine.m_SpeedWalk;
    }
    public override void Update()
    {
        base.Update();
        if(stateMachine.HasStopDistance())
        {
            if (!m_IsAttack1)
            {
                
                stateMachine.Animator.SetTrigger("Attack2");
              
                m_IsAttack1 = true;
            }
           
        }
    }
    public override void Exit()
    {
        base.Exit();
        stateMachine.Animator.ResetTrigger("Attack2");
        stateMachine.BullTankAgent.ResetPath();

    }
    public override void OnAnimationComplete(NameState namState)
    {
        base.OnAnimationComplete(namState);
        if(namState.Equals(NameState.Attack2End))
        {
            RaiseComplete();
        }    
    }



}
