using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class NPCRotation : MonoBehaviour
{
    public float rotationSpeed = 3f; // T?c ?? xoay
    private Transform player;
    private bool shouldRotate = false;
    private bool m_PlayerHasAcceptMission;
    [SerializeField] private GameObject m_IconQuestionMark;


    private void Start()
    {
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_HAS_ACCEPT_QUEST, ReceiverValueHasAcceptMission);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_HAS_ACCEPT_QUEST, ReceiverValueHasAcceptMission);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            shouldRotate = true;

            if (UIManager.HasInstance)
            {
                UIManager.Instance.ShowPopup<DialobGuidePopup>();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        shouldRotate = false;
        if (UIManager.HasInstance)
        {
            UIManager.Instance.HidePopup<DialobGuidePopup>();
        }
    }
    private void Update()
    {
        if (shouldRotate && player != null) RotateTowardsPlayer();
        if(m_PlayerHasAcceptMission) m_IconQuestionMark.gameObject.SetActive(false);

    }
    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    private void ReceiverValueHasAcceptMission(object value)
    {
        if(value != null)
        {
            if(value is bool HasAcceptMission)
            {
                m_PlayerHasAcceptMission = HasAcceptMission;
                Debug.Log("m_PlayerHasAcceptMission " + m_PlayerHasAcceptMission);
            }
        }
    }
}
