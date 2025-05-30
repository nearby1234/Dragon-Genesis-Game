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
        stateMachine.BullTankAgent.SetDestination(stateMachine.Player.transform.position);
        stateMachine.EnableAgentPosition(false);

    }
    public override void Update()
    {
        base.Update();
        if (stateMachine.Rotation() <= angleThreshold)
        {
            stateMachine.Animator.SetTrigger("Charge");
        }
    }
    public override void Exit()
    {
        base.Exit();
        stateMachine.BullTankAgent.ResetPath();
        stateMachine.EnableAgentPosition(true);
        stateMachine.Animator.ResetTrigger("Charge");
    }
    public override void OnAnimationComplete(NameState namState)
    {
        base.OnAnimationComplete(namState);
        if (namState.Equals(NameState.RunStart))
        {
            RaiseComplete();
        }
    }
}
