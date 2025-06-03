using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BullTankSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
  
    [ShowInInspector]
    private Dictionary<string, AudioClip> seDic = new();
    

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    
    }
    private void Start()
    {
        if (AudioManager.HasInstance)
        {
            seDic = AudioManager.Instance.SeDic;
        }
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.PLAYSOUNDSE_BOSSBULLTANK, ReceiverEventPlaySound);
        }
      


    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.PLAYSOUNDSE_BOSSBULLTANK, ReceiverEventPlaySound);
        }
        
    }

    public void PlaySoundFx(string nameSound)
    {
        if (seDic.ContainsKey(nameSound))
        {
            audioClip = seDic[nameSound];
            audioSource.clip = audioClip;
            audioSource.PlayOneShot(audioClip);
        }
    }

    
    private void ReceiverEventPlaySound(object value)
    {
        if(value != null)
        {
            if(value is string nameSound)
            {
                if (seDic.ContainsKey(nameSound))
                {
                    audioClip = seDic[nameSound];
                    audioSource.clip = audioClip;
                    audioSource.PlayOneShot(audioClip);
                }
            }    
        } 
    }    

   

}
