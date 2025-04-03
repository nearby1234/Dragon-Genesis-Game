using System.Collections.Generic;
using UnityEngine;

public class EffectManager : BaseManager<EffectManager>
{
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
    }

    public GameObject GetPrefabs(string name)
    {
        foreach (GameObject pref in effects)
        {
            if (pref.name.Equals(name))
            {
                return pref;
            }else
            {
                Debug.Log($"{name} does not exist");
            }
        }
        return null;
    }
}
