using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] private KeyCode m_ButtonDodge;
    [SerializeField] private bool m_IsPressDodge;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Dodge();
    }

    private void Dodge()
    {
        if (Input.GetKeyDown(m_ButtonDodge))
        {
            m_IsPressDodge = true;
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsPressDodge", true);
        }
        else
        {
            m_IsPressDodge = false;
            PlayerManager.instance.playerAnim.GetAnimator().SetBool("IsPressDodge", false);
        }
    }
}
