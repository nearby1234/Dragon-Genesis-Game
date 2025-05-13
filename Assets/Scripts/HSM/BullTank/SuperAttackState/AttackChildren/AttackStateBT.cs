using System.Collections;
using UnityEngine;

public class AttackStateBT : State<BullTankBoss>
{
    private SuperAttackStateBT parent;
    private float m_angelThreshhold = 2f;
    private bool m_IsAttack;
    private bool m_IsRun;

    public AttackStateBT(BullTankBoss stateMachine, SuperAttackStateBT parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSubStateHSM(this);
        stateMachine.BullTankAgent.updateRotation = false;
        stateMachine.BullTankAgent.stoppingDistance = stateMachine.m_DistanceAttackSword;
        stateMachine.Animator.SetTrigger("WalkIdle");

    }
    public override void Update()
    {
        stateMachine.Rotation();
        if ((stateMachine.Rotation() > m_angelThreshhold)) return;
        if (stateMachine.DistanceWithPlayer() <= stateMachine.BullTankAgent.stoppingDistance)
        {
            if (!m_IsAttack)
            {
                stateMachine.BullTankAgent.isStopped = true;
                stateMachine.BullTankAgent.ResetPath();
                if (!m_IsRun)
                {
                    stateMachine.Animator.SetTrigger("AttackSword");
                }
                else
                {
                    stateMachine.Animator.SetBool("Run", false);
                }
                m_IsAttack = true;
                stateMachine.StartCoroutine(WaitAttackPlay());
            }
        }
        else
        {
            if (!m_IsRun)
            {
                stateMachine.BullTankAgent.SetDestination(stateMachine.Player.transform.position);
                stateMachine.BullTankAgent.updateRotation = true;
                stateMachine.BullTankAgent.updatePosition = true;
                stateMachine.Animator.SetBool("Run", true);
                if (stateMachine.DistanceWithPlayer() <= stateMachine.BullTankAgent.stoppingDistance)
                {
                    stateMachine.Animator.SetBool("Run", false);
                }
                m_IsRun = true;
            }
        }
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
        stateMachine.Animator.ResetTrigger("AttackSword");
        stateMachine.Animator.ResetTrigger("WalkIdle");
    }
    IEnumerator WaitAttackPlay()
    {
        yield return new WaitUntil(() => stateMachine.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_01"));
        yield return new WaitUntil(() =>
        {
            stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
            return (stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        });
        parent.NotifyAttackStateComplete();
    }
}
