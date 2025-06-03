using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.UI;

public struct DataBullTankBoss
{
    public CreepType creepType;
    public float Heal;

}

public class BullTankHeal : MonoBehaviour
{
    public float heal;
    public float Heal
    {
        get => heal;
        set
        {
            heal = value;
            Debug.Log($"Heal : {heal}");
        }
    }

    public float maxHeal;
    public BehaviorTreeSO bulltankSO;
    [SerializeField] private CreepType creepType;
    public CreepType CreepType => creepType;
    private bool m_IsDie;
    private BehaviorGraphAgent graphAgent;
    public event Action<BehaviorGraphAgent> OnActionAgent;


    private void Start()
    {
        maxHeal = bulltankSO.Heal;
        heal = maxHeal;
        if(ListenerManager.HasInstance)
        {
            DataBullTankBoss dataBull = new DataBullTankBoss()
            {
                creepType = this.creepType,
                Heal = heal,
            };
            ListenerManager.Instance.BroadCast(ListenType.BOSSTYPE_SEND_HEAL_VALUE, dataBull);
        }    
        graphAgent = GetComponent<BehaviorGraphAgent>();
        if (graphAgent != null)
        {
            OnActionAgent?.Invoke(graphAgent);
            graphAgent.SetVariableValue<float>("BullTankHeal", Heal);
        }
    }

    public void ReduceHeal(float damage)
    {

        Heal -= damage;
        //healSlider.value = Heal;
        if (ListenerManager.HasInstance)
        {
            DataBullTankBoss data = new DataBullTankBoss
            {
                creepType = this.creepType,
                Heal = this.Heal
            };

            ListenerManager.Instance.BroadCast(ListenType.BOSSTYPE_UPDATE_HEAL_VALUE, data);
        } 
            
        if (graphAgent != null)
        {
            if (graphAgent.GetVariable<bool>("IsWait", out BlackboardVariable<bool> IsWait))

                if (IsWait) return;

            graphAgent.SetVariableValue<bool>("IsHit", true);
            graphAgent.SetVariableValue<float>("BullTankHeal", Heal);
            OnActionAgent?.Invoke(graphAgent);
        }
        if (m_IsDie) return;
        if (Heal <= 0)
        {
            Debug.Log("bulltank Die");
            m_IsDie = true;

        }
    }
}
