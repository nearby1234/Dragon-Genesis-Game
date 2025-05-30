using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/EnemyDetected")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "EnemyDetected", message: "[Agent] has Spotted [Enemy]", category: "Events", id: "01e284fa1147b83095131836c41480db")]
public partial class EnemyDetected : EventChannelBase
{
    public delegate void EnemyDetectedEventHandler(GameObject Agent, GameObject Enemy);
    public event EnemyDetectedEventHandler Event; 

    public void SendEventMessage(GameObject Agent, GameObject Enemy)
    {
        Event?.Invoke(Agent, Enemy);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> AgentBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Agent = AgentBlackboardVariable != null ? AgentBlackboardVariable.Value : default(GameObject);

        BlackboardVariable<GameObject> EnemyBlackboardVariable = messageData[1] as BlackboardVariable<GameObject>;
        var Enemy = EnemyBlackboardVariable != null ? EnemyBlackboardVariable.Value : default(GameObject);

        Event?.Invoke(Agent, Enemy);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        EnemyDetectedEventHandler del = (Agent, Enemy) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Agent;

            BlackboardVariable<GameObject> var1 = vars[1] as BlackboardVariable<GameObject>;
            if(var1 != null)
                var1.Value = Enemy;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as EnemyDetectedEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as EnemyDetectedEventHandler;
    }
}

