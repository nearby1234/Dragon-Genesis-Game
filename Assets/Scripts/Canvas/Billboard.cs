using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private bool m_Invert;
    [SerializeField] private bool m_IsClick;

    private void Awake()
    {
        mainCamera = Camera.main != null ? Camera.main.transform : null;
    }
    private void Start()
    {
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, ReceiverClickMainMenu);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, ReceiverClickMainMenu);
        }
    }
    private void Update()
    {
        if (m_IsClick) return;
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
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    private void ReceiverClickMainMenu(object value)
    {
        m_IsClick = true;
    }
}
