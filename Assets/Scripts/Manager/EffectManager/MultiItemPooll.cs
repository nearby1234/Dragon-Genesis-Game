using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;

public class MultiItemPooll : MonoBehaviour
{
    [SerializeField] private float scatterRadius; // Distance from the center of the explosion to spawn orbs
    [SerializeField] private float scatterDuration; // Duration of the scatter effect
    [SerializeField] private Transform itemParents;

    [SerializeField] private List<BaseEnemyItem> itemPools;
    [ShowInInspector, ReadOnly]
    private List<PoolDebugView> DebugPools
    {
        get
        {
            return poolDict.Select(pair => new PoolDebugView
            {
                itemID = pair.Key,
                Count = pair.Value.Count,
                objects = pair.Value.ToList() // Nếu muốn debug từng object thì bật lên
            }).ToList();
        }
    }
    [ShowInInspector]
    private readonly Dictionary<string, Queue<GameObject>> poolDict = new();
    [ShowInInspector]
    private readonly Dictionary<string, GameObject> prefabDict = new();

    private void Start()
    {
       if(ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.SEND_ITEM_INIT_POOL, OnEventSendItemInitPool);
        }
        StartCoroutine(DelayItemPool());
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SEND_ITEM_INIT_POOL, OnEventSendItemInitPool);
        }
    }
    private void InitItem()
    {
        Debug.Log("InitItem poool");
        foreach (var item in itemPools)
        {
            Queue<GameObject> newPool = new();
            for (int i = 0; i < item.InitialSize; i++)
            {
                GameObject obj = Instantiate(item.QuestItemEffect.ItemEffectPrefabs, itemParents);
                obj.SetActive(false);
                newPool.Enqueue(obj);
            }

            poolDict[item.ItemId] = newPool;
            prefabDict[item.ItemId] = item.Prefabs;
        }
    }
    public GameObject Get(string itemID)
    {
        if (!poolDict.ContainsKey(itemID))
        {
            Debug.LogWarning($"No pool for item type: {itemID}");
            return null;
        }

        if (poolDict[itemID].Count > 0)
        {
            GameObject obj = poolDict[itemID].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefabDict[itemID], transform);
            return obj;
        }
    }
    public void Return(string itemId, GameObject obj)
    {
        obj.SetActive(false);
        poolDict[itemId].Enqueue(obj);
    }
    private void OnEventSendItemInitPool(object value)
    {
        if(value is BaseEnemyItem baseEnemyItem)
        {
            itemPools.Add(baseEnemyItem);
            //InitItem();
        }
    }
    public void SpawnItems(string itemID,Vector3 position)
    {
        GameObject obj = Get(itemID);
        if(obj != null)
        {
            obj.transform.position = position;
            if(obj.TryGetComponent(out ParticleSystem particleSystem))
            {
                particleSystem.Play();
            }
            TweenItem(obj, position);
        }else
        {
            Debug.LogWarning("không có obj item");
        }
    }
    private void TweenItem(GameObject orb, Vector3 position)
    {
        // Tạo vị trí XZ ngẫu nhiên
        Vector2 randomXZ = Random.insideUnitCircle * scatterRadius;
        Vector3 scatterTarget = new(position.x + randomXZ.x, 0.1f, position.z + randomXZ.y);

        float height = Random.Range(3f, 5f); // Độ cao văng lên
        float halfDuration = scatterDuration*0.5f;

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
    IEnumerator DelayItemPool()
    {
        yield return null;
        InitItem();
    }
}

[System.Serializable]
public class PoolDebugView
{
    public string itemID;

    [ShowInInspector, ReadOnly]
    public int Count;

    // Optional: hiển thị chi tiết các object nếu cần
    [ShowInInspector, ReadOnly]
    public List<GameObject> objects = new();
}
