using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckDistancePlayer", story: "[Player] In Detection Range", category: "Action", id: "2488d942e004cabd703b6bfdda93ed27")]
public partial class CheckDistancePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<BehaviorTreeSO> BehaviorTreeSO;
    private float distance;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;
        if (Player.Value == null) return Status.Failure;
        if (BehaviorTreeSO.Value == null) return Status.Failure;
        distance = BehaviorTreeSO.Value.WanderRadius; ;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        bool isDistance = BehaviorUtils.DistanceToPlayer(Agent, Player, distance);
        if (!isDistance)
        {
            Debug.Log($"isDistance : {isDistance}");
            return Status.Failure;
        }
        else
        {
            Debug.Log($"isDistance : {isDistance}");
            return Status.Success;
        }
       
    }


}

