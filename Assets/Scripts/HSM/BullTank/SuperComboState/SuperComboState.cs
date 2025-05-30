using UnityEngine;

public class SuperComboState : CompositeState<BullTankBoss>
{
    private bool m_IsAwakenStateComplete;
    private bool m_IsThrowStateComplete;
    private bool m_IsCoolDownStateComplete;
    private bool m_IsAttackAxeStateComplete;

    public SuperComboState(BullTankBoss stateMachine) : base(stateMachine)
    {
        var awakenStateBT = new AwakenStateBT(stateMachine, this);
        var throwAxeStateBT = new ThrowAxeStateBT(stateMachine, this);
        var coolDownStateBT = new CoolDownStateBT(stateMachine, this);
        var attackAxeStateBT = new AttackAxeStateBT(stateMachine, this);
        coolDownStateBT.OnStateComplete += state => ChangeChild<AwakenStateBT>();
        awakenStateBT.OnStateComplete += state => ChangeChild<AttackAxeStateBT>();

        AddChild(awakenStateBT);
        AddChild(throwAxeStateBT);
        AddChild(coolDownStateBT);
        AddChild(attackAxeStateBT);  
        SetInitial<CoolDownStateBT>();
    }
    public override void Enter()
    {
        base.Enter();
        stateMachine.SetSuperStateHSM(this);
    }
    public override void Update()
    {
        if(m_IsAwakenStateComplete && !m_IsAttackAxeStateComplete)
        {
            ChangeChild<AttackAxeStateBT>();
            return;
        }
        if (m_IsAttackAxeStateComplete && !m_IsThrowStateComplete)
        {
            ChangeChild<ThrowAxeStateBT>();
            return;
        }
        if(m_IsThrowStateComplete && !m_IsCoolDownStateComplete)
        {
            ChangeChild<CoolDownStateBT>();
            return;
        }
        base.Update();
    }

    public void NotifyAwakenStateComplete()
    {
        m_IsAwakenStateComplete = true;
    }
    public void NotifyAttackThrowStateComplete(bool isComplete)
    {
        m_IsThrowStateComplete = isComplete;
    }
    public void NotifyCoolDownStateComplete(bool isComplete)
    {
        m_IsCoolDownStateComplete = isComplete;
    }
    public void NotifyAttackAxeStateComplete(bool isComplete)
    {
        m_IsAttackAxeStateComplete = isComplete;
    }

}
