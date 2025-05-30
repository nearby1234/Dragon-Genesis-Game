
using System.Diagnostics;

public class SuperIdleState : CompositeState<BullTankBoss>
{
    private bool _hasDetectedPlayer;
    public SuperIdleState(BullTankBoss stateMachine) : base(stateMachine)
    {
        var moveState = new MoveStateBT(stateMachine, this);
        var idleState = new IdleStateBT(stateMachine, this);
        var turnState = new TurnStateBT(stateMachine, this);
        var chaseState = new ChaseStateBT(stateMachine, this);

        AddTransition<MoveStateBT, TurnStateBT>(() => stateMachine.IsWithin(stateMachine.m_ZoneDetecPlayerDraw));
        turnState.OnStateComplete += state => ChangeChild<ChaseStateBT>();
        chaseState.OnStateComplete += state => RaiseComplete();

        AddChild(moveState);
        AddChild(idleState);
        AddChild(turnState);
        AddChild(chaseState);
        SetInitial<MoveStateBT>();
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSuperStateHSM(this);
        
    }
    public override void Update()
    {
        //if(stateMachine.IsWithin(stateMachine.m_ZoneDetecPlayerDraw) && CurrentChild is not TurnStateBT)
        //{
        //    ChangeChild<TurnStateBT>();
        //    return;
        //}    
        
        base.Update();// = currentchild?.update;
    }
    
}
