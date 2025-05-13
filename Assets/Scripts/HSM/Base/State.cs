using UnityEditor;
using UnityEngine;

public abstract class State<Tmachine>
    where Tmachine : StateMachine<Tmachine> 
{
    protected Tmachine stateMachine;
    public State(Tmachine stateMachine) => this.stateMachine = stateMachine;
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
