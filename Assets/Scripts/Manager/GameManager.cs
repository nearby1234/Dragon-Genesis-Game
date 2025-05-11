using System.Collections;
using UnityEngine;
using UnityEngineInternal;


public enum GAMESTATE
{
    NONE,
    MENULOADING,
    START,
    PLAYING,
    PAUSE,
    GAMEOVER,
}
public class GameManager : BaseManager<GameManager>
{
    public Transform m_SpawnPlayer;

    [SerializeField] private GAMESTATE m_GameState = GAMESTATE.NONE;
    public bool m_IsPlaying = false;

    
    public GAMESTATE GameState
    {
        get => m_GameState;
        set
        {
            m_GameState = value;
        }
    }
    private void Start()
    {
        StartGame();
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.SEND_POS_SPAWN_PLAYER,ReceiverEventPosPlayerSpawn);
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_PLAYAGAIN,ReceiverEventPlayerAGain);
        }    
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SEND_POS_SPAWN_PLAYER, ReceiverEventPosPlayerSpawn);
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_PLAYAGAIN, ReceiverEventPlayerAGain);
        }
    }
    protected override void Awake()
    {
        base.Awake();
        //GetChildNPC();
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;   
#endif
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void ShowMenuLoading()
    {
        m_GameState = GAMESTATE.MENULOADING;
        if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenMenuPanel>();
        }
    }
    public void ShowBoardPlayerStats()
    {
        if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<PopupCharacterPanel>();
            UIManager.Instance.HidePopup<PopupCharacterPanel>();
        }
    }

     private void StartGame()
    {
        ShowMenuLoading();
    }
    private  void ReceiverEventPlayerAGain(object value)
    {
        Debug.Log($"ReceiverEventPlayerAGain : called");
        PlayerManager.instance.playerHeal.ResetPlayerHeal();
        PlayerManager.instance.playerMana.ResetMana();
        PlayerManager.instance.playerStamina.ResetStamina();
        PlayerManager.instance.playerAnim.ResetAnimPlayerIdle();
        PlayerManager.instance.controller.enabled = false;
        PlayerManager.instance.transform.SetPositionAndRotation(m_SpawnPlayer.position, m_SpawnPlayer.rotation);
        PlayerManager.instance.controller.enabled = true;
        PlayerManager.instance.playerMove.canMove = true;
        PlayerManager.instance.playerMove.ResetInput();
        PlayerManager.instance.playerDamage.RegisterEventAttack();
        PlayerManager.instance.ChangeStatePlayer(PlayerManager.PlayerState.idle);
        //ResetUi();
    }

    private void ResetUi()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.RemoveScreen<ScreenOriginalScrollBtn>();
            UIManager.Instance.RemoveScreen<ScreenIconInventory>();
            UIManager.Instance.RemoveScreen<ScreenBookSkill>();
        } 
            
    }    
    private void ReceiverEventPosPlayerSpawn(object value)
    {
        if(value != null)
        {
            if(value is Transform transform)
            {
                m_SpawnPlayer = transform;
            }    
        }
    }

   
}
