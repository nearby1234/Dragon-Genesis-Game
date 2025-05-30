using System.Collections;
using UnityEngine;

public class AttackThrowAxeBT : State<BullTankBoss>
{
    private SuperAttackStateBT parent;
    private bool m_IsThrow;
    private bool m_HasLockedDirection;
    private Vector3 m_ThrowDirection;
    private Coroutine m_CrThrowDuration;
    public AttackThrowAxeBT(BullTankBoss stateMachine, SuperAttackStateBT parent) : base(stateMachine)
    {
        this.parent = parent;
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSubStateHSM(this);
        stateMachine.Animator.SetTrigger("StartThrow");
    }
    public override void Update()
    {
        if (stateMachine.IsWithin(stateMachine.m_ZoneDetecPlayerDraw) && !m_HasLockedDirection)
        {
            stateMachine.Rotation();

            if (!m_IsThrow)
            {
                m_CrThrowDuration = stateMachine.StartCoroutine(ThrowDuration());
                m_IsThrow = true;
            }
        }
        base.Update();
    }
    public override void Exit()
    {
        stateMachine.StopCoroutine(m_CrThrowDuration);
        stateMachine.Animator.ResetTrigger("EndThrow");
        stateMachine.Animator.ResetTrigger("StartThrow");
        base.Exit();
    }

    IEnumerator ThrowDuration()
    {
        yield return new WaitForSeconds(4f);
        m_HasLockedDirection = true;
        m_ThrowDirection = stateMachine.transform.forward;
        stateMachine.transform.rotation = Quaternion.LookRotation(m_ThrowDirection);
        stateMachine.Animator.SetTrigger("EndThrow");
    }
    public override void OnAnimationComplete(NameState nameState)
    {
        base.OnAnimationComplete(nameState);
        if (nameState.Equals(NameState.Anim_SpawnAxe))
        {
            RaiseComplete();
        }
    }
}
