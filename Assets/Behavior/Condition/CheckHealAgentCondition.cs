using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckHealAgent", story: "[Agent] is blood remains [percent] %", category: "Conditions", id: "edf554c26b2f1f12e4fd335c9a03a257")]
public partial class CheckHealAgentCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Percent;
    private BullTankHeal bulltankHeal;
    private float healBase;
    private float healTarget;
    public override bool IsTrue()
    {
        if(bulltankHeal.heal <= healTarget)
        {
            return true;
        }    
        return false;
    }

    public override void OnStart()
    {
        if(Agent.Value == null) return;
        if(Percent == null) return ;
        bulltankHeal = Agent.Value.GetComponent<BullTankHeal>();
        if(bulltankHeal !=null)
        {
            healBase = bulltankHeal.maxHeal;
            healTarget = healBase * (Percent.Value / 100);
        } 
            
        
    }

    public override void OnEnd()
    {
    }
}
