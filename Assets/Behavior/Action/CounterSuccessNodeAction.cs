using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Counter Success Node ", story: "Counter Success Node [Counter]", category: "Action/Test Flow", id: "eb03d9a50a09f2fc0e308a1a4509a746")]
public partial class CounterSuccessNodeAction : Action
{
    [SerializeReference] public BlackboardVariable<int> Counter;
    private int threshold;

    protected override Status OnStart()
    {
        if (Counter == null) return Status.Failure;
        else Debug.Log("có value");
        threshold = 0;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        threshold++;
        Debug.Log($"threshold : {threshold} - Counter {Counter.Value}");
        if(threshold >= Counter.Value)
        {
            return Status.Success;
        }    
        return Status.Running;
    }

    protected override void OnEnd()
    {
        threshold = 0;
    }
}

