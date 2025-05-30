using UnityEngine;

public abstract class StateMachine<Tmachine> : MonoBehaviour
    where Tmachine : StateMachine<Tmachine>
{
    private State<Tmachine> currentState;
    public State<Tmachine> CurrentState => currentState;
    public void SetState(State<Tmachine> newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    protected virtual void Update()
    {
        currentState?.Update();
    }
}
