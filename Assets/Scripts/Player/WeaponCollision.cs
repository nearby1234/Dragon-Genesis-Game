using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Creep"))
        {
            if (PlayerManager.instance.m_PlayerState.Equals(PlayerManager.PlayerState.idle)) return;
            //PlayerManager.instance.effectSpawn.SpawnBlood();
        }
    }
}
