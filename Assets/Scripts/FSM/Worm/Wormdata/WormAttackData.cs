using UnityEngine;

[System.Serializable]
public class WormAttackData
{
    public string animationName;
    public float stopDistance;
    public bool needFollowUp;
    public float Damage;
    public int damagePercent;
    public bool IsUpdateDamaged;
    public string nameWorm = "Sâu Hắc Ám";

    public float CalculateDamage(WormBoss boss)
    {
        float baseDamage = boss.m_WormDamage;
        Damage = baseDamage * (damagePercent / 100f);

        if (boss.IsRageState)
        {
            if (IsUpdateDamaged)
            {
                Damage = baseDamage * ((damagePercent * 2) / 100f);
            }
        }
        return Damage; 
    }
}
