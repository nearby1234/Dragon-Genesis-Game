using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStat",menuName = "New Enemy Stat")]
public class EnemyStatSO : ScriptableObject
{
    public int heal;
    public int damage;
}
