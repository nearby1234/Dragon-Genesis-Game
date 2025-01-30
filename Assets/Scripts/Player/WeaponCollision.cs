using Unity.Mathematics;
using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    [SerializeField] private GameObject m_BloodPrehabs;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Creep"))
        {
            if (PlayerManager.instance.m_PlayerState.Equals(PlayerManager.PlayerState.idle)) return;
            Vector3 hitPos = other.ClosestPoint(transform.position);
            GameObject BloodFX = Instantiate(m_BloodPrehabs, hitPos, Quaternion.identity);
        }
    }
}
  