using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "testEvent", story: "Send event", category: "Action", id: "9f5de87815c90c5f1be3b3544ca3b37f")]
public partial class TestAction : EventAction
{
    [SerializeReference]
    public BlackboardVariable[] MessageVariables = new BlackboardVariable[4];

    protected override Status OnStart()
    {
        if (!EventChannel)
        {
            LogFailure("No EventChannel assigned.");
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        EventChannel.SendEventMessage(MessageVariables);
        return Status.Success;
    }

}

