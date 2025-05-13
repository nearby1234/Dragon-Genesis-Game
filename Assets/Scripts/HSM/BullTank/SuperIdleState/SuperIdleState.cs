using Unity.VisualScripting;
using UnityEngine;

public class SuperIdleState : CompositeState<BullTankBoss>
{
    private bool _hasDetectedPlayer;
    public SuperIdleState(BullTankBoss stateMachine) : base(stateMachine)
    {
        AddChild(new MoveStateBT(stateMachine, this));
        AddChild(new IdleStateBT(stateMachine, this));
        AddChild(new TurnStateBT(stateMachine, this));
        AddChild(new ChaseStateBT(stateMachine, this));
        SetInitial<MoveStateBT>();
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSuperStateHSM(this);
        _hasDetectedPlayer = false;
        
    }
    public override void Update()
    {
        if (stateMachine.Distance() && !_hasDetectedPlayer)
        {
            stateMachine.BullTankAgent.isStopped = true;
            stateMachine.BullTankAgent.ResetPath();
            ChangeChild<TurnStateBT>();
            _hasDetectedPlayer = true;
            return;   // NGĂT, không gọi sub-state trong frame này
        }
        
        base.Update();// = currentchild?.update;
    }
}
