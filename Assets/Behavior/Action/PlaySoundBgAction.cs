using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Play Sound BG", story: "Play Sound [Name]", category: "Action", id: "935f4cecadd0a0256e274dbfa791edc7")]
public partial class PlaySoundBgAction : Action
{
    [SerializeReference] public BlackboardVariable<string> Name;

    protected override Status OnStart()
    {
        if (Name == null) return Status.Failure;
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlayBGM(Name.Value,true);
            return Status.Success;
        }
        return Status.Running;
    }


}

