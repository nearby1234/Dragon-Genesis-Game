using UnityEngine;

public class SpawnPosPlayAgain : MonoBehaviour
{
    [SerializeField] private CreepType creepType;
    public CreepType CreepType => creepType;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(Cheat.HasInstance)
            {
                Cheat.Instance.GetCheckPoint(creepType);
            }
        }
    }
  
}
