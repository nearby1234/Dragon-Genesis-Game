using UnityEngine;
using System.Collections.Generic;

public class EnemySkill : MonoBehaviour
{
    [SerializeField] private Transform m_SpawnEffect;
    [SerializeField] private ParticleSystem activeFireBall;
    public void CastFireBall()
    {
        if (activeFireBall != null)
        {
            activeFireBall.gameObject.SetActive(true);
            activeFireBall.transform.SetPositionAndRotation(m_SpawnEffect.position, m_SpawnEffect.rotation);
            PlayEffect();
        }
        else Debug.Log("Miss FireFX");
    }
    private void PlayEffect()
    {
        if (activeFireBall != null)
        {
            activeFireBall.Stop();
            activeFireBall.Play();  
        }
    }
}
