using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BullTankMoveAgent", story: "[Bulltank] move [Player] with [AnimationName]", category: "Action", id: "1c0fd1b552615d965ac098f6c87fb191")]
public partial class BullTankMoveAgentAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Bulltank;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<BehaviorTreeSO> bullTankSo;
    [SerializeReference] public BlackboardVariable<string> AnimationName;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    protected override Status OnStart()
    {
        if (Bulltank.Value == null) return Status.Failure;
        if (Player.Value == null) return Status.Failure;
        if (bullTankSo.Value == null) return Status.Failure;
        if(AnimationName == null) return Status.Failure;
        animator = Bulltank.Value.GetComponent<Animator>();
        navMeshAgent = Bulltank.Value.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = bullTankSo.Value.SpeedRun;
        navMeshAgent.stoppingDistance = bullTankSo.Value.RangeAttackSword;
        animator.Play(AnimationName.Value);
        navMeshAgent.SetDestination(Player.Value.transform.position);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        navMeshAgent.SetDestination(Player.Value.transform.position);

        // Nếu agent vẫn đang tính đường, đợi
        if (navMeshAgent.pathPending)
            return Status.Running;

        // Nếu path đã stale hoặc agent rời khỏi NavMesh, thử tính lại
        if (navMeshAgent.isPathStale || !navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.ResetPath();
            navMeshAgent.SetDestination(Player.Value.transform.position);
            return Status.Running;
        }

        // Kiểm tra khoảng cách còn lại
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            return Status.Success;

        return Status.Running;
    }

    protected override void OnEnd()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.ResetPath();
    }
}

