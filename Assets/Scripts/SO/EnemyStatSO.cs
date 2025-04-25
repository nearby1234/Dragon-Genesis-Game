using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyStat",menuName = "Scriptable Object/EnemyStatSO")]
public class EnemyStatSO : ScriptableObject
{
    public CreepType creepType;
    public int heal;
    public int damage;
}
