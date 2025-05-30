using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AgentAttackSword", story: "[Agent] attack Sword", category: "Action", id: "f8b902a29b1dc0d26ba829192d13db9c")]
public partial class AgentAttackSwordAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    private Animator animator;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;
        animator = Agent.Value.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("AttackSword");
        }
        else
        {
            Debug.Log("không có animator");
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("AttackSword") && info.normalizedTime >= 1f) return Status.Success;

        return Status.Running;
    }

    protected override void OnEnd()
    {
        animator.ResetTrigger("AttackSword");
        animator.ResetTrigger("Run");
    }
}

