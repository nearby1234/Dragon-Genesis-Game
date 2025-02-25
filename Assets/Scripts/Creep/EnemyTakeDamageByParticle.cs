using UnityEngine;

public class EnemyTakeDamageByParticle : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ground"))
        {
            Debug.Log("Fireball hit Player!");
            
        }
    }
}
