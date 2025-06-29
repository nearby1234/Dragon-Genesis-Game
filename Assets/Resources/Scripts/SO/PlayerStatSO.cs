using UnityEngine;



public enum PlayerType
{
    Player,
}
[CreateAssetMenu(fileName = "New PlayerStat", menuName = "Scriptable Object/PlayerStatSO")]
public class PlayerStatSO : ScriptableObject , IEnumKeyed<PlayerType>
{
    public int m_PlayerHeal;
    public int m_PlayerMana;
    public int m_PlayerStamina;
    public int m_PlayerDamage;
    public int m_PlayerArmor;

    [Header("Setting Move")]
    public float speedJogging;
    public float speedRun;

    [Header("Setting Attack")]
    public float timeResetFlagAttack;

    [Header("Setting Admin")]
    public string passwordAdmin;
    public int maxLengthPassword = 4;
    public PlayerType Key => playerType;
    public PlayerType playerType;
}
