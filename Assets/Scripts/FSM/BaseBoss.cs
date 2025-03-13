using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using System;

public abstract class BaseBoss<T, TState> : MonoBehaviour
    where T : BaseBoss<T, TState>
    where TState : Enum
{
    [SerializeField] protected TState currentState;
    [SerializeField] protected TState beforeState;
    [SerializeField] protected FSM<T, TState> finiteSM;
    [SerializeField] protected NavMeshSurface m_NavmeshSurface;
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent m_NavmeshAgent;

    protected virtual void Start()
    {
        // Các lớp con có thể override nếu cần
    }

    protected virtual void Update()
    {
        finiteSM?.Update();
    }

    // Phương thức cập nhật trạng thái hiện tại (các state có thể gọi)
    public virtual void ChangeStateCurrent(TState newState)
    {
        currentState = newState;
    }

    // Phương thức chuyển state (các lớp con định nghĩa cụ thể)
    public virtual void RequestStateTransition(TState requestedState)
    {
        // Default implementation, có thể override ở lớp con
    }
}
