using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Show Heal Bar", story: "Show Heal Bar", category: "Action", id: "b76f76c098f85527b27ad216916b05bc")]
public partial class ShowHealBarAction : Action
{

    protected override Status OnStart()
    {
        if(UIManager.HasInstance)
        {
            var data = new DataBullTank();
            UIManager.Instance.ShowScreen<ScreenHealBarBoss>(data,true);
            return Status.Success;
        }
        return Status.Running;
    }

   
}

