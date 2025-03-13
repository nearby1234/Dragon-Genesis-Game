using System.Collections;
using UnityEngine;

public class AttackState : BaseState<BlackKnightBoss, ENEMYSTATE>
{
    private Coroutine attackCoroutine;
    private Quaternion lockedRotation;

    public AttackState(BlackKnightBoss miniBoss, FSM<BlackKnightBoss, ENEMYSTATE> fsm) : base(miniBoss, fsm) { }

    public override void Enter()
    {
        // Chuyển sang trạng thái Attack và khóa hướng tại thời điểm bắt đầu
        boss.ChangeStateCurrent(ENEMYSTATE.ATTACK);
        boss.NavmeshAgent.isStopped = true;
        
        lockedRotation = boss.transform.rotation; // Lấy hướng hiện tại của boss
        attackCoroutine = boss.StartCoroutine(AttackSequence());
    }

    public override void Updates()
    {

    }

    public override void Exit()
    {
        if (attackCoroutine != null)
        {
            boss.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        boss.NavmeshAgent.isStopped = false;
        boss.m_IsMiniBossAttacked = false;
        boss.NavmeshAgent.Warp(boss.transform.position);
    }
    private IEnumerator AttackSequence()
    {
        while (true)
        {
            boss.transform.rotation = lockedRotation;
            yield return boss.PlaySingleAttackAnimation();
            yield return new WaitForSeconds(1f);

            if (!boss.PlayerInStopRange())
            {
                break;
            }
        }
        yield return new WaitForSeconds(0.1f);

        // Nếu sau attack, player không còn trong khoảng dừng, chuyển qua IdleStand để phát animation IdleCombat
        if (!boss.PlayerInStopRange())
            boss.RequestStateTransition(ENEMYSTATE.IDLESTAND);
        else
            boss.RequestStateTransition(ENEMYSTATE.RUN);
    }
}
