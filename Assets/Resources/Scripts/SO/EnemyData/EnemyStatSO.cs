using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyStat",menuName = "Scriptable Object/EnemyStatSO")]
public class EnemyStatSO : ScriptableObject , IEnumKeyed<EnemyType>
{
    public CreepType creepType;
    public int heal;
    public int damage;
    public int m_PhysicsDamage;
    public int m_MagicDamage;
    public int m_EffectDamage;

    public EnemyType Key => enemyType;
    public EnemyType enemyType;
}
