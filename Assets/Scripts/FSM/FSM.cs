using System;

public class FSM<T, TState>
    where T : BaseBoss<T, TState>
    where TState : Enum
{
    private BaseState<T, TState> currentState;

    public void ChangeState(BaseState<T, TState> newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Updates();
    }
}
