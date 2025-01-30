using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.5f; // Th?i gian t?n t?i c?a hi?u ?ng

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}