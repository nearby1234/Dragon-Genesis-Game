using System.Runtime.CompilerServices;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private DataBullTankBoss dataBullTank;
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.SEND_DATAHEAL_VALUE, ReceiverEventData);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.SEND_DATAHEAL_VALUE, ReceiverEventData);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(UIManager.HasInstance)
            {
                FakeLoadingSetting fakeLoading = new();
                UIManager.Instance.ShowPopup<PopupFakeLoading>(fakeLoading,true);
                if(Cheat.HasInstance)
                {
                    Cheat.Instance.TeleportPlayer();
                }
                if(GameManager.HasInstance)
                {
                    GameManager.Instance.SetCreepType(CreepType.BullTank);
                }
                if (ListenerManager.HasInstance)
                {
                    ListenerManager.Instance.BroadCast(ListenType.BOSSTYPE_SEND_HEAL_VALUE, dataBullTank);
                }
            }
        }
    }

    private void ReceiverEventData(object value)
    {
        if (value != null)
        {
            if (value is DataBullTankBoss data)
            {
                dataBullTank = data;
            }
        }
    }
}
