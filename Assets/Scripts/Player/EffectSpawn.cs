using System.Collections;
using UnityEngine;

public class EffectSpawn : MonoBehaviour
{
    [SerializeField] private Transform m_SpawnSlash;
    [SerializeField] private float m_Timer;
    public void Show()
    {
        GameObject slash = SlashPool.instance.GetSlash();

        if (slash != null)
        {
            slash.transform.position = m_SpawnSlash.position;
            slash.transform.rotation = m_SpawnSlash.rotation;
            StartCoroutine(Delay(slash));
        }
    }
    IEnumerator Delay(GameObject slash)
    {
        yield return new WaitForSeconds(m_Timer);
        SlashPool.instance.ReturnPool(slash);
    }
}
