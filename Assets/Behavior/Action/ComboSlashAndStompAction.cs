using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Agent Jump", story: "[Agent] Jump", category: "Action", id: "dd2ecca1d973f9a14f2006f34f07bb5d")]
public partial class ComboSlashAndStompAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    private Animator animator;

    protected override Status OnStart()
    {
        if(Agent.Value == null) return Status.Failure;
        animator = Agent.Value.GetComponent<Animator>();
        if(animator != null)
        {
            animator.SetTrigger("Jump");
        }    
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Jump") && info.normalizedTime >= 0.9f)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        animator.ResetTrigger("Jump");
    }
}

