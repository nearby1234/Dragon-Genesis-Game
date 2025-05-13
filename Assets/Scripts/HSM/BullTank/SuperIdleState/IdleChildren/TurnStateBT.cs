using Unity.AI.Navigation.Samples;
using Unity.VisualScripting;
using UnityEngine;

public class TurnStateBT : State<BullTankBoss>
{
    private SuperIdleState parent;
    private readonly float angleThreshold = 2f;  // khi nào coi là đã xoay xong

    public TurnStateBT(BullTankBoss stateMachine, SuperIdleState parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter();
        Debug.Log($"TurnStateBT enter");
        stateMachine.SetSubStateHSM(this);
        var agent = stateMachine.BullTankAgent;
        agent.updateRotation = false;
        agent.isStopped = true;
    }
    public override void Update()
    {
        base.Update();
        if (stateMachine.Rotation() <= angleThreshold)
        {
            parent.ChangeChild<ChaseStateBT>();
        }
    }
    public override void Exit()
    {
        base.Exit();
        stateMachine.BullTankAgent.updatePosition = true;
        stateMachine.BullTankAgent.ResetPath();
        stateMachine.BullTankAgent.isStopped = false;
    }
}
