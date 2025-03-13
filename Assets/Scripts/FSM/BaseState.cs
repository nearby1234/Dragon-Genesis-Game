using System;

public abstract class BaseState<T, TState>
    where T : BaseBoss<T, TState>
    where TState : Enum
{
    protected T boss;
    protected FSM<T, TState> fsm;

    public BaseState(T boss, FSM<T, TState> fsm)
    {
        this.boss = boss;
        this.fsm = fsm;
    }

    public abstract void Enter();
    public abstract void Updates();
    public abstract void Exit();
}
