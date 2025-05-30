using UnityEngine;

public class SuperAttackStateBT : CompositeState<BullTankBoss>
{
    public SuperAttackStateBT(BullTankBoss stateMachine) : base(stateMachine)
    {
        var AttackState = new AttackStateBT(stateMachine, this);
        var AttackThrowAxe = new AttackThrowAxeBT(stateMachine, this);

        AttackState.OnStateComplete += state => ChangeChild<AttackThrowAxeBT>();
        AttackThrowAxe.OnStateComplete += state => RaiseComplete();
        AddChild(AttackState);
        AddChild(AttackThrowAxe);
        SetInitial<AttackStateBT>();

    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSuperStateHSM(this);
      
    }
    public override void Update()
    {
        base.Update();
    }
}
