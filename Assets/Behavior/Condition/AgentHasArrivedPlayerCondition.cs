using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "AgentHasArrivedPlayer", story: "[Agent] Has Stopped [Player] With Range [Range]", category: "Conditions", id: "5a4ba885a6f7d0ebda21e3053596415b")]
public partial class AgentHasArrivedPlayerCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> Range;
    private NavMeshAgent navMeshAgent;

    public override bool IsTrue()
    {
        if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) return true;
        
        return false;
    }

    public override void OnStart()
    {
        if (Agent.Value == null) return;
        if(Player.Value == null) return;    
        navMeshAgent = Agent.Value.GetComponent<NavMeshAgent>();
        if(navMeshAgent == null) return;
        navMeshAgent.stoppingDistance = Range.Value;
    }

    public override void OnEnd()
    {
    }
}
