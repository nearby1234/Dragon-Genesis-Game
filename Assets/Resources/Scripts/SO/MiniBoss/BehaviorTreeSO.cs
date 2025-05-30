using UnityEngine;

[CreateAssetMenu(fileName ="New Data Setitng",menuName = "Behavior Tree SO")]
public class BehaviorTreeSO : ScriptableObject , IEnumKeyed<EnemyType>
{
    public EnemyType Key => enemyType;
    public EnemyType enemyType;

    public float WanderRadius = 30f;
    public float SpeedAgent = 3.5f;
    public float SpeedWalk;
    public float SpeedRun;

    [Header("AttackSetting")]
    public float RangeAttackJump;
    public float RangeAttackSword;
    public float RangeAttackThrow;

    [Header("Atribute BullTank")]
    public float Heal;
    public float DamageBase;

    [Header("Percent Attack")]
    public float percentAttackAxe;
    public float percentAttackThrowAxe;
    public float percentAttackJump;

    [Header("Percent Each Phase")]
    public float PercentPhaseSecond;
    public float PercentPhaseThird;

    [Header("Range Throw Axe each Phase")]
    public float rangeFirstPhase;
    public float rangeSecondPhase;
    public float rangeThirdPhase;

   
}
