using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ChildTriggerForwarder : MonoBehaviour
{
    #region Serialized Fields and References
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private bool isEnergyTakeDamaged = false;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private GameObject explosionPrefab;
    #endregion

    private PivotScaleWeapon pivotScaleWeapon;

    private void Awake()
    {
        pivotScaleWeapon = GetComponentInParent<PivotScaleWeapon>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            ProcessBossTrigger(other);
        }
        else if (other.CompareTag("Creep"))
        {
            ProcessCreepTrigger(other);
        }
    }

    #region Trigger Processing

    private void ProcessBossTrigger(Collider other)
    {
        WormBoss wormBoss = GetComponentInParent<WormBoss>();
        if (wormBoss != null && !isEnergyTakeDamaged)
        {
            wormBoss.GetDamage(pivotScaleWeapon.EnergyWeaponDamage);
            ExecuteCommonDamageEffects(other, instantiateExplosion: false);
        }
    }

    private void ProcessCreepTrigger(Collider other)
    {
        EnemyHeal enemyHeal = other.GetComponentInParent<EnemyHeal>();
        if (enemyHeal != null && !isEnergyTakeDamaged)
        {
            enemyHeal.ReducePlayerHealth(pivotScaleWeapon.EnergyWeaponDamage);
            ExecuteCommonDamageEffects(other, instantiateExplosion: true);
        }
    }

    /// <summary>
    /// Executes actions common to all triggers: camera shake, effects, and damage reset.
    /// </summary>
    /// <param name="other">Collider used to calculate hit position</param>
    /// <param name="instantiateExplosion">
    /// If true, the explosion prefab will be instantiated for the effect;
    /// otherwise, the existing explosion prefab (fetched via EffectManager) is used directly.
    /// </param>
    private void ExecuteCommonDamageEffects(Collider other, bool instantiateExplosion)
    {
        CameraManager.Instance.ShakeCamera();
        isEnergyTakeDamaged = true;

        // Calculate the closest point on the collider surface to set effect origin
        Vector3 closePoint = other.ClosestPoint(other.transform.position);

        // If an EffectManager exists, retrieve and instantiate the necessary effects.
        if (EffectManager.HasInstance)
        {
            // Re-acquire the prefabs so any update in the manager is captured.
            damageTextPrefab = EffectManager.Instance.GetPrefabs("DamageText");
            explosionPrefab = EffectManager.Instance.GetPrefabs("Explosion");

            // Instantiate and update the damage text effect.
            GameObject textDamage = Instantiate(damageTextPrefab, other.transform.position, other.transform.rotation);
            if (textDamage.TryGetComponent<SetupTextDamage>(out var setupTextDamage))
            {
                setupTextDamage.ChangeTextDamage(pivotScaleWeapon.EnergyWeaponDamage, closePoint);
            }

            GameObject explosionInstance = Instantiate(explosionPrefab, other.transform.position, other.transform.rotation);
            PlayParticleEffect(explosionInstance, closePoint);

        }

        StartCoroutine(ResetEnergyDamageFlag());
    }

    /// <summary>
    /// Helper method to attempt to play a ParticleSystem if available on the given GameObject.
    /// </summary>
    /// <param name="effectObject">The prefab or instance containing the ParticleSystem component.</param>
    /// <param name="position">Position to set before playing the particle system.</param>
    private void PlayParticleEffect(GameObject effectObject, Vector3 position)
    {
        if (effectObject != null && effectObject.TryGetComponent<ParticleSystem>(out var particleSystem))
        {
            particleSystem.transform.position = position;
            particleSystem.Play();
        }
    }

    /// <summary>
    /// Resets the damage flag after a delay so that the energy weapon can apply damage again.
    /// </summary>
    private IEnumerator ResetEnergyDamageFlag()
    {
        yield return new WaitForSeconds(1f);
        isEnergyTakeDamaged = false;
    }
    #endregion
}
