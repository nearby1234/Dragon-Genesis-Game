using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Play Particle System", story: "Play [ParticleSystem]", category: "Action", id: "edf15623a75a94e0879b50e3c1d88abb")]
public partial class PlayParticleSystemAction : Action
{
    [SerializeReference] public BlackboardVariable<ParticleSystem> ParticleSystem;

    protected override Status OnStart()
    {
        if(ParticleSystem.Value == null) return Status.Failure;
        ParticleSystem.Value.gameObject.SetActive(true);
        if(ParticleSystem.Value.isPlaying)
        {
            ParticleSystem.Value.Stop();
            ParticleSystem.Value.Play();
            Debug.Log($"Play {ParticleSystem.Value.name} axxe");
            return Status.Success;
        }    
        return Status.Running;
    }
  
}

