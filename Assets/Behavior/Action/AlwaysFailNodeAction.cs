using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Always Fail Node ", story: "Always Fail Node", category: "Action/Test Flow", id: "c32483a0d6430de12cd35f55c1152df0")]
public partial class AlwaysFailNodeAction : Action
{

    protected override Status OnStart()
    {
        Debug.Log("Always Fail Node");
        return Status.Failure;
    }

   
}

