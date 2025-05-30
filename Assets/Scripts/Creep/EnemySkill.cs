using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using PilotoStudio;

public class EnemySkill : MonoBehaviour
{
    [SerializeField] private Transform m_SpawnEffect;
    [SerializeField] private List<GameObject> fxSpawn = new();
    [SerializeField] private ParticleHandler m_ParticleHandler;
    [SerializeField] private WormBoss m_WormBoss;
    [SerializeField] private float m_damageFireBall;
    private const string m_AnimationNamePhase1 = "Attack03";
    private const string m_AnimationNamePhase2 = "Attack04";
    private void Awake()
    {
        m_WormBoss = GetComponent<WormBoss>();
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.BOSS_SENDER_DAMAGED, ReceiverBossDamageFireBall);
        }
    }
    public void CastFireBall(int index)
    {
        if (fxSpawn != null)
        {
            fxSpawn[index].SetActive(true);
            fxSpawn[index].transform.SetPositionAndRotation(m_SpawnEffect.position, m_SpawnEffect.rotation);
            PlayEffect(index);
            if(AudioManager.HasInstance)
            {
                AudioManager.Instance.PlaySE("DragonFire");
            }
        }
    }
    private void PlayEffect(int index)
    {
        if (fxSpawn != null)
        {
            if (fxSpawn[index].TryGetComponent<ParticleSystem>(out var fx))
            {
                fx.Stop();
                fx.Play();
            }

        }
    }

    public void SetupBlackHole()
    {
        m_ParticleHandler.gameObject.SetActive(true);
        m_ParticleHandler.Cast();
        
    }
    public IEnumerator StopFireBall(int index)
    {
        yield return new WaitForSeconds(3f);
        fxSpawn[index].SetActive(false);
    }

    public void HandleParticleCollision( GameObject other, List<ParticleCollisionEvent> collisionEvents)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHeal>().ReducePlayerHeal((int)m_damageFireBall * collisionEvents.Count,TypeCollider.UnKnown);
            if(AudioManager.HasInstance)
            {
                AudioManager.Instance.PlaySE("FireExplosion");
                AudioManager.Instance.PlaySE("PlayerHit");
            }
        }
    }

    private void ReceiverBossDamageFireBall(object value)
    {
        if (value != null)
        {
            if (m_WormBoss != null)

            {
                if (value is (int currentAttackIndex, List<WormAttackData> attackDataList))
                {
                    if (attackDataList[currentAttackIndex].animationName.Equals(m_AnimationNamePhase1)
                        || attackDataList[currentAttackIndex].animationName.Equals(m_AnimationNamePhase2))
                    {
                        m_damageFireBall = attackDataList[currentAttackIndex].CalculateDamage(m_WormBoss);
                    }
                }
            }
        }
    }
}
