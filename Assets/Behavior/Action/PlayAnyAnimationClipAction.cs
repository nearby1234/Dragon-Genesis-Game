using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Play Any Animation Clip", story: "[Agent] play animation [Name]", category: "Action", id: "5d3ffa85c6ceb8aa9cb36e681c515fbc")]
public partial class PlayAnyAnimationClipAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<string> Name;
    private Animator animator;
    private int hashName;

    protected override Status OnStart()
    {
        if(Agent.Value == null) return Status.Failure;
        if(Name == null) return Status.Failure;
        animator=Agent.Value.GetComponent<Animator>();
        if(animator !=null )
        {
            hashName = Animator.StringToHash(Name.Value);
            animator.CrossFade(hashName, 0.2f);
            Debug.Log($"hashName : {hashName}");
        }    
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

