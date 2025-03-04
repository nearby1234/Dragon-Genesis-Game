using UnityEngine;
using UnityEngine.AI;
using static MiniBoss;

public class DetecState : BaseState
{
    private float idleTimer = 0f;       // Bộ đếm thời gian idle
    private float idleDuration = 2f;    // Thời gian boss đứng yên trước khi chuyển sang walk (có thể điều chỉnh)
    private float lateralDistance = 5f;
    private bool hasRandomized = false;
    public DetecState(MiniBoss MiniBoss, FSM FSM) : base(MiniBoss, FSM)
    {
    }

    public override void Enter()
    {
        Debug.Log($"Enter {GetType().Name}");
        miniBoss.ChangeStateCurrent(ENEMYSTATE.DETEC);
        miniBoss.Animator.SetBool("IsDetec", true);
        miniBoss.NavmeshAgent.isStopped = true;
        idleTimer = 0f;
    }

    public override void Executed()
    {
        BossMoveLeftOrRight();
    }

    public override void Exit()
    {
        Debug.Log($"Exit {GetType().Name}");
    }

    private void BossMoveLeftOrRight()
    {
        // 1. Tính hướng từ boss đến player
        Vector3 directionToPlayer = miniBoss.Distance();
        // 2. Loại bỏ thành phần y để xoay chỉ theo mặt phẳng ngang
        directionToPlayer.y = 0f;

        // 3. Tính toán hướng xoay mục tiêu
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        float rotationSpeed = 120f; // Tốc độ xoay, bạn có thể điều chỉnh
        miniBoss.transform.rotation = Quaternion.Slerp(miniBoss.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (miniBoss.PlayerInRange(miniBoss.NavmeshAgent.stoppingDistance))
        {
            fSM.ChangeState(new AttackState(miniBoss, fSM));
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration && !hasRandomized)
            {
                
                hasRandomized = true;
                int randomSide = Random.Range(1, 3);
                miniBoss.Animator.SetInteger("Random", 1);
                Vector3 lateralDirection = (randomSide == 1) ? miniBoss.transform.right : -miniBoss.transform.right;
                Vector3 targetPos = miniBoss.transform.position + lateralDirection * lateralDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(targetPos, out hit, 5f, NavMesh.AllAreas))
                {
                    miniBoss.NavmeshAgent.isStopped = false;
                    miniBoss.NavmeshAgent.stoppingDistance = 0;
                    miniBoss.NavmeshAgent.SetDestination(hit.position);
                }
            }
        }
        if (miniBoss.MoveTarget())
        {
            fSM.ChangeState(new RunState(miniBoss, fSM));
        }
    }
}
