using System.Collections;
using UnityEngine;

public class SlashDemo : MonoBehaviour
{
    //public Transform weaponPivot;
    public Transform m_spawnEffect;
    public GameObject slashEffectPrefab;


    public void ShowSlashEffect(float timer)
    {
        slashEffectPrefab.SetActive(true);
        StartCoroutine(DelayHideEffect(timer));
    }

   private IEnumerator DelayHideEffect(float timer)
    {
        yield return new WaitForSeconds(timer);
        slashEffectPrefab.SetActive(false);
    }    

    
}
