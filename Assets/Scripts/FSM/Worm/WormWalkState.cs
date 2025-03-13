using UnityEngine;

public class WormWalkState : BaseState<WormBoss, WORMSTATE>
{
    public WormWalkState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm)
    {
    }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.WALK);
        boss.GetRandomEmergencePosition();
    }

    public override void Exit()
    {
        boss.ChangeBeforeState(WORMSTATE.WALK);
    }

    public override void Updates()
    {
        
    }
}
