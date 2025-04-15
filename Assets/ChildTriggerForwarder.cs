using System.Collections;
using System.Collections.Generic;
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
    private List<Collider> damagedColliders = new();

    private void Awake()
    {
        pivotScaleWeapon = GetComponentInParent<PivotScaleWeapon>();
    }
    private void Start()
    {
        if (EffectManager.HasInstance)
        {
            damageTextPrefab = EffectManager.Instance.GetPrefabs("DamageText");
            explosionPrefab = EffectManager.Instance.GetPrefabs("Explosion");
        }
        //pivotScaleWeapon.GetChildTriggerForwarder(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapBox(GetComponent<Collider>().bounds.center,
                                                    GetComponent<Collider>().bounds.extents,
                                                    transform.rotation);
        foreach (Collider col in colliders)
        {
            if (col == GetComponent<Collider>())
                continue;

            // Kiểm tra nếu chưa xử lý damage cho collider này trong lần swing hiện tại
            if (!damagedColliders.Contains(col))
            {
                if (col.CompareTag("Boss"))
                {
                    ProcessBossTrigger(col);
                    damagedColliders.Add(col);
                }
                else if (col.CompareTag("Creep"))
                {
                    ProcessCreepTrigger(col);
                    damagedColliders.Add(col);
                }
            }
        }
    }

    // Sau khi kết thúc swing, reset danh sách
    public IEnumerator ResetSwing()
    {
        yield return new WaitForSeconds(1f);
        damagedColliders.Clear();
    }


    #region Trigger Processing

    private void ProcessBossTrigger(Collider other)
    {
        WormBoss wormBoss = GetComponentInParent<WormBoss>();
        if (wormBoss != null /*&& !isEnergyTakeDamaged*/)
        {
            wormBoss.GetDamage(pivotScaleWeapon.EnergyWeaponDamage);
            ExecuteCommonDamageEffects(other, instantiateExplosion: false);
        }
    }

    private void ProcessCreepTrigger(Collider other)
    {
        EnemyHeal enemyHeal = other.GetComponentInParent<EnemyHeal>();
        if (enemyHeal != null /*&& !isEnergyTakeDamaged*/)
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
        //i/*sEnergyTakeDamaged = true;*/

        // Calculate the closest point on the collider surface to set effect origin
        Vector3 closePoint = other.ClosestPoint(other.transform.position);

        // Instantiate and update the damage text effect.
        GameObject textDamage = Instantiate(damageTextPrefab, other.transform.position, other.transform.rotation);
        if (textDamage.TryGetComponent<SetupTextDamage>(out var setupTextDamage))
        {
            setupTextDamage.ChangeTextDamage(pivotScaleWeapon.EnergyWeaponDamage, closePoint);
        }

        //GameObject explosionInstance = Instantiate(explosionPrefab, other.transform.position, other.transform.rotation);
        GameObject explosionInstance = Instantiate(explosionPrefab,new Vector3( other.transform.position.x,0,other.transform.position.z), other.transform.rotation);
        PlayParticleEffect(explosionInstance, closePoint);

        //StartCoroutine(ResetEnergyDamageFlag());
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
