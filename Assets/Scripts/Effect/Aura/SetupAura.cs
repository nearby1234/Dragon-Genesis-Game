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
    void Update()
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
}
