using UnityEngine;


[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Scriptable Object/Data/CharacterData")]
public class CharacterData : ScriptableObject , IEnumKeyed<CharacterType>
{
    public CharacterType Key => characterType;
    public CharacterType characterType;
    public int health;
    public int damage;
}
