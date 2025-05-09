using System.Collections.Generic;
using UnityEngine;

public class SetupAura : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_Aura = new();
    private WormBoss m_Boss;

    private void Awake()
    {
        m_Boss = GetComponent<WormBoss>();
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.BOSS_STATE_CURRENT, SetOffAura);
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_PLAYAGAIN, ReceiverOnClickPlayAgain);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.BOSS_STATE_CURRENT, SetOffAura);
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_PLAYAGAIN, ReceiverOnClickPlayAgain);
        }

    }
    void Update()
    {
        InitAura();
    }
    private void InitAura()
    {
        if (m_Boss != null)
        {
            if (m_Boss.m_WormBossHeal <= 0)
            {
                foreach (var a in m_Aura)
                {
                    a.gameObject.SetActive(false);
                    if (a.isPlaying)
                    {
                        a.Stop();
                    }
                }
            }
            else if (m_Boss.IsRageState && m_Aura != null)
            {
                foreach (var a in m_Aura)
                {
                    a.gameObject.SetActive(true);
                    if (!a.isPlaying)
                    {
                        a.Play();
                    }
                }
            }
        }
    }
    private void SetOffAura(object value)
    {
        if (value != null)
        {
            if (value is WORMSTATE wormState)
            {
                if (wormState.Equals(WORMSTATE.DIE))
                {
                    foreach (var a in m_Aura)
                    {
                        a.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    private void ReceiverOnClickPlayAgain(object value)
    {
        foreach (var a in m_Aura)
        {
            a.gameObject.SetActive(false);
        }

    }
}
