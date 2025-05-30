using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChaseAction", story: "[Agent] chase and attack jump [Player]", category: "Action", id: "78392dd2cd61b8b0c0abea6e37f5b202")]
public partial class ChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<BehaviorTreeSO> BehaviorTreeSO;
    private NavMeshAgent _nav;
    private Animator animator;
    private int stringtoHash = Animator.StringToHash("RunStart");
    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;
        if (Player.Value == null) return Status.Failure;
        if (BehaviorTreeSO.Value == null) return Status.Failure;


        _nav = Agent.Value.GetComponent<NavMeshAgent>();
        if (_nav != null)
        {
            _nav.stoppingDistance = BehaviorTreeSO.Value.RangeAttackJump;
            _nav.speed = BehaviorTreeSO.Value.SpeedRun;
            _nav.updatePosition = false;
            _nav.isStopped = true;
            _nav.SetDestination(Player.Value.transform.position);
        }
        animator = Agent.Value.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("ChargeStart");
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        _nav.SetDestination(Player.Value.transform.position);
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if(info.shortNameHash == stringtoHash && info.normalizedTime >=0.9f)
        {
            Debug.Log("da het animation RunStart");
            _nav.isStopped = false;
            _nav.updatePosition = true;
        }    
       
        // 1) N?u path ch?a s?n sàng ? ti?p t?c ch?y
        if (_nav.pathPending || !_nav.hasPath)
            return Status.Running;

        // 2) Ki?m tra agent có trên NavMesh không
        if (!_nav.isOnNavMesh)
        {
            Debug.LogError("Agent is not on a NavMesh!");
            return Status.Failure;
        }

        // 3) Bây gi? newRemainingDistance m?i ph?n ánh ?úng
        float remDist = _nav.remainingDistance;
        if (remDist <= _nav.stoppingDistance)
        {
            Debug.Log($"remainingDistance: {remDist}, stoppingDistance: {_nav.stoppingDistance}");
            animator.SetTrigger("JumpEnd");
            return Status.Success;
        }
        else
            return Status.Running;
    }


    protected override void OnEnd()
    {
        // Ch? ResetPath khi NavMeshAgent còn active và ?ang on NavMesh
        if (_nav.enabled && _nav.isOnNavMesh && _nav.hasPath)
            _nav.ResetPath();
    }
}

