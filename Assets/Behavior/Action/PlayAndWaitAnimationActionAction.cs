using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait Play Animation complete", story: "Wait Animation [StateName] complete", category: "Action", id: "f01bd120ee6d181d458c90e7b91c7988")]
public partial class PlayAndWaitAnimationActionAction : Action
{
    [SerializeReference] public BlackboardVariable<string> StateName;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    private Animator animator;
    private bool animationStarted;
    protected override Status OnStart()
    {
        if(Agent.Value == null) return Status.Failure;
        if(StateName.Value == null) return Status.Failure;
        animator = Agent.Value.GetComponent<Animator>();
        if (animator == null)
            return Status.Failure;
        animationStarted = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
       // Lấy thông tin state hiện tại
        var state = animator.GetCurrentAnimatorStateInfo(0);

        // Nếu còn đang trong transition, vẫn chờ
        if (animator.IsInTransition(0))
            return Status.Running;

        // Nếu chưa từng vào state ThrowAxe, check lần đầu
        if (!animationStarted)
        {
            if (state.IsName(StateName.Value))
            {
                // Đã thực sự vào animation
                animationStarted = true;
            }
            return Status.Running;
        }

        // Đã vào state, giờ check normalizedTime
        if (state.normalizedTime >= 1f)
            return Status.Success;

        return Status.Running;
    }

    protected override void OnEnd()
    {
        animationStarted = false; 
    }
}

