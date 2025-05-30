using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Always Success Node ", story: "Always Success Node", category: "Action/Test Flow", id: "cd2723b0eb9f3cc20e6f664ce3f3ca42")]
public partial class AlwaysSuccessNodeAction : Action
{

    protected override Status OnStart()
    {
        Debug.Log("Always Success Node");
        return Status.Success;
    }

   
}

