using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerStat", menuName = "Scriptable Object/PlayerStatSO")]
public class PlayerStatSO : ScriptableObject
{
    public int m_PlayerHeal;
    public int m_PlayerMana;
    public int m_PlayerStamina;
}
