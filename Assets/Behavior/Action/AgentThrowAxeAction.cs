using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AgentThrowAxe", story: "[Agent] throw axe [Name]", category: "Action", id: "23d2cf51731da5198b959d5d70c83f31")]
public partial class AgentThrowAxeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<String> Name;
    private Animator animator;
    private bool animationStarted;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;
        if (Name == null) return Status.Failure;
        animator = Agent.Value.GetComponent<Animator>();
        if (animator != null)
        {
            //animator.ResetTrigger("ThrowAxe");
            animator.SetTrigger("ThrowAxe");
            //animationStarted = false;
        }

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
            if (state.IsName(Name.Value))
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
        animator.ResetTrigger("ThrowAxe");
        animationStarted = false;

    }

}

