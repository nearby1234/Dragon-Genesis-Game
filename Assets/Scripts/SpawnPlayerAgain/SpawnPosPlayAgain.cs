using UnityEngine;

public class SpawnPosPlayAgain : MonoBehaviour
{
    [SerializeField] private CreepType creepType;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(Cheat.HasInstance)
            {
                Cheat.Instance.SetAndSentEventPosPlayAgain(creepType);
            }
        }
    }
}
