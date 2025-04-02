using UnityEngine;

public class PlayerDialog : MonoBehaviour
{
    private bool m_IsPressButtonJ;

    private void Update()
    {
        OnButtonJ();
    }

    private void OnButtonJ()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            m_IsPressButtonJ = true;
        }else if(Input.GetKeyUp(KeyCode.J))
        {
            m_IsPressButtonJ = false;
        }

        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.PLAYER_PRESS_BUTTON_J, m_IsPressButtonJ);
        }
      
    }
}
