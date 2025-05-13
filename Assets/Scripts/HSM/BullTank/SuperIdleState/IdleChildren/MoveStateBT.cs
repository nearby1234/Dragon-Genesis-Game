using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MoveStateBT : State<BullTankBoss>
{
    private SuperIdleState parent;
    private bool m_HasDestination;

    public MoveStateBT(BullTankBoss stateMachine, SuperIdleState parent) : base(stateMachine)
    {
        this.parent = parent;

    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSubStateHSM(this);
        m_HasDestination = stateMachine.MoveToRandomPosition();
        stateMachine.Animator.SetBool("Walk", true);

    }
    public override void Update()
    {
        base.Update();
        if (!m_HasDestination)
        {
            m_HasDestination = stateMachine.MoveToRandomPosition();
            return;
        }
        if (stateMachine.IsMoveWayPoint())
        {
            Debug.Log($"stateMachine.IsMoveWayPoint : {stateMachine.IsMoveWayPoint()}");
            stateMachine.BullTankAgent.isStopped = true;
            stateMachine.BullTankAgent.ResetPath();
            parent.ChangeChild<IdleStateBT>();
        }
    }
    public override void Exit()
    {
        base.Exit();
        stateMachine.BullTankAgent.ResetPath();
        //stateMachine.Animator.SetBool("Walk", false);

    }
}
