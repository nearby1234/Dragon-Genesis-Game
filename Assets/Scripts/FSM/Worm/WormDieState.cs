using UnityEngine;

public class WormDieState : BaseState<WormBoss, WORMSTATE>
{
    public WormDieState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm)
    {
    }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.DIE);
        boss.Animator.CrossFade("Die",0.1f);
    }

    public override void Exit()
    {
       boss.ChangeBeforeState(WORMSTATE.DIE);
    }

    public override void Updates()
    {
       
    }
}
