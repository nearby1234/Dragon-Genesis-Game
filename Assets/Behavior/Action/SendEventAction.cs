using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Send Event", story: "Send Event With [ListenType] [Value]", category: "Action", id: "cc12b433ce70799d17e3e16b98daf222")]
public partial class SendEventAction : Action
{
    [SerializeReference] public BlackboardVariable Value;
    [SerializeReference] public BlackboardVariable<ListenType> ListenType;

    protected override Status OnStart()
    {
        if (Value == null) return Status.Failure;
        if (ListenType == null) return Status.Failure;
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.Value, Value.ObjectValue);
            Debug.Log($"Value.ObjectValue {Value.ObjectValue}");
            Debug.Log($"ListenType : {ListenType.Value}");
            return Status.Success;
        }    
        return Status.Running;
    }

   
}

