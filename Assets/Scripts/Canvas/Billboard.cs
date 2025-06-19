using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private bool m_Invert;
    [SerializeField] private bool m_IsClickMainMenu;
    [SerializeField] private bool m_IsClickTalkNPC;

    private void Awake()
    {
        mainCamera = Camera.main != null ? Camera.main.transform : null;
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverClickMainMenu);
            ListenerManager.Instance.Register(ListenType.CLICK_TALK_NPC, OnEventClickTalkNPC);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverClickMainMenu);
            ListenerManager.Instance.Unregister(ListenType.CLICK_TALK_NPC, OnEventClickTalkNPC);
        }
    }
    private void Update()
    {
        if (m_IsClickMainMenu) return;
        RotationTarget();
    }
    private void RotationTarget()
    {
        Vector3 direction = transform.position - mainCamera.position;
        direction.y = 0;
        if (m_Invert)
        {
            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
        }
        else
        {
            if (m_IsClickTalkNPC)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -30f, 0);
            }



        }

    }
    private void ReceiverClickMainMenu(object value)
    {
        m_IsClickMainMenu = true;
    }

    private void OnEventClickTalkNPC(object value)
    {
        if (value is bool isClick)
            m_IsClickTalkNPC = isClick;

    }


}
