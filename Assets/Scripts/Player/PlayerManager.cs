using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
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
    public PlayerCamera playerCamera;
    public PlayerCasting playerCasting;
    public PlayerDodge playerDodge;
    public EffectSpawn effectSpawn;
    public PlayerState m_PlayerState;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        CacheComponents();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeStatePlayer(PlayerState.idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHeal.GetPlayerDeath())
        {
            playerDamage.DegreeEventClickMouse();
            StartCoroutine(DelayHideAnimator());
            return;
        }
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
        playerCamera = GetComponent<PlayerCamera>();
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
        playerDamage.Attack();
    }

    IEnumerator DelayHideAnimator()
    {
        yield return new WaitForSeconds(3f);
        playerAnim.GetAnimator().enabled = false;
    }
}
