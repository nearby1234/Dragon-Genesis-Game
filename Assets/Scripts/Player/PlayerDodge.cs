using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] private KeyCode m_ButtonDodge;
    [SerializeField] private bool m_IsPressDodge;
    public void Dodge()
    {
        if (Input.GetKeyDown(m_ButtonDodge))
        {
            m_IsPressDodge = true;
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsDodge", true);
        }
        else
        {
            m_IsPressDodge = false;
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsDodge", false);
        }
    }
}
