using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyStat",menuName = "Scriptable Object/EnemyStatSO")]
public class EnemyStatSO : ScriptableObject
{
    public int heal;
    public int damage;
}
