using System.Collections;
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
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_HAS_ACCEPT_QUEST, ReceiverValueHasAcceptMission);
            ListenerManager.Instance.Register(ListenType.CLICK_TALK_NPC, ReceiverEventClickTalkNPC);
            ListenerManager.Instance.Register(ListenType.PLAYER_HAS_ACCEPT_QUEST, ReceiverEventPlayerClickAccept);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_HAS_ACCEPT_QUEST, ReceiverValueHasAcceptMission);
            ListenerManager.Instance.Unregister(ListenType.CLICK_TALK_NPC, ReceiverEventClickTalkNPC);
            ListenerManager.Instance.Unregister(ListenType.PLAYER_HAS_ACCEPT_QUEST, ReceiverEventPlayerClickAccept);
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
        if (m_PlayerHasAcceptMission) m_IconQuestionMark.SetActive(false);

    }
    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void ReceiverEventClickTalkNPC(object value)
    {
        shouldRotate = false;
        StartCoroutine(RotateTowardCameraCoroutine());
    }
    private void ReceiverEventPlayerClickAccept(object value)
    {
        if(value is bool isClick) shouldRotate = isClick;

    }
    private void ReceiverValueHasAcceptMission(object value)
    {
        if (value != null)
        {
            if (value is bool HasAcceptMission)
            {
                m_PlayerHasAcceptMission = HasAcceptMission;
                Debug.Log("m_PlayerHasAcceptMission " + m_PlayerHasAcceptMission);
            }
        }
    }
    private IEnumerator RotateTowardCameraCoroutine()
    {
        if (!CameraManager.HasInstance)
            yield break;

        Transform cam = CameraManager.Instance.GetObject("NPCCamera").transform;
        const float speed = 120f;    // độ nhanh (độ/giây)
        const float threshold = 0.1f;    // khi nhỏ hơn góc này coi như đã xong

        while (true)
        {
            // Tính targetY với bù 180° để mặt NPC hướng về camera
            Vector3 dir = (cam.position - transform.position).normalized;
            float rawY = Quaternion.LookRotation(dir).eulerAngles.y;
            float targetY = rawY + 180f;  // bù thêm 180 độ

            // Lấy góc Y hiện tại và tính góc mới theo quãng ngắn nhất
            float currentY = transform.eulerAngles.y;
            float delta = speed * Time.deltaTime;
            float newY = Mathf.MoveTowardsAngle(currentY, targetY, delta);

            // Gán rotation
            transform.rotation = Quaternion.Euler(0f, newY, 0f);

            // Nếu đã đến gần target thì dừng
            if (Mathf.Abs(Mathf.DeltaAngle(newY, targetY)) < threshold)
            {
                // Snap hẳn về đúng góc target (loại bỏ sai số)
                transform.rotation = Quaternion.Euler(0f, targetY, 0f);
                break;
            }

            yield return null;
        }
    }
}
