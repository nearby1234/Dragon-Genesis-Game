using UnityEngine;

public class SuperAttackStateBT : CompositeState<BullTankBoss>
{
    private bool notifyAttackComplete;
    private bool notifyAttackThrowComplete;
    public SuperAttackStateBT(BullTankBoss stateMachine) : base(stateMachine)
    {
        AddChild(new AttackStateBT(stateMachine,this));
        AddChild(new AttackThrowAxeBT(stateMachine,this));
        SetInitial<AttackStateBT>();

    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSuperStateHSM(this);
      
    }
    public override void Update()
    {
        if(notifyAttackComplete)
        {
            ChangeChild<AttackThrowAxeBT>();
            //return;
        }
        if(notifyAttackThrowComplete)
        {
            stateMachine.NotifySuperAttackStateComplete();
            return;
        }

       
        base.Update();
    }
    public void NotifyAttackStateComplete()
    {
        notifyAttackComplete = true;
    }
    public void NotifyAttackThrowStateComplete()
    {
        notifyAttackThrowComplete = true;
    }
}
