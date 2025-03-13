using System.Collections;
using System.Threading;
using UnityEngine;

public class RunState : BaseState<BlackKnightBoss, ENEMYSTATE>
{
    public RunState(BlackKnightBoss miniBoss, FSM<BlackKnightBoss, ENEMYSTATE> FSM) : base(miniBoss, FSM) { }

    public override void Enter()
    {
        boss.ChangeStateCurrent(ENEMYSTATE.RUN);
        boss.Animator.SetBool("IsRun",true);
        boss.Animator.CrossFade("Run",0.2f);
        boss.NavmeshAgent.speed = boss.RunSpeed;
        boss.NavmeshAgent.stoppingDistance = boss.StopDistance;
        boss.NavmeshAgent.isStopped = false;



    }
    public override void Updates()
    {
        SmoothDestination();

        // Kiểm tra khoảng cách: nếu player trong khoảng dừng thì khóa hướng và chuyển sang attack
        if (boss.PlayerInStopRange())
        {
            boss.SetDestinationToPlayerPosition(true);  // khóa hướng
            boss.RequestStateTransition(ENEMYSTATE.ATTACK);
        }
        else
        {
            // Nếu không khóa hướng, bạn vẫn có thể làm mượt hoặc cập nhật destination như trên
            //miniBoss.SetDestinationToPlayerPosition(false); // có thể loại bỏ gọi này vì đã update ở trên
            boss.Rotation();
        }
    }


    public override void Exit()
    {
        boss.BeforState = ENEMYSTATE.RUN;
        boss.Animator.SetBool("IsRun", false);

    }
    private void SmoothDestination()
    {
        Vector3 targetPos = boss.m_Player.transform.position;
        Vector3 currentDest = boss.NavmeshAgent.destination;
        float threshold = 0.5f; // Ngưỡng cập nhật, có thể điều chỉnh

        if (Vector3.Distance(currentDest, targetPos) > threshold)
        {
            // Làm mượt destination
            Vector3 smoothDest = Vector3.Lerp(currentDest, targetPos, Time.deltaTime * 3f);
            boss.NavmeshAgent.SetDestination(smoothDest);
        }
    }
}

