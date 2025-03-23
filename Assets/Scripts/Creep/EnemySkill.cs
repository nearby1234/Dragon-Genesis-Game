using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemySkill : MonoBehaviour
{
    [SerializeField] private Transform m_SpawnEffect;
    [SerializeField] private List<GameObject> fxSpawn = new();
    public void CastFireBall(int index)
    {
        if (fxSpawn != null)
        {
            fxSpawn[index].SetActive(true);
            fxSpawn[index].transform.SetPositionAndRotation(m_SpawnEffect.position, m_SpawnEffect.rotation);
            PlayEffect(index);
        }
        else Debug.Log("Miss FireFX");
    }
    private void PlayEffect(int index)
    {
        if (fxSpawn != null)
        {
            if(fxSpawn[index].TryGetComponent<ParticleSystem>(out var fx))
            {
                fx.Stop();
                fx.Play();
            }
            
        }
    }
    public IEnumerator StopFireBall(int index )
    {
        yield return new WaitForSeconds(3f);
        fxSpawn[index].SetActive(false);
    }

    public void HandleParticleCollision(GameObject fxObj, GameObject other, List<ParticleCollisionEvent> collisionEvents)
    {
        Debug.Log($"Particle System '{fxObj.name}' collided with '{other.name}'. S? l??ng collision events: {collisionEvents.Count}");
        // X? lý logic va ch?m ? ?ây
    }
}
