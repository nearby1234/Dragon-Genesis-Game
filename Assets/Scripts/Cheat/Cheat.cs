#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class Cheat : BaseManager<Cheat>
{
    [SerializeField] private Transform m_BossTranformSave;
    [SerializeField] private Transform cheatPosition;
    [SerializeField] private CreepType creepType;
    public CreepType CreepType
    {
        get => creepType;
        set
        {
            creepType = value;
            CheatPosition(creepType); // Update the cheat position based on the new creep type
        }
    }
    [SerializeField] private Transform m_player;
    [SerializeField] private CharacterController charController;
    [SerializeField] private InputAction m_translatButton;
    [Header("Particle System Setting")]
    [SerializeField] private ParticleSystem m_PotralOpen;
    [SerializeField] private ParticleSystem m_PotralLoop;
    [SerializeField] private GameObject m_ParentCheckPoint;
    [SerializeField] private List<Transform> checkPointList = new();

    protected override void Awake()
    {
        base.Awake();
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_POS, m_player);
        }
    }

    void Start()
    {
        m_translatButton.Enable();
        m_translatButton.performed += OnClickTranslateButton;
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.SEND_POS_SPAWN_PLAYER, m_BossTranformSave);
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_POS, m_player);
        }
        if (m_ParentCheckPoint != null)
        {
            for (int i = 0; i < m_ParentCheckPoint.transform.childCount; i++)
            {
                checkPointList.Add(m_ParentCheckPoint.transform.GetChild(i));
            }
        }

    }
    private void OnDestroy()
    {
        m_translatButton.performed -= OnClickTranslateButton;
        m_translatButton.Disable();

    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        // M?i l?n thay ??i creepType (ho?c m_ParentCheckPoint) trong Inspector, g?i l?i
      
        CheatPosition(creepType);

        // ?ánh d?u dirty ?? Inspector c?p nh?t giá tr? ngay
        EditorUtility.SetDirty(this);
    }
#endif

    private void OnClickTranslateButton(InputAction.CallbackContext callback)
    {
        TeleportPlayer();
    }

    public void TeleportPlayer()
    {
        if (cheatPosition == null || m_player == null) return;
        cheatPosition.GetPositionAndRotation(out Vector3 targetPos, out Quaternion targetRot);

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
        if (AudioManager.HasInstance)
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
        if (AudioManager.HasInstance)
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
    public void GetCheckPoint(CreepType creepType)
    {
        for(int i = 0; i < checkPointList.Count; i++)
        {
            if (checkPointList[i].GetComponent<SpawnPosPlayAgain>().CreepType == creepType)
            {
                m_BossTranformSave = checkPointList[i];
                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.SEND_POS_SPAWN_PLAYER, m_BossTranformSave);
                }
                return;
            }
        }
    }
    private void CheatPosition(CreepType creepType)
    {
        for (int i = 0; i < checkPointList.Count; i++)
        {
            if (checkPointList[i].GetComponent<SpawnPosPlayAgain>().CreepType == creepType)
            {
                cheatPosition = checkPointList[i];
                return;
            }
        }
    }
}
