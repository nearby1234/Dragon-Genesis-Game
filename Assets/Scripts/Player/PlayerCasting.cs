using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCasting : MonoBehaviour
{
    [SerializeField] private KeyCode m_ButtonCasting;
    [SerializeField] private ParticleSystem m_PowerCasting;
    [SerializeField] private ParticleSystem m_PowerCastingAsura;
    [SerializeField] private ParticleSystem m_PowerCastingBigBang;
    [SerializeField] private float m_Time;
    [SerializeField] private float m_TimeActivePower;
    [SerializeField] private float m_TimeOffPower;

    [SerializeField] private bool m_CanCasting = true;


    private void ActivePartycleCasting()
    {
        ToggleParticle(m_PowerCasting, true);
    }
    public void Casting()
    {
        if (Input.GetKey(m_ButtonCasting) && m_CanCasting)
        {
            ActivePartycleCasting();
            ActiveAsura();
        }
    }
    private void ActiveAsura()
    {
        m_Time += Time.deltaTime;
        if (m_Time >= m_TimeActivePower)
        {
            if (m_PowerCasting != null && m_PowerCastingAsura != null && m_PowerCastingBigBang != null)
            {
                ToggleParticle(m_PowerCasting,false);
                ToggleParticle(m_PowerCastingBigBang,true);

                m_CanCasting = false;
                StartCoroutine(DelayBigBang());
            }

            ToggleParticle(m_PowerCastingAsura,true);
        }
    }
    private IEnumerator DelayBigBang()
    {
        yield return new WaitForSeconds(0.5f);

        ToggleParticle(m_PowerCastingBigBang,false);

        StartCoroutine(WaitForKeyRelease());
    }
    private IEnumerator WaitForKeyRelease()
    {
        while (Input.GetKey(m_ButtonCasting))
        {
            yield return null; // Đợi cho đến khi phím được thả
        }

        m_CanCasting = true;
        m_Time = 0;

        StartCoroutine(OffPowerAsura());
    }

    private IEnumerator OffPowerAsura()
    {
        yield return new WaitForSeconds(m_TimeOffPower);

        ToggleParticle(m_PowerCastingAsura, false);
    }

    private void ToggleParticle(ParticleSystem particle, bool state)
    {
        if (particle == null) return;
        if (state)
        {
            if (!particle.isPlaying) 
            {
                particle.gameObject.SetActive(true);
                particle.Play();
            }
        }
        else
        {
            if (particle.isPlaying) particle.Stop();
            particle.gameObject.SetActive(false);
        }
    }

}
