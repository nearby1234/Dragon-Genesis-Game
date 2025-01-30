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
    public EffectSpawn effectSpawn;
    public PlayerState m_PlayerState;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        controller = GetComponent<CharacterController>();
        playerMove = GetComponent<PlayerMove>();
        playerJump = GetComponent<PlayerJump>();
        playerAnim = GetComponent<PlayerAnim>();
        effectSpawn = GetComponent<EffectSpawn>();
        playerDamage = GetComponent<PlayerDamage>();
        playerHeal = GetComponent<PlayerHeal>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_PlayerState = PlayerState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        playerMove.PlayerMovement();
        playerJump.PlayerJumpUp();
    }

    public void ChangeStatePlayer(PlayerState playerState)
    {
        m_PlayerState = playerState;
    }
}
