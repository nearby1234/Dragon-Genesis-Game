using System.Collections;
using Unity.Cinemachine;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class ChildTriggerForwarder : MonoBehaviour
{
    private PivotScaleWeapon pivotScaleWeapon;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private bool m_IsEnergyTakeDamaged = false;
    private void Awake()
    {
        pivotScaleWeapon = GetComponentInParent<PivotScaleWeapon>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            WormBoss wormBoss = GetComponentInParent<WormBoss>();
            if (wormBoss != null)
            {
                if (!m_IsEnergyTakeDamaged)
                {
                    wormBoss.GetDamage(pivotScaleWeapon.EnergyWeaponDamage);
                    GameManager.Instance.ShakeCamera();
                    m_IsEnergyTakeDamaged = true;
                    Vector3 closePoint = other.ClosestPoint(other.transform.position);
                    if(EffectManager.HasInstance)
                    {
                        GameObject textDamage = Instantiate(EffectManager.Instance.GetPrefabs("DamageText"),other.transform.position,other.transform.rotation);
                        if (textDamage.TryGetComponent<SetupTextDamage>(out var setupTextDamage))
                        {
                            setupTextDamage.ChangeTextDamage(pivotScaleWeapon.EnergyWeaponDamage, closePoint);
                        }
                    }
                    pivotScaleWeapon.AeSetupEffect(other.transform.position);
                    StartCoroutine(pivotScaleWeapon.TurnOffExplosion());
                    StartCoroutine(TimeSetDefaultEnergyWeapon());
                }

            }
        }
        else if (other.CompareTag("Creep"))
        {
            EnemyHeal enemyHeal = other.GetComponentInParent<EnemyHeal>();
            if (enemyHeal != null)
            {
                if (!m_IsEnergyTakeDamaged)
                {
                    enemyHeal.ReducePlayerHealth(pivotScaleWeapon.EnergyWeaponDamage);
                    GameManager.Instance.ShakeCamera();
                    m_IsEnergyTakeDamaged = true;
                    Vector3 closePoint = other.ClosestPoint(other.transform.position);
                    if (EffectManager.HasInstance)
                    {
                        GameObject textDamage = Instantiate(EffectManager.Instance.GetPrefabs("DamageText"), other.transform.position, other.transform.rotation);
                        if (textDamage.TryGetComponent<SetupTextDamage>(out var setupTextDamage))
                        {
                            setupTextDamage.ChangeTextDamage(pivotScaleWeapon.EnergyWeaponDamage, closePoint);
                        }
                    }
                    pivotScaleWeapon.AeSetupEffect(other.transform.position);
                    StartCoroutine(pivotScaleWeapon.TurnOffExplosion());
                    StartCoroutine(TimeSetDefaultEnergyWeapon());
                }
            }
        }
    }

    private IEnumerator TimeSetDefaultEnergyWeapon()
    {
        yield return new WaitForSeconds(1f);
        m_IsEnergyTakeDamaged = false;
    }

   
}
