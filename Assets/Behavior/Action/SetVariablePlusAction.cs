using System;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Variable Plus", story: "Set [Valiable] Plus", category: "Action", id: "a06cfecc05de5ac9bec7810dc8b2c3cb")]
public partial class SetVariablePlusAction : Action
{
    [SerializeReference] public BlackboardVariable<int> Valiable;

    protected override Status OnStart()
    {
        if (Valiable == null)
            return Status.Failure;

        Valiable.Value++;
        Debug.Log($"Valiable.Value : {Valiable.Value}");
        return Status.Success;
    }

     
   
}

