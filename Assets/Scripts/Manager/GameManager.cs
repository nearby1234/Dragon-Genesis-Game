using Sirenix.OdinInspector.Editor.Drawers;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;


public enum GAMESTATE
{
    NONE,
    MENULOADING,
    START,
    PLAYING,
    PAUSE,
    GAMEOVER,
}
public class GameManager : BaseManager<GameManager>
{
    //[SerializeField] private GameObject m_Player;

    //[SerializeField] private List<GameObject> npcList = new();

    [SerializeField] private GAMESTATE m_GameState = GAMESTATE.NONE;
    public bool m_IsPlaying = false;
    public GAMESTATE GameState
    {
        get => m_GameState;
        set
        {
            m_GameState = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        //GetChildNPC();
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;   
#endif
    }
    private void Start()
    {
        m_GameState = GAMESTATE.MENULOADING;
    }
    //private void GetChildNPC()
    //{
    //    GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
    //    foreach (GameObject npc in npcs)
    //    {
    //        npcList.Add(npc);
    //    }
    //}
    //public GameObject GetNPC(string npcName)
    //{
    //    foreach (GameObject npc in npcList)
    //    {
    //        if (npc.name == npcName)
    //        {
    //            return npc;
    //        }
    //    }
    //    return null;
    //}
}
