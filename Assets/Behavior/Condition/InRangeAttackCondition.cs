using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "InRangeAttack", story: "[Player] In Range Attack Sword [BehaviorTreeSO] Of [Agent]", category: "Conditions", id: "4eb1315f1e94cc35cbc47f654e694368")]
public partial class InRangeAttackCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> PLayer;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<BehaviorTreeSO> BehaviorTreeSO;
    private float rangeAttack;
    public override bool IsTrue()
    {
        bool isRangeAttack = BehaviorUtils.InRangeAttack(Agent, PLayer, rangeAttack);
        if (isRangeAttack)
        {
            return true;
        }
        else return false;
    }

    public override void OnStart()
    {
        if (PLayer.Value == null) return;
        if (Agent.Value == null) return;
        if (BehaviorTreeSO.Value == null)
        {
            Debug.LogWarning("không có BehaviorTreeSO");
            return;
        }

        rangeAttack = BehaviorTreeSO.Value.RangeAttackSword;

    }

    public override void OnEnd()
    {
    }
}
