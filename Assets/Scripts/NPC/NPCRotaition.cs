using UnityEngine;

public class NPCRotaition : MonoBehaviour
{
    public float rotationSpeed = 3f; // T?c ?? xoay
    private Transform player;
    private bool shouldRotate = false;
    private bool m_IsPressJ;
    private bool m_IsPlayerCollison;

    private void Awake()
    {
        
    }
    private void Start()
    {
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_PRESS_BUTTON_J,PlayerIsPressedButtonJ);
        }
      
       
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_PRESS_BUTTON_J, PlayerIsPressedButtonJ);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ki?m tra va ch?m v?i Player
        {
            player = other.transform;
            shouldRotate = true;
            m_IsPlayerCollison = true;
            if(m_IsPlayerCollison)
            {
                if (UIManager.HasInstance)
                {
                    UIManager.Instance.ShowPopup<DialobGuidePopup>();
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        shouldRotate = false;
        m_IsPlayerCollison = false;
        if (UIManager.HasInstance)
        {
            UIManager.Instance.HidePopup<DialobGuidePopup>();
        }
    }

    private void Update()
    {
        if (shouldRotate && player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void PlayerIsPressedButtonJ(object value)
    {
        if(value !=null)
        {
            if(value is bool IsPressJ)
            {
                m_IsPressJ = IsPressJ;
            }
        }
    }
}
