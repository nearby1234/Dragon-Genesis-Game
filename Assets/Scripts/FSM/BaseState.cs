using UnityEngine;

public abstract class BaseState
{
    protected MiniBoss miniBoss;
    protected FSM finiteSM;

    public BaseState( MiniBoss MiniBoss, FSM finiteSM )
    {
        this.miniBoss = MiniBoss;
        this.finiteSM = finiteSM;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Updates();
}
