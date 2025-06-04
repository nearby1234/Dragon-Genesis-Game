using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    //[SerializeField] private GameObject m_BloodPrehabs;
    [SerializeField] private GameObject m_HitPrehabs;
    [SerializeField] private bool m_IsTakeDamaged;
    [SerializeField] private float m_timer;
    [SerializeField] private List<string> tagList;
    private void OnTriggerEnter(Collider other)
    {
        string nameTag = other.gameObject.tag;
        if (!tagList.Contains(nameTag)) return;

        //// Ch? x? lý n?u va ch?m v?i enemy ki?u Creep ho?c Boss
        //if (!other.gameObject.CompareTag("Creep") && !other.gameObject.CompareTag("Boss"))
        //    return;

        // N?u player ?ang ? tr?ng thái idle thì không gây damage
        if (PlayerManager.instance.m_PlayerState.Equals(PlayerManager.PlayerState.idle))
            return;

        // N?u enemy ?ã b? damage trong ?òn này, b? qua
        if (m_IsTakeDamaged)
            return;
        int damage = PlayerManager.instance.playerDamage.GetPlayerDamage();

        if (PlayerManager.instance.playerDamage.Heavyattack)
        {
            Debug.Log($"Heavyattack :{PlayerManager.instance.playerDamage.Heavyattack}");
            int baseDamage = damage;
            int bonus = Mathf.RoundToInt(baseDamage * 1.15f);
            damage = bonus;
            Debug.Log($"damage : {damage}");
            Debug.Log($"bonus :{bonus}");
        }

        switch (other.gameObject.tag)
        {
            case "Creep":
                {
                    EnemyHeal enemyHeal = other.GetComponentInParent<EnemyHeal>();
                    if (enemyHeal != null)
                    {
                        enemyHeal.ReducePlayerHealth(damage);
                    }
                    ShowDamageText(0, other, damage);
                    CameraManager.Instance.ShakeCamera();
                    if (AudioManager.HasInstance)
                    {
                        AudioManager.Instance.PlaySE("attaccolidersound");
                    }
                }
                break;
            case "Boss":
                {
                    if (other.TryGetComponent<WormBoss>(out var wormBoss))
                    {
                        wormBoss.GetDamage(damage);
                    }
                    ShowDamageText(1, other, damage);
                    CameraManager.Instance.ShakeCamera();
                    if (AudioManager.HasInstance)
                    {
                        AudioManager.Instance.PlaySE("WormBossHit");
                        AudioManager.Instance.PlaySE("attaccolidersound");

                    }
                }
                break;
            case "BullTank":
                {
                    if(other.TryGetComponent<BullTankHeal>(out var bullTankHeal))
                    {
                        bullTankHeal.ReduceHeal(damage);
                    }
                    ShowDamageText(3, other, damage);
                    CameraManager.Instance.ShakeCamera();
                    if (AudioManager.HasInstance)
                    {
                        AudioManager.Instance.PlaySE("BullTankHit");
                        AudioManager.Instance.PlaySE("attaccolidersound");
                    }
                }
                break;

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
    private void ShowDamageText(int offSetPosZ, Collider collider, int damage)
    {
        if (EffectManager.HasInstance)
        {
            Vector3 closepoint = collider.ClosestPoint(collider.transform.position);
            Vector3 newClosePoint = closepoint + Vector3.up * offSetPosZ;
            GameObject textDamage = Instantiate(EffectManager.Instance.GetPrefabs("DamageText"), collider.transform.position, collider.transform.rotation);
            textDamage.TryGetComponent<SetupTextDamage>(out var damageText);
            if (damageText != null)
            {
                damageText.ChangeTextDamage(damage, newClosePoint);
            }
        }
    }

}

