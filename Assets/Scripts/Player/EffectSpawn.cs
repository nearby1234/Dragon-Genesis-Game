using System.Collections;
using System.Threading;
using UnityEngine;

public class EffectSpawn : MonoBehaviour
{
    [SerializeField] private Transform m_SpawnSlash;
    [SerializeField] private float m_Timer;
    public void Show()
    {
        GameObject slash = SlashPool.instance.GetSlash();

        if (slash == null)
        {
            Debug.Log("test");
        }
        slash.transform.position = m_SpawnSlash.position;
        slash.transform.rotation = m_SpawnSlash.rotation;
        StartCoroutine(Delay(slash));
    }
    IEnumerator Delay(GameObject slash)
    {
        yield return new WaitForSeconds(m_Timer);
        SlashPool.instance.ReturnPool(slash);
    }
    
    //public void SpawnBlood()
    //{
    //    m_SpawnBlood.gameObject.SetActive(true);
    //    StartCoroutine(DelayOffSpawnBlood());
    //}

    //IEnumerator DelayOffSpawnBlood()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    m_SpawnBlood.gameObject.SetActive(false);
    //}

}
