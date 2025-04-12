using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerManager : BaseManager<PlayerManager>
{
    public enum PlayerState
    {
        idle,
        attack
    }
    public static PlayerManager instance;

    public CharacterController controller;
    public PlayerMove playerMove;
    public PlayerJump playerJump;
    public PlayerDamage playerDamage;
    public PlayerAnim playerAnim;
    public PlayerHeal playerHeal;
    public PlayerCasting playerCasting;
    public PlayerDodge playerDodge;
    public EffectSpawn effectSpawn;
    public PlayerState m_PlayerState;

    [Header("Player Stat")]
    [SerializeField] private PlayerStatSO m_PlayerStatSO;
   
    public PlayerStatSO PlayerStatSO => m_PlayerStatSO;

    private bool m_IsShowLosePopup = false;
    private bool m_IsTalkingNPC;
    public bool isInteractingWithUI;

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
        ChangeStatePlayer(PlayerState.idle);
        if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYER_IS_TALKING_NPC,ReceiverValueIsTalkingNPC);
        }
    }
    private void OnDisable()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYER_IS_TALKING_NPC, ReceiverValueIsTalkingNPC);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (playerHeal.GetPlayerDeath())
        {
            if(!m_IsShowLosePopup)
            {
                StartCoroutine(DelayShowLosePopup());
                m_IsShowLosePopup = true;
            }
            playerDamage.DegreeEventClickMouse();
            StartCoroutine(DelayHideAnimator());
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            playerDamage.DegreeEventClickMouse();
            if(CameraManager.HasInstance)
            {
                CameraManager.Instance.SetActiveInputAxisController(false);
            }
            return;
        }
        else
        {
            playerDamage.RegisterEventAttack();
            if (CameraManager.HasInstance)
            {
                CameraManager.Instance.SetActiveInputAxisController(true);
            }
        }
        if (m_IsTalkingNPC) return;
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
    
    private void CacheComponents()
    {
        controller = GetComponent<CharacterController>();
        playerMove = GetComponent<PlayerMove>();
        playerJump = GetComponent<PlayerJump>();
        playerAnim = GetComponent<PlayerAnim>();
        effectSpawn = GetComponent<EffectSpawn>();
        playerDamage = GetComponent<PlayerDamage>();
        playerHeal = GetComponent<PlayerHeal>();
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

    IEnumerator DelayHideAnimator()
    {
        yield return new WaitForSeconds(3f);
        playerAnim.GetAnimator().enabled = false;
    }
    private IEnumerator DelayShowLosePopup()
    {
        yield return new WaitForSeconds(3f);
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ShowPopup<LosePopup>();
        }
    }
    private void ReceiverValueIsTalkingNPC(object value)
    {
        if(value != null)
        {
            if(value is bool isTalking)
            {
                m_IsTalkingNPC = isTalking;
            }
        }
    }

}
