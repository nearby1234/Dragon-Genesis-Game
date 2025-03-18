using UnityEngine;
using System.Collections.Generic;

public class EnemySkill : MonoBehaviour
{
    [SerializeField] private Transform m_SpawnEffect;
    [SerializeField] private List<GameObject> fxSpawn = new();
    public void CastFireBall()
    {
        if (fxSpawn != null)
        {
            fxSpawn[0].SetActive(true);
            fxSpawn[0].transform.SetPositionAndRotation(m_SpawnEffect.position, m_SpawnEffect.rotation);
            PlayEffect();
        }
        else Debug.Log("Miss FireFX");
    }
    private void PlayEffect()
    {
        if (fxSpawn != null)
        {
            if(fxSpawn[0].TryGetComponent<ParticleSystem>(out var fx))
            {
                fx.Stop();
                fx.Play();
            }
            
        }
    }
}
