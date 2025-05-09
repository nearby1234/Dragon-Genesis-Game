using UnityEngine;

public class ShockWave : MonoBehaviour
{
    [SerializeField]private  ParticleSystem shockWave;
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_PLAYAGAIN, ReceiverOnClickPlayAgain);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_PLAYAGAIN, ReceiverOnClickPlayAgain);
        }
    }
    public void PlayShockWave()
    {
      if(shockWave != null)
        {
            shockWave.gameObject.SetActive(true);
            if (!shockWave.isPlaying)
            {
                shockWave.Play();
            }
        }    
    }  
    
    public void StopShockWave()
    {
        if (shockWave != null)
        {
            shockWave.gameObject.SetActive(false);
            if (shockWave.isPlaying)
            {
                shockWave.Stop();
            }    
        }
    }    
    private void ReceiverOnClickPlayAgain(object value)
    {
        StopShockWave();
    }
}
