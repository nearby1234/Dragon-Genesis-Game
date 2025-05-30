using NUnit.Compatibility;
using System.Collections.Generic;
using UnityEngine;

public class ChildHitBox : MonoBehaviour
{
    [SerializeField] private TypeCollider typeCollider;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.TYPECOLLIDER_CHILD, (typeCollider,other));
            }    
        }    
    }

    private void OnParticleTrigger()
    {
        Debug.Log("Tes trigget");
        ParticleSystem ps = GetComponent<ParticleSystem>();

        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
        int enterCount = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        if (enterCount > 0)
            Debug.Log($"Enter: {enterCount}");

        List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();
        int insideCount = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);
        if (insideCount > 0)
            Debug.Log($"Inside: {insideCount}");

        List<ParticleSystem.Particle> exit = new List<ParticleSystem.Particle>();
        int exitCount = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);
        if (exitCount > 0)
            Debug.Log($"Exit: {exitCount}");
    }
}
