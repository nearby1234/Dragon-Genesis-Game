using System.Collections.Generic;
using UnityEngine;

public class EffectManager : BaseManager<EffectManager>
{
    [SerializeField] private ExpOrbEffectSpawner expOrbEffectSpawner;
    public ExpOrbEffectSpawner ExpOrbEffectSpawner => expOrbEffectSpawner;
    [SerializeField] private List<GameObject> effects = new();
    private const string path = "Prefabs/Effect";

    protected override void Awake()
    {
        base.Awake();
        GameObject[] prefabs = Resources.LoadAll<GameObject>(path);
        foreach (GameObject pref in prefabs)
        {
            effects.Add(pref);
        }
        expOrbEffectSpawner = GetComponent<ExpOrbEffectSpawner>();
        //multiItemPooll = GetComponent<MultiItemPooll>();
    }

    public GameObject GetPrefabs(string name)
    {
        foreach (GameObject pref in effects)
        {
            if (pref.name.Equals(name))
            {
                return pref;
            }
        }
        // Chỉ in thông báo lỗi sau khi không tìm thấy bất kỳ prefab nào khớp với tên
        Debug.Log($"{name} does not exist");
        return null;
    }
}
