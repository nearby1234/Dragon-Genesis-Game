using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CoolDownAttack", story: "[Agent] cooldown [duration] second after attack", category: "Action", id: "15be8c20dfd012de0d509845b9738fc4")]
public partial class CoolDownAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Duration;
    [CreateProperty] private Animator animator;
    [CreateProperty] private float m_Timer;
    protected override Status OnStart()
    {
        if(Agent.Value == null) return Status.Failure;
        if(Duration == null) return Status.Failure;
        animator = Agent.Value.GetComponent<Animator>();
        animator.SetTrigger("Idle");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        AnimatorStateInfo clipInfo = animator.GetCurrentAnimatorStateInfo(0);
        m_Timer += Time.deltaTime;
        if (m_Timer >= Duration.Value)
        {
            if(clipInfo.IsName("Idle") && clipInfo.normalizedTime >0.9f)
            {
                return Status.Success;
            }    
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        m_Timer =0f;
        animator.ResetTrigger("Idle");
    }
}

