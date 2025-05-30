using System;
using System.Collections.Generic;

public abstract class CompositeState<Tmachine> : State<Tmachine>
    where Tmachine : StateMachine<Tmachine>
{
    private State<Tmachine> currentChild;
    public State<Tmachine> CurrentChild => currentChild;
    private readonly Dictionary<Type, State<Tmachine>> children = new();
    private readonly List<(Type sourceType, Func<bool> guard, Action transitionAction)> transitionList = new();

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
        foreach (var (sourceType, guard, transitionAction) in transitionList)
        {
            // Nếu currentChild đúng sourceType và guard() trả true
            if (currentChild.GetType() == sourceType && guard())
            {
                transitionAction();  // gọi ChangeChild<TTo>()
                return;              // dừng update thêm
            }
        }

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

    protected void AddTransition<TForm, TTo>(Func<bool> guard)
        where TForm : State<Tmachine>
        where TTo : State<Tmachine>
    {
        transitionList.Add(
            (
               sourceType: typeof(TForm),
               guard: guard,
               transitionAction: () => ChangeChild<TTo>()
            ));

    }
    public override void OnAnimationComplete(NameState namState)
    {
        base.OnAnimationComplete(namState);
        currentChild?.OnAnimationComplete(namState);
    }



}
