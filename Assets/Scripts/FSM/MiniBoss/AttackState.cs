using System.Collections;
using UnityEngine;
using static MiniBoss;

public class AttackState : BaseState
{
    private Coroutine attackCoroutine;
    private Quaternion lockedRotation;

    public AttackState(MiniBoss miniBoss, FSM fsm) : base(miniBoss, fsm) { }

    public override void Enter()
    {
        // Chuyển sang trạng thái Attack và khóa hướng tại thời điểm bắt đầu
        miniBoss.ChangeStateCurrent(MiniBoss.ENEMYSTATE.ATTACK);
        miniBoss.NavmeshAgent.isStopped = true;
        
        lockedRotation = miniBoss.transform.rotation; // Lấy hướng hiện tại của boss
        attackCoroutine = miniBoss.StartCoroutine(AttackSequence());
    }

    public override void Updates()
    {

    }

    public override void Exit()
    {
        if (attackCoroutine != null)
        {
            miniBoss.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        miniBoss.NavmeshAgent.isStopped = false;
        miniBoss.m_IsMiniBossAttacked = false;
        miniBoss.NavmeshAgent.Warp(miniBoss.transform.position);
    }
    private IEnumerator AttackSequence()
    {
        while (true)
        {
            miniBoss.transform.rotation = lockedRotation;
            yield return miniBoss.PlaySingleAttackAnimation();
            yield return new WaitForSeconds(1f);

            if (!miniBoss.PlayerInStopRange())
            {
                break;
            }
        }
        yield return new WaitForSeconds(0.1f);

        // Nếu sau attack, player không còn trong khoảng dừng, chuyển qua IdleStand để phát animation IdleCombat
        if (!miniBoss.PlayerInStopRange())
            miniBoss.RequestStateTransition(ENEMYSTATE.IDLESTAND);
        else
            miniBoss.RequestStateTransition(ENEMYSTATE.RUN);
    }
}
