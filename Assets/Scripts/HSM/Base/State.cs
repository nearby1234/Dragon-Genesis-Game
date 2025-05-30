using System;
using UnityEditor;
using UnityEngine;

public abstract class State<Tmachine>
    where Tmachine : StateMachine<Tmachine> 
{
    public event Action<State<Tmachine>> OnStateComplete; // Action truy?n vào là 1 state<Tmachine>
    protected Tmachine stateMachine;
    public State(Tmachine stateMachine) => this.stateMachine = stateMachine;

    protected  void RaiseComplete()
    {
        OnStateComplete?.Invoke(this);
    }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
    public virtual void OnAnimationComplete(NameState nameState) { }
}
