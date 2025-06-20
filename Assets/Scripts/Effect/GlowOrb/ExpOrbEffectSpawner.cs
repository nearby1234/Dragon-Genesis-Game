using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpOrbEffectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int m_OrbCount;
    [SerializeField] private float scatterRadius; // Distance from the center of the explosion to spawn orbs
    [SerializeField] private float scatterDuration; // Duration of the scatter effect
    [SerializeField] private Transform m_UiVFXTransformParent;
    [SerializeField] private Queue<GameObject> orbPool = new(); // Pool of orbs
    [ShowInInspector, ReadOnly]
    private List<GameObject> OrbPoolList => new(orbPool);
    [SerializeField] private int m_PoolCount = 10; // Number of orbs in the pool
    private void Start()
    {
        explosionPrefab = EffectManager.Instance.GetPrefabs("GlowingOrb40(Dup)");
        PoolOrbEffect();
    }
    public void SpawnOrbs(Vector3 position, int count, GameObject obj = null)
    {
        if (obj == null)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject orb = GetOrbFromPool();
                if (orb == null) continue;
                orb.transform.position = position;

                TweenItem(orb, position);
            }
        }
    }
    private void TweenItem(GameObject orb, Vector3 position)
    {
        // Tạo vị trí XZ ngẫu nhiên
        Vector2 randomXZ = Random.insideUnitCircle * scatterRadius;
        Vector3 scatterTarget = new(position.x + randomXZ.x, 0.5f, position.z + randomXZ.y);

        float height = Random.Range(3f, 5f); // Độ cao văng lên
        float halfDuration = scatterDuration * 0.5f;

        // Giai đoạn 1: Văng lên theo Y
        orb.transform.DOMoveY(position.y + height, halfDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Giai đoạn 2: Rơi xuống vị trí ngẫu nhiên
                orb.transform.DOMove(scatterTarget, halfDuration)
                    .SetEase(Ease.InQuad);
            });
    }
    private void PoolOrbEffect()
    {
        for (int i = 0; i < m_PoolCount; i++)
        {
            GameObject orb = Instantiate(explosionPrefab, this.transform);
            orb.SetActive(false);
            orb.transform.SetParent(m_UiVFXTransformParent);
            orb.transform.position = Vector3.zero; // Đặt vị trí của orb về 0
            orbPool.Enqueue(orb);
        }
    }
    private GameObject GetOrbFromPool()
    {
        if (orbPool.Count > 0)
        {
            GameObject orb = orbPool.Dequeue();
            orb.SetActive(true);
            return orb;
        }
        else
        {
            Debug.Log("No orbs available in the pool.");
        }
        return null;
    }

    public void ReturnPoolOrbEffect(GameObject orb)
    {
        orbPool.Enqueue(orb);
        orb.SetActive(false);
        orb.transform.position = Vector3.zero; // Đặt vị trí của orb về 0
    }
}
