using UnityEngine;
using UnityEngine.InputSystem;

public class Cheat : MonoBehaviour
{
    [SerializeField] private Transform m_BossTranformSave;
    [SerializeField] private Transform m_player;
    [SerializeField] private CharacterController charController;
    [SerializeField] private InputAction m_translatButton;

    void Start()
    {
        m_translatButton.Enable();
        m_translatButton.performed += OnClickTranslateButton;
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.SEND_POS_SPAWN_PLAYER,m_BossTranformSave);
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
    //private void ReceiverEventClickButtonPlayAgain(object value )
    //{
    //    if(GameManager.HasInstance)
    //    {
    //        GameManager.Instance.m_SpawnPlayer = m_BossTranformSave;
    //    }
    //}
}
