using System;
using Unity.Behavior;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "RandomRateAttack", story: "[Agent] attack if in rate [duration] %", category: "Conditions", id: "22f7ce0714c11ec466b99fb9c6a7ac54")]
public partial class RandomRateAttackCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Duration;
    private float numberRandom;
    public override bool IsTrue()
    {
        if(numberRandom <= Duration.Value) return true;
        else return false;
    }

    public override void OnStart()
    {
        if (Agent.Value == null) return;
        if (Duration == null) return;
        numberRandom = Random.Range(0, 100);

    }

    public override void OnEnd()
    {
    }
}
