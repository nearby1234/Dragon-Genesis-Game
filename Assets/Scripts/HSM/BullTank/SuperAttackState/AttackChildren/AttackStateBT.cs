using System.Collections;
using UnityEngine;

public class AttackStateBT : State<BullTankBoss>
{
    private SuperAttackStateBT parent;
    private bool m_IsAttack;

    public AttackStateBT(BullTankBoss stateMachine, SuperAttackStateBT parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSubStateHSM(this);
        stateMachine.EnableAgentRotation(false);
        //stateMachine.BullTankAgent.SetDestination(stateMachine.Player.transform.position);
        stateMachine.BullTankAgent.stoppingDistance = stateMachine.m_DistanceAttackSword;
        //stateMachine.Animator.SetTrigger("RunAttack");

    }
    public override void Update()
    {
        if (m_IsAttack) return;
        if (stateMachine.GetDistanceToPlayer()<= stateMachine.m_DistanceAttackSword)
        {
            stateMachine.Rotation();
            stateMachine.Animator.SetTrigger("WalkRotate");
            if (stateMachine.RotateForwardPlayer())
            {
                stateMachine.Animator.SetTrigger("AttackSword");
            }
        }
        else
        {
            stateMachine.EnableAgentRotation(true);
            stateMachine.Animator.SetTrigger("RunAttack");
            stateMachine.BullTankAgent.SetDestination(stateMachine.Player.transform.position);
            if(stateMachine.HasStopDistance())
            {
                stateMachine.Animator.SetTrigger("AttackSword");
            }    
        }

        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
        stateMachine.Animator.ResetTrigger("AttackSword");
        stateMachine.Animator.ResetTrigger("RunAttack");
        stateMachine.Animator.ResetTrigger("WalkRotate");
        stateMachine.BullTankAgent.ResetPath();
    }
    public override void OnAnimationComplete(NameState nameState)
    {
        base.OnAnimationComplete(nameState);
        if (nameState.Equals(NameState.Attack01Axe))
        {
            RaiseComplete();
        }
    }
}
