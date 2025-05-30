using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cheat : BaseManager<Cheat>
{
    [SerializeField] private Transform m_BossTranformSave;
    [SerializeField] private Transform m_player;
    [SerializeField] private CharacterController charController;
    [SerializeField] private InputAction m_translatButton;
    [Header("Particle System Setting")]
    [SerializeField] private ParticleSystem m_PotralOpen;
    [SerializeField] private ParticleSystem m_PotralLoop;

    void Start()
    {
        m_translatButton.Enable();
        m_translatButton.performed += OnClickTranslateButton;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.SEND_POS_SPAWN_PLAYER, m_BossTranformSave);
        }

    }
    private void OnDestroy()
    {
        m_translatButton.performed -= OnClickTranslateButton;
        m_translatButton.Disable();

    }

    private void OnClickTranslateButton(InputAction.CallbackContext callback)
    {
        if (m_BossTranformSave == null || m_player == null) return;

        Vector3 targetPos = m_BossTranformSave.position;
        Quaternion targetRot = m_BossTranformSave.rotation;

        // 1) CharacterController
        if (charController != null)
        {
            charController.enabled = false;
            m_player.SetPositionAndRotation(targetPos, targetRot);
            charController.enabled = true;
            return;
        }
    }
    public void StartParticleOpen()
    {
        m_PotralOpen.gameObject.SetActive(true);
        if (m_PotralOpen.isPlaying)
        {
            m_PotralOpen.Stop();
            m_PotralOpen.Play();
        }
        if(AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("portalopen");
            Debug.Log($"portalopen");

        }
        StartCoroutine(WaitForAThenPlayB());
    }
    private IEnumerator WaitForAThenPlayB()
    {
        yield return new WaitForSeconds(0.6f);
        m_PotralLoop.gameObject.SetActive(true);
        if (m_PotralLoop.isPlaying)
        {
            m_PotralLoop.Stop();
            m_PotralLoop.Play();
        }
        if(AudioManager.HasInstance)
        {
            Debug.Log($"portalLoop");
            AudioManager.Instance.PlayLoopSE("portalLoop");
        }    
    }

    public void StopParticleOpen()
    {
        m_PotralOpen.gameObject.SetActive(false);
        m_PotralLoop.gameObject.SetActive(false);
    }
}
