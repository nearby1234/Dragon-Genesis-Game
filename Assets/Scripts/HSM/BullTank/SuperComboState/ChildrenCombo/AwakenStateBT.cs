using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class AwakenStateBT : State<BullTankBoss>
{
    private SuperComboState parent;

    public AwakenStateBT(BullTankBoss stateMachine, SuperComboState parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSubStateHSM(this);
        stateMachine.EnableAgentPosition(true);
        stateMachine.EnableAgentRotation(true);
        stateMachine.Animator.SetTrigger("Angry");
    }
    public override void Exit()
    {
        base.Exit();
        stateMachine.Animator.ResetTrigger("Angry");
    }
    public override void OnAnimationComplete(NameState nameState)
    {
        base.OnAnimationComplete(nameState);
    }
}
