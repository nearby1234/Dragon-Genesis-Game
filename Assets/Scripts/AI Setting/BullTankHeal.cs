using Unity.Behavior;
using UnityEngine;
using UnityEngine.UI;

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
    public Slider healSlider;
    private bool m_IsDie;
    private BehaviorGraphAgent graphAgent;

    private void Start()
    {
        maxHeal = bulltankSO.Heal;
        heal = maxHeal;
        healSlider.maxValue = heal;
        healSlider.value = heal;
        graphAgent = GetComponent<BehaviorGraphAgent>();
        if (graphAgent != null)
        {
            graphAgent.SetVariableValue<float>("BullTankHeal", Heal);
        }
    }
    private void RotationSlider()
    {
        Vector3 dir = (Camera.main.transform.position - healSlider.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        healSlider.transform.rotation = lookRotation;

    }

    public void ReduceHeal(float damage)
    {
        Heal -= damage;
        healSlider.value = Heal;
        if (graphAgent != null)
        {
            if (graphAgent.GetVariable<bool>("IsWait", out BlackboardVariable<bool> IsWait))

                if (IsWait) return;

            graphAgent.SetVariableValue<bool>("IsHit", true);
            graphAgent.SetVariableValue<float>("BullTankHeal", Heal);
        }
        if (m_IsDie) return;
        if (Heal <= 0)
        {
            Debug.Log("bulltank Die");
            m_IsDie = true;

        }
    }
}
