using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : BaseManager<PoolManager>
{
    [ShowInInspector]
    private readonly List<EnemyStatSO> enemyList = new();
    [ShowInInspector]
    private readonly Dictionary<CreepType, int> enemyCountDict = new();
    [SerializeField] private MultiItemPooll multiItemPool;
    public MultiItemPooll MultiItemPooll => multiItemPool;

    protected override void Awake()
    {
        base.Awake();
        multiItemPool = GetComponent<MultiItemPooll>();
    }
    private void Start()
    {
        InitDictEnemy();
    }
    private void InitDictEnemy()
    {
        int countEnemy = Enum.GetValues(typeof(CreepType)).Length;
        for (int i = 1; i < countEnemy; i++)
        {
            CreepType creepType = (CreepType)i;
            if (!enemyCountDict.ContainsKey(creepType))
            {
                enemyCountDict.Add(creepType, 0);
            }
        }
    }
    public void AddAmoutEnemyDeath(CreepType creepType)
    {
        if (enemyCountDict.ContainsKey(creepType))
        {
            enemyCountDict[creepType] += 1;
        }
    }

    public int GetCountEnemyDeath(CreepType creepType)
    {
        if (enemyCountDict.ContainsKey(creepType))
        {
            int amout = enemyCountDict[creepType];
            return amout;
        }
        return 0;
    }
    public bool CheckCountEnemyDeath(CreepType creepType)
    {
        int amount = GetCountEnemyDeath(creepType);
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].creepType == creepType)
            {
                if (amount.Equals(enemyList[i].amountEnemyDeath))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
