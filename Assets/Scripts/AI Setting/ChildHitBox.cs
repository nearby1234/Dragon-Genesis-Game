using NUnit.Compatibility;
using System.Collections;
using UnityEngine;

public class ChildHitBox : MonoBehaviour
{
    [SerializeField] private TypeCollider typeCollider;
    public TypeCollider TypeCollider
    {
        get => typeCollider;
        set => typeCollider = value;
    }
    private bool IsCollisionParticle;

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
    private void OnParticleCollision(GameObject other)
    {
        if(!IsCollisionParticle)
        {
            Debug.Log($"{this.gameObject.name}");
            if (ListenerManager.HasInstance)
            {
                ListenerManager.Instance.BroadCast(ListenType.PARTICLE_TRIGGER, (typeCollider, other));
            }
            IsCollisionParticle = true;
        }
        StartCoroutine(SetStateIsCollision());
    }

    IEnumerator SetStateIsCollision()
    {
        yield return new WaitForSeconds(0.8f);
        IsCollisionParticle = false;
    }
 
    
}
