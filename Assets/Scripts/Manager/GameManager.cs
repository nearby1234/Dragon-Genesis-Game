using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : BaseManager<GameManager>
{
    [SerializeField] private GameObject m_Player;

    [SerializeField] private List<GameObject> npcList = new();



    protected override void Awake()
    {
        base.Awake();
        GetChildNPC();
    }
    private void GetChildNPC()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject npc in npcs)
        {
            npcList.Add(npc);
        }
    }
    public GameObject GetNPC(string npcName)
    {
        foreach (GameObject npc in npcList)
        {
            if (npc.name == npcName)
            {
                return npc;
            }
        }
        return null;
    }
}
