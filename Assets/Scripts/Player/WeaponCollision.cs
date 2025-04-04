using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    //[SerializeField] private GameObject m_BloodPrehabs;
    [SerializeField] private GameObject m_HitPrehabs;
    [SerializeField] private bool m_IsTakeDamaged;
    [SerializeField] private float m_timer;
    private void OnTriggerEnter(Collider other)
    {
        // Ch? x? lý n?u va ch?m v?i enemy ki?u Creep ho?c Boss
        if (!other.gameObject.CompareTag("Creep") && !other.gameObject.CompareTag("Boss"))
            return;

        // N?u player ?ang ? tr?ng thái idle thì không gây damage
        if (PlayerManager.instance.m_PlayerState.Equals(PlayerManager.PlayerState.idle))
            return;

        // N?u enemy ?ã b? damage trong ?òn này, b? qua
        if (m_IsTakeDamaged)
            return;

        int damage = PlayerManager.instance.playerDamage.GetPlayerDamage();

        if (other.gameObject.CompareTag("Creep"))
        {
            EnemyHeal enemyHeal = other.GetComponentInParent<EnemyHeal>();
            if (enemyHeal != null)
            {
                enemyHeal.ReducePlayerHealth(damage);
                if(EffectManager.HasInstance)
                {
                   
                    Vector3 closepoint = other.ClosestPoint(other.transform.position);
                    GameObject textDamage = Instantiate(EffectManager.Instance.GetPrefabs("DamageText"), other.transform.position, other.transform.rotation);

                    textDamage.TryGetComponent<SetupTextDamage>(out var damageText);
                    if (damageText != null)
                    {
                        damageText.ChangeTextDamage(damage, closepoint);
                    }
                }
                CameraManager.Instance.ShakeCamera();
            }
        }
        else if (other.gameObject.CompareTag("Boss"))
        {
            if (other.TryGetComponent<WormBoss>(out var wormBoss))
            {
                wormBoss.GetDamage(damage);
                if (EffectManager.HasInstance)
                {
                    Vector3 closepoint = other.ClosestPoint(other.transform.position);
                    GameObject textDamage = Instantiate(EffectManager.Instance.GetPrefabs("DamageText"), other.transform.position, other.transform.rotation);
                    textDamage.TryGetComponent<SetupTextDamage>(out var damageText);
                    if (damageText != null)
                    {
                        damageText.ChangeTextDamage(damage, closepoint);
                    }
                }
                CameraManager.Instance.ShakeCamera();
            }
        }

        // ?ánh d?u ?ã gây damage r?i và reset flag sau m?t kho?ng th?i gian nh?t ??nh
        m_IsTakeDamaged = true;
        StartCoroutine(ResetDamageFlag());

        SpawnHitPrehabs(other);
    }

    private void SpawnHitPrehabs(Collider other)
    {
        Vector3 hitPos = other.ClosestPoint(transform.position);
        GameObject HitFX = Instantiate(m_HitPrehabs, hitPos, Quaternion.identity);
        HitFX.SetActive(true);
        Destroy(HitFX, 1f);
    }
    private IEnumerator ResetDamageFlag()
    {
        yield return new WaitForSeconds(m_timer);
        m_IsTakeDamaged = false;
    }

}

