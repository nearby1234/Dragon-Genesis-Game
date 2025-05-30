using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AgentResetPath", story: "[Agent] reset path", category: "Action/Navigation", id: "06b83183c8edeaaf2465bb385c630d30")]
public partial class AgentResetPathAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    private NavMeshAgent navMeshAgent;

    protected override Status OnStart()
    {
        if(Agent.Value == null) return Status.Failure;
        navMeshAgent = Agent.Value.GetComponent<NavMeshAgent>();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(navMeshAgent.hasPath)
        {
            navMeshAgent.ResetPath();
            navMeshAgent.isStopped = true;
        }    
        return Status.Success;
    }

    protected override void OnEnd()
    {
        navMeshAgent.isStopped = false;
    }
}

