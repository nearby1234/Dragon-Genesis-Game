using UnityEngine;

public abstract class BaseState
{
    protected MiniBoss miniBoss;
    protected FSM fSM;

    public BaseState( MiniBoss MiniBoss, FSM FSM )
    {
        this.miniBoss = MiniBoss;
        this.fSM = FSM;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Executed();
}
