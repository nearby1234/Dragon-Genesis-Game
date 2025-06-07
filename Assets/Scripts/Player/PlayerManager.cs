using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : BaseManager<PlayerManager>
{
    private bool m_IsShowLosePopup = false;
    private bool m_CancelLosePopup = false;
    private Coroutine _delayLoseCoroutine;
    [SerializeField] private InputAction m_EscButton;
    public enum PlayerState
    {
        idle,
        attack,
        hit,
    }
    public static PlayerManager instance;

    public CharacterController controller;
    public PlayerMove playerMove;
    public PlayerJump playerJump;
    public PlayerDamage playerDamage;
    public PlayerAnim playerAnim;
    public PlayerHeal playerHeal;
    public PlayerMana playerMana;
    public PlayerStamina playerStamina;

    public PlayerCasting playerCasting;
    public PlayerDodge playerDodge;
    public EffectSpawn effectSpawn;
    public PlayerState m_PlayerState;

    [Header("Player Stat")]
    [SerializeField] private PlayerStatSO m_PlayerStatSO;

    public PlayerStatSO PlayerStatSO => m_PlayerStatSO;

    public bool isInteractingWithUI;
    public bool m_IsShowingLosePopup;

    protected override void Awake()
    {
        base.Awake();
        if (instance == null) instance = this;
        else Destroy(gameObject);

        CacheComponents();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerDamage.RegisterEventAttack();
        ChangeStatePlayer(PlayerState.idle);
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_DIE, ReceiverPlayerDie);
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_MAINMENU, _ => CancelLoseCoroutine());
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_PLAYAGAIN, _ => CancelLoseCoroutine());
            ListenerManager.Instance.Register(ListenType.UI_CLICK_SHOWUI, ReceiverEventClickShowUI);
            ListenerManager.Instance.Register(ListenType.UI_DISABLE_SHOWUI, ReceiverEvenDisableShowUI);
        }
        m_EscButton.Enable();
        m_EscButton.performed += (ctx) => OnEscPerfomed();
    }

    private void OnDestroy()
    {
        m_EscButton.performed -= (ctx) => OnEscPerfomed();
        m_EscButton.Disable();
    }
    private void OnDisable()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_DIE, ReceiverPlayerDie);
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_MAINMENU, _ => CancelLoseCoroutine());
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_PLAYAGAIN, _ => CancelLoseCoroutine());
            ListenerManager.Instance.Unregister(ListenType.UI_CLICK_SHOWUI, ReceiverEventClickShowUI);
            ListenerManager.Instance.Unregister(ListenType.UI_DISABLE_SHOWUI, ReceiverEvenDisableShowUI);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (playerHeal.GetPlayerDeath()) return;
        if (isInteractingWithUI) return;
        switch (m_PlayerState)
        {
            case PlayerState.idle:
                HandleIdleState();
                break;
            case PlayerState.attack:
                HandleAttackState();
                break;
        }

    }

    public void ChangeStatePlayer(PlayerState playerState)
    {
        m_PlayerState = playerState;
    }
    private void OnEscPerfomed()
    {
        if (m_IsShowingLosePopup) return;
        var msg = new PopupMessage()
        {
            popupType = PopupType.PAUSE,
            OnResume = () =>
            {
                Time.timeScale = 1f;
             
            },
            OnMainMenu = () =>
            {
                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.CLICK_BUTTON_MAINMENU, null);
                  
                }
                Time.timeScale = 1f;
                //StopAllCoroutines();
                SceneManager.LoadScene("Menu");
                GameManager.Instance.GameState = GAMESTATE.MENULOADING;
            },
        };
        if(GameManager.HasInstance)
        {
            GameManager.Instance.ShowCursor();
        }
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<LosePopup>(msg, true);
        }
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ClickSound");
        }
        Time.timeScale = 0;
    }
    private void ReceiverEventClickShowUI(object value)
    {
        playerDamage.DegreeEventClickMouse();
        if (CameraManager.HasInstance)
        {
            CameraManager.Instance.SetActiveInputAxisController(false);
        }
        isInteractingWithUI = true;
    }
    private void ReceiverEvenDisableShowUI(object value)
    {
        playerDamage.RegisterEventAttack();
        if (CameraManager.HasInstance)
        {
            CameraManager.Instance.SetActiveInputAxisController(true);
        }
        isInteractingWithUI = false;
    }

    private void CacheComponents()
    {
        controller = GetComponent<CharacterController>();
        playerMove = GetComponent<PlayerMove>();
        playerJump = GetComponent<PlayerJump>();
        playerAnim = GetComponent<PlayerAnim>();
        effectSpawn = GetComponent<EffectSpawn>();
        playerDamage = GetComponent<PlayerDamage>();
        playerHeal = GetComponent<PlayerHeal>();
        playerMana = GetComponent<PlayerMana>();
        playerStamina = GetComponent<PlayerStamina>();
        playerCasting = GetComponent<PlayerCasting>();
        playerDodge = GetComponent<PlayerDodge>();
    }
    private void HandleIdleState()
    {
        playerMove.PlayerMovement();
        playerJump.PlayerJumpUp();
        //playerCamera.RotationPlayer();
        playerCasting.Casting();
    }
    private void HandleAttackState()
    {
        //playerDamage.Attack();
    }

    
    private IEnumerator DelayShowLosePopup()
    {
        yield return new WaitForSeconds(3f);
        // Nếu đã hủy từ trước, dừng luôn
        if (m_CancelLosePopup)
        {
            yield break;
        }
        if (UIManager.HasInstance)
        {
            string hex = "#FF0000";
            if (ColorUtility.TryParseHtmlString(hex, out Color parsed))
            {
                var message = new PopupMessage()
                {
                    popupType = PopupType.LOSE,
                    OnPlayAgain = () =>
                    {
                        if (ListenerManager.HasInstance)
                        {
                            if(GameManager.HasInstance)
                            {
                                ListenerManager.Instance.BroadCast(ListenType.CLICK_BUTTON_PLAYAGAIN, null);
                            }    
                           
                        }
                        var fakeLoadingSetting = new FakeLoadingSetting();
                        UIManager.Instance.ShowPopup<PopupFakeLoading>(fakeLoadingSetting, true);
                    },
                    OnMainMenu = () =>
                    {
                        if (ListenerManager.HasInstance)
                        {
                            ListenerManager.Instance.BroadCast(ListenType.CLICK_BUTTON_MAINMENU, null);
                        }

                        //StopAllCoroutines();
                        SceneManager.LoadScene("Menu");
                        GameManager.Instance.GameState = GAMESTATE.MENULOADING;
                    },

                };
                UIManager.Instance.ShowPopup<LosePopup>(message, true);
            }

            if (GameManager.HasInstance)
            {
                GameManager.Instance.ShowCursor();
            }
        }
    }
    private void ReceiverPlayerDie(object value)
    {
        if (value is bool died && died && !m_IsShowLosePopup)
        {
            m_IsShowLosePopup = true;
            m_CancelLosePopup = false;
            _delayLoseCoroutine = StartCoroutine(DelayShowLosePopup());

            // Tắt attack
            playerDamage.DegreeEventClickMouse();
        }
    }
    private void CancelLoseCoroutine()
    {
        // Dùng để hủy coroutine chờ popup
        m_CancelLosePopup = true;
        m_IsShowLosePopup = false;
        if (_delayLoseCoroutine != null)
        {
            StopCoroutine(_delayLoseCoroutine);
            _delayLoseCoroutine = null;
        }
        playerDamage.RegisterEventAttack();
    }
}
