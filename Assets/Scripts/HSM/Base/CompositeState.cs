using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeState<Tmachine> : State<Tmachine>
    where Tmachine : StateMachine<Tmachine>
{
    private State<Tmachine> currentChild;
    private Dictionary<Type, State<Tmachine>> children = new();
    public CompositeState(Tmachine stateMachine) : base(stateMachine) { }
    public void AddChild(State<Tmachine> child)
    {
        children[child.GetType()] = child;
    }
    
    public void SetInitial<T>() where T : State<Tmachine>
    {
        currentChild = children[typeof(T)];
    }
    public override void Enter()
    {
        base.Enter();
        currentChild?.Enter();
    }
    public override void Update()
    {
       currentChild?.Update();
    }
    public override void Exit()
    {
        base.Exit();
        currentChild?.Exit();
    }
    public void ChangeChild<T>() where T : State<Tmachine>
    {
        currentChild?.Exit(); 
        currentChild = children[typeof(T)];
        currentChild.Enter();
    }    
}
