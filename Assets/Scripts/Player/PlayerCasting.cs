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

    // Update is called once per frame
    void Update()
    {
        Casting();
    }

    private void ActivePartycleCasting()
    {
        if (m_PowerCasting != null)
        {
            if (!m_PowerCasting.isPlaying)
            {
                m_PowerCasting.gameObject.SetActive(true);
                m_PowerCasting.Play(); // Đảm bảo ParticleSystem được phát
            }
        }
    }

    private void Casting()
    {
        if (Input.GetKey(m_ButtonCasting) && m_CanCasting)
        {
            // PlayerManager.instance.playerAnim.GetAnimator().Play("Attack_3Combo_2_Duplicate");
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
                if (m_PowerCasting.isPlaying) m_PowerCasting.Stop(); // Dừng trước khi tắt
                m_PowerCasting.gameObject.SetActive(false);

                m_PowerCastingBigBang.gameObject.SetActive(true);
                m_PowerCastingBigBang.Play(); // Phát BigBang hiệu ứng

                m_CanCasting = false;
                StartCoroutine(DelayBigBang());
            }

            if (!m_PowerCastingAsura.isPlaying)
            {
                m_PowerCastingAsura.gameObject.SetActive(true);
                m_PowerCastingAsura.Play(); // Đảm bảo Asura hiệu ứng chạy
            }
        }
    }

    private IEnumerator DelayBigBang()
    {
        yield return new WaitForSeconds(0.5f);

        if (m_PowerCastingBigBang != null)
        {
            if (m_PowerCastingBigBang.isPlaying) m_PowerCastingBigBang.Stop(); // Dừng trước khi tắt
            m_PowerCastingBigBang.gameObject.SetActive(false);
        }

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

        if (m_PowerCastingAsura != null)
        {
            if (m_PowerCastingAsura.isPlaying) m_PowerCastingAsura.Stop(); // Dừng trước khi tắt
            m_PowerCastingAsura.gameObject.SetActive(false);
        }
    }
}
