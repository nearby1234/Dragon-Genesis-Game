using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private GameObject m_ParentWeapon;
    [SerializeField] private QuestItemSO m_CurrentItem;
    [SerializeField] private GameObject m_WeaponPrefabs;
    [SerializeField] private Config config;
    [ShowInInspector]
    private Dictionary<string, GameObject> m_Items = new();

}
