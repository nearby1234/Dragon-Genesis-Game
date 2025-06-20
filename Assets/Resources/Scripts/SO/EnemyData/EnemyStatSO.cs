using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "New EnemyStat",menuName = "Scriptable Object/EnemyStatSO")]
public class EnemyStatSO : ScriptableObject , IEnumKeyed<EnemyType>
{
    public CreepType creepType;
    public EnemyType Key => enemyType;
    public EnemyType enemyType;
    public int heal;
    public int damage;
    public int m_PhysicsDamage;
    public int m_MagicDamage;
    public int m_EffectDamage;
    public int amountEnemyDeath;

    [Header("Else Setting")]
    [ShowIf("@creepType == CreepType.BullTank")]
    public float distanceAttackJump;
    [ShowIf("@creepType == CreepType.BullTank")]
    public float distanceAttackSword;
    [ShowIf("@creepType == CreepType.BullTank")]
    public float speedWalk;
    
    [ShowIf("@creepType == CreepType.WORM")]
    public float percentHealTranslatePhase; 
    [ShowIf("@creepType == CreepType.WORM")]
    public float timeWaitUnderGround;


}
