using System.Collections;
using UnityEngine;

public class ThrowAxeStateBT : State<BullTankBoss>
{
    private SuperComboState parent;
    private bool m_IsThrow;
    private bool m_HasLockedDirection;
    private Vector3 m_ThrowDirection;
    public ThrowAxeStateBT(BullTankBoss stateMachine, SuperComboState parent) : base(stateMachine)
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
                stateMachine.StartCoroutine(ThrowDuration());
                m_IsThrow = true;
            }
        }
        base.Update();
    }

    IEnumerator ThrowDuration()
    {
        yield return new WaitForSeconds(2f);
        m_HasLockedDirection = true;
        m_ThrowDirection = stateMachine.transform.forward;
        stateMachine.transform.rotation = Quaternion.LookRotation(m_ThrowDirection);
        Debug.Log("tesst");
        stateMachine.Animator.SetTrigger("EndThrow");
        //m_IsThrow = true;
        yield return new WaitUntil(() => stateMachine.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack03_SpawnAxe"));
        yield return new WaitUntil(() =>
        {
            stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
            return (stateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        });
        parent.NotifyAttackThrowStateComplete(true);
    }
}
