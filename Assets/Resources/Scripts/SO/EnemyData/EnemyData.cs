using UnityEngine;
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Scriptable Object/Data/Enemy Data")]
public class EnemyData : ScriptableObject, IEnumKeyed<EnemyType>
{
    public EnemyType Key => enemyType;
    public EnemyType enemyType;
    public int m_PhysicsDamage;
    public int m_MagicDamage;
    public int m_EffectDamage;
}
