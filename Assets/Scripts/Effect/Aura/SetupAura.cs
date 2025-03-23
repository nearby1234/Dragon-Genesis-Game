using System.Collections.Generic;
using UnityEngine;

public class SetupAura : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_Aura = new();
    [SerializeField] private bool m_IsRage;
    private WormBoss m_Boss;

    private void Awake()
    {
        m_Boss = GetComponent<WormBoss>();
    }
    void Update()
    {
        if (m_Boss != null)
        {
            if (m_Boss.IsRageState && m_Aura != null)
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
