using System.Collections;
using UnityEngine;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Casting();
    }

    private void ActivePartycleCasting()
    {
        if (m_PowerCasting != null) m_PowerCasting.gameObject.SetActive(true);

    }

    private void Casting()
    {
        if (Input.GetKey(m_ButtonCasting) && m_CanCasting)
        {
            //PlayerManager.instance.playerAnim.GetAnimator().Play("Attack_3Combo_2_Duplicate");
            ActivePartycleCasting();
            ActiveAusa();
        }
    }

    private void ActiveAusa()
    {
        m_Time += Time.deltaTime;
        if (m_Time >= m_TimeActivePower)
        {
            if (m_PowerCasting != null && m_PowerCastingAsura != null && m_PowerCastingBigBang != null)
            {
                m_PowerCasting.gameObject.SetActive(false);
                m_PowerCastingBigBang.gameObject.SetActive(true);
                m_CanCasting = false;
                StartCoroutine(DelayBigBang());

            }
            m_PowerCastingAsura.gameObject.SetActive(true);
        }


    }

    private IEnumerator DelayBigBang()
    {
        yield return new WaitForSeconds(0.5f);
        m_PowerCastingBigBang.gameObject.SetActive(false);
        StartCoroutine(WaitForKeyReliease());
    }

    private IEnumerator WaitForKeyReliease()
    {
        while (Input.GetKey(m_ButtonCasting))
        {
            yield return null;
        }

        m_CanCasting = true;
        m_Time = 0;
        StartCoroutine(OffPowerAsura());
    }

    private IEnumerator OffPowerAsura()
    {
        yield return new WaitForSeconds(m_TimeOffPower);
        m_PowerCastingAsura.gameObject.SetActive(false);
    }


}
