using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using Unity.Mathematics;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RotationFowardPlayer", story: "[Agent] Rotation Forward [Player] ", category: "Action", id: "fd9c8a5490849fc6a61c04e00be81970")]
public partial class RotationFowardPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    private NavMeshAgent agent;
    private bool m_IsRotate;
    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;
        if (Player.Value == null) return Status.Failure;

        agent = Agent.Value.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.ResetPath();
        }
        m_IsRotate = false;




        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!m_IsRotate)
        {
            if (Rotation())
            {
                Debug.Log($"Rotation() : {Rotation()}");
                m_IsRotate = true;
                return Status.Success;
            }
            else return Status.Running;
        }
        return Status.Success;
    }

    protected override void OnEnd()
    {
        // Chỉ ResetPath khi NavMeshAgent còn active và đang on NavMesh
        //if (agent.enabled && agent.isOnNavMesh && agent.hasPath)
        //    agent.ResetPath();

        //agent.updatePosition = true;

    }
    protected bool Rotation()
    {
        Vector3 dir = (Player.Value.transform.position - Agent.Value.transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude < 0.0001f)
            return true; // Hai đối tượng gần như chồng lên nhau

        dir.Normalize();

        float signedAngle = Vector3.SignedAngle(
        Agent.Value.transform.forward,   // hướng trước mặt hiện tại
        dir,                              // hướng mong muốn
        Vector3.up                        // quanh trục Y
        );
        // Lượng góc tối đa có thể quay trong khung hình này
        float maxTurn = agent.angularSpeed * Time.deltaTime;

        // Giới hạn góc quay sao cho không vượt quá maxTurn và luôn chọn chiều ngắn nhất
        float step = Mathf.Clamp(signedAngle, -maxTurn, maxTurn);

        // Thực hiện phép quay
        Agent.Value.transform.Rotate(0f, step, 0f, Space.World);

        // Nếu góc dư nhỏ hơn ngưỡng (2°), coi như đã quay xong
        return Mathf.Abs(signedAngle) <= 10f;
    }

}

