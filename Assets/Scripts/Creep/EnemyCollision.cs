using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Sword"))
        {
            Debug.Log($"Collision with {other.gameObject.name} ");
        }
    }
}
