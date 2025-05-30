using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "In Range Throw", story: "[Player] in range throw [data] Of [Agent]", category: "Conditions", id: "5db2693e123a81f38115b3d63e1c55f4")]
public partial class InRangeThrowCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<BehaviorTreeSO> Data;
    private float rangeAttack;
    public override bool IsTrue()
    {
        bool isRangeAttack = BehaviorUtils.InRangeAttack(Agent, Player, rangeAttack);
        if (isRangeAttack)
        {
            return true;
        }
        else return false;
    }

    public override void OnStart()
    {
        if (Player.Value == null) return;
        if (Agent.Value == null) return;
        if (Data.Value == null)
        {
            Debug.LogWarning("không có BehaviorTreeSO");
            return;
        }

        rangeAttack = Data.Value.RangeAttackThrow;
    }

    public override void OnEnd()
    {
    }
}
