using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public struct DataBullTankBoss
{
    public CreepType creepType;
    public float m_Heal;
}

public class BullTankHeal : MonoBehaviour
{
    public float heal;
    public float maxHeal;
    public BehaviorTreeSO bulltankSO;
    public event Action<BehaviorGraphAgent> OnActionAgent;
    [SerializeField] private CreepType creepType;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private PlayParrticleEarth playParrticle;
    private BehaviorGraphAgent graphAgent;
    private bool m_IsDie;
    private Transform baseTranform;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playParrticle = GetComponent<PlayParrticleEarth>();
    }
    private void Start()
    {
        baseTranform = transform;
        maxHeal = bulltankSO.Heal;
        heal = maxHeal;
        if (ListenerManager.HasInstance)
        {
            DataBullTankBoss dataBull = new()
            {
                creepType = this.creepType,
                m_Heal = heal,
            };
            ListenerManager.Instance.BroadCast(ListenType.SEND_DATAHEAL_VALUE, dataBull);
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_PLAYAGAIN, ReceiverEventOnClickPlayAgain);

        }
        graphAgent = GetComponent<BehaviorGraphAgent>();
        if (graphAgent != null)
        {
            OnActionAgent?.Invoke(graphAgent);
            graphAgent.SetVariableValue<float>("BullTankHeal", heal);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_PLAYAGAIN, ReceiverEventOnClickPlayAgain);
        }
    }

    public void ReduceHeal(float damage)
    {

        heal -= damage;
        Debug.Log($"heal : {heal}");

        if (ListenerManager.HasInstance)
        {
            DataBullTankBoss data = new()
            {
                creepType = this.creepType,
                m_Heal = heal
            };
            ListenerManager.Instance.BroadCast(ListenType.BOSSTYPE_UPDATE_HEAL_VALUE, data);
        }

        if (graphAgent != null)
        {
            if (graphAgent.GetVariable<bool>("IsWait", out BlackboardVariable<bool> IsWait))

                if (IsWait) return;

            graphAgent.SetVariableValue<bool>("IsHit", true);
            graphAgent.SetVariableValue<float>("BullTankHeal", heal);
            OnActionAgent?.Invoke(graphAgent);
        }
        if (m_IsDie) return;
        if (heal <= 0)
        {
            Debug.Log("bulltank Die");
            m_IsDie = true;
            
            if (UIManager.HasInstance)
            {
                //UIManager.Instance.HideScreen
                PopupMessage msg = new()
                {
                    popupType = PopupType.BULLTANK_DIE,

                };
                if (PlayerManager.HasInstance)
                {
                    PlayerManager.Instance.m_IsShowingLosePopup = true;
                }
                UIManager.Instance.ShowPopup<LosePopup>(msg, true);
            }
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.BOSS_STATE_CURRENT, creepType);
                ListenerManager.Instance.BroadCast(ListenType.CREEP_IS_DEAD, creepType);
            }

        }
    }

    private void ReceiverEventOnClickPlayAgain(object value)
    {
        if (GameManager.HasInstance)
        {
            CreepType creep = GameManager.Instance.GetCreepType();
            if (creep == CreepType.BullTank)
            {
                heal = maxHeal;
                if (ListenerManager.HasInstance)
                {
                    DataBullTankBoss data = new()
                    {
                        creepType = this.creepType,
                        m_Heal = heal,
                    };
                    Debug.Log($"DataBullTankBoss : {data.m_Heal}");
                    ListenerManager.Instance.BroadCast(ListenType.BOSSTYPE_SEND_HEAL_VALUE, data);
                }
                navMeshAgent.Warp(baseTranform.position); // Reset position to base transform position
                ResetNode();
                ResetParticle();
                animator.CrossFade("Idle", 0.1f);
                StartCoroutine(SetStateNodeClickPlayAgain());
            }
        }
    }

    private void ResetNode()
    {
        if (graphAgent != null)
        {
            graphAgent.SetVariableValue<float>("BullTankHeal", heal);
            graphAgent.SetVariableValue<bool>("IsShowHealBar", false);
            graphAgent.SetVariableValue<PhaseState>("PhaseStateBoss", PhaseState.None);
            graphAgent.SetVariableValue<bool>("HasChased", false);
            graphAgent.SetVariableValue<bool>("IsClickPlayAgain", true);
            graphAgent.SetVariableValue<bool>("IsAngry", false);
            graphAgent.SetVariableValue<bool>("IsSuperAngry", false);
        }
    }
    private void ResetParticle()
    {
        if (playParrticle != null)
        {
            playParrticle.HideAxeFire();
            playParrticle.HideThunderArmor();
        }
    }

    IEnumerator SetStateNodeClickPlayAgain()
    {
        yield return new WaitForSeconds(1f);
        if (graphAgent != null)
        {
            graphAgent.SetVariableValue<bool>("IsClickPlayAgain", false);
        }
    }
}
