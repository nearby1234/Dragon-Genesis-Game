using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Randommove", story: "[Agent] move random", category: "Action", id: "a92f92cee71790ec04d6d1a40b787444")]
public partial class RandommoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    //[SerializeReference] public BlackboardVariable<float> WanderRadius;
    //[SerializeReference] public BlackboardVariable<float> SpeedAgent;
    
    [SerializeReference] public BlackboardVariable<BehaviorTreeSO> MovementSetting;

    private Animator animator;
    private NavMeshAgent _nav;
    private Vector3 _targetPos;
    private bool _hasTarget;
    protected override Status OnStart()
    {
        if (Agent?.Value == null)
        {
            LogFailure("Agent is not set.", true);
            return Status.Failure;
        }

        animator = Agent.Value.GetComponent<Animator>();
        if (animator == null)
        {
            LogFailure("Animator component not found on Agent.", true);
            return Status.Failure;
        }
        

        _nav = Agent.Value.GetComponent<NavMeshAgent>();
        if (_nav == null)
        {
            LogFailure("NavMeshAgent component not found on Agent.", true);
            return Status.Failure;
        }
      

        // Kiểm tra BlackboardVariables


        // Gán speed và animation
        _nav.speed = MovementSetting.Value.SpeedAgent;
        animator.SetBool("Walk", true);

        _hasTarget = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!_hasTarget)
        {
            Vector3 randomDir = UnityEngine.Random.insideUnitSphere * MovementSetting.Value.WanderRadius;
            randomDir.y = 0f;
            Vector3 randomPoint = Agent.Value.transform.position + randomDir;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, MovementSetting.Value.WanderRadius, NavMesh.AllAreas))
            {
                _targetPos = hit.position;

                _nav.SetDestination(_targetPos);
                _hasTarget = true;
            }
            // Nếu không tìm được vị trí khả dụng, thử lại lần sau
            return Status.Running;

        }
        if (_nav.remainingDistance <= _nav.stoppingDistance)
        {
            //_isWaiting = true;
            animator.SetBool("Walk", false);
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        _hasTarget = false;
        //_waitElapsed = 0f;
        if (_nav != null
        && _nav.enabled
        && _nav.isOnNavMesh)
        {
            _nav.ResetPath();
        }
    }
}

