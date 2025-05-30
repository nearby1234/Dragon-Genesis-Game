using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Check Detection Range", story: "[Player] in Detection [Range] of [Agent]", category: "Conditions", id: "c4f1a41d73dcf2036412f5a5b70e4481")]
public partial class CheckDetectionRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<BehaviorTreeSO> Range;
    //[SerializeReference] public BlackboardVariable<BehaviorTreeSO> BehaviorTreeSO;
    private float m_zoneDetec;

    public override bool IsTrue()
    {
        bool distance = BehaviorUtils.DistanceToPlayer(Agent, Player, m_zoneDetec);
       return distance;
    }

    public override void OnStart()
    {
        if(Player.Value == null || Agent.Value == null || Range.Value == null) return;
        m_zoneDetec = Range.Value.WanderRadius;
    }

    public override void OnEnd()
    {
    }
}
