using UnityEngine;

public class WormHitState : BaseState<WormBoss, WORMSTATE>
{
    private float m_Timer = 0f;
    private float m_HitDuration = 3f;
    public WormHitState(WormBoss boss, FSM<WormBoss, WORMSTATE> fsm) : base(boss, fsm)
    {
    }

    public override void Enter()
    {
        boss.ChangeStateCurrent(WORMSTATE.HIT);
        boss.Animator.SetTrigger("Hit");
        m_Timer = 0f;
        Debug.Log("tesst");
    }

    public override void Exit()
    {
        boss.ChangeBeforeState(WORMSTATE.HIT);
        boss.Animator.ResetTrigger("Hit");
    }

    public override void Updates()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= m_HitDuration)
        {
            Debug.Log(m_Timer);
            boss.RequestStateTransition(WORMSTATE.IDLE);
        }
    }
}
