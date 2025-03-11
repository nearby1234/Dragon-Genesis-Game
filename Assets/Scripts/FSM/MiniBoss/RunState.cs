using System.Collections;
using System.Threading;
using UnityEngine;
using static MiniBoss;

public class RunState : BaseState
{
    public RunState(MiniBoss miniBoss, FSM FSM) : base(miniBoss, FSM) { }

    public override void Enter()
    {
        miniBoss.ChangeStateCurrent(ENEMYSTATE.RUN);
        miniBoss.Animator.SetBool("IsRun",true);
        miniBoss.Animator.CrossFade("Run",0.2f);
        miniBoss.NavmeshAgent.speed = miniBoss.RunSpeed;
        miniBoss.NavmeshAgent.stoppingDistance = miniBoss.StopDistance;
        miniBoss.NavmeshAgent.isStopped = false;



    }
    public override void Updates()
    {
        SmoothDestination();

        // Kiểm tra khoảng cách: nếu player trong khoảng dừng thì khóa hướng và chuyển sang attack
        if (miniBoss.PlayerInStopRange())
        {
            miniBoss.SetDestinationToPlayerPosition(true);  // khóa hướng
            miniBoss.RequestStateTransition(ENEMYSTATE.ATTACK);
        }
        else
        {
            // Nếu không khóa hướng, bạn vẫn có thể làm mượt hoặc cập nhật destination như trên
            //miniBoss.SetDestinationToPlayerPosition(false); // có thể loại bỏ gọi này vì đã update ở trên
            miniBoss.Rotation();
        }
    }


    public override void Exit()
    {
        miniBoss.beforState = ENEMYSTATE.RUN;
        miniBoss.Animator.SetBool("IsRun", false);

    }
    private void SmoothDestination()
    {
        Vector3 targetPos = miniBoss.m_Player.transform.position;
        Vector3 currentDest = miniBoss.NavmeshAgent.destination;
        float threshold = 0.5f; // Ngưỡng cập nhật, có thể điều chỉnh

        if (Vector3.Distance(currentDest, targetPos) > threshold)
        {
            // Làm mượt destination
            Vector3 smoothDest = Vector3.Lerp(currentDest, targetPos, Time.deltaTime * 3f);
            miniBoss.NavmeshAgent.SetDestination(smoothDest);
        }
    }
}

