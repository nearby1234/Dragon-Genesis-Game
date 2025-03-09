using UnityEngine;

public class FSM
{
    private BaseState currentState;
    public void ChangeState(BaseState newstate)
    {
        currentState?.Exit();
        currentState = newstate;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Updates();
    }
}
