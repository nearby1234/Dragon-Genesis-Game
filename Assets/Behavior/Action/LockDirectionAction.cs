using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LockDirection", story: "[Agent] lock Direction", category: "Action", id: "8270242739e6d4279106b8f697ebd69f")]
public partial class LockDirectionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    private Vector3 lockDirection;

    protected override Status OnStart()
    {
        if(Agent.Value == null) return Status.Failure;
        lockDirection = Agent.Value.transform.forward;
        Agent.Value.transform.rotation = Quaternion.LookRotation(lockDirection);

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

