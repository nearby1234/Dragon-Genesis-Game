using UnityEngine;


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
    //[SerializeField] private GameObject m_Player;

    //[SerializeField] private List<GameObject> npcList = new();

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
        StateMenuLoading();
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
    public void StateMenuLoading()
    {
        m_GameState = GAMESTATE.MENULOADING;
        if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenMenuPanel>();
        }
    }
   
}
