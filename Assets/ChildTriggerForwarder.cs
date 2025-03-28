using UnityEngine;

public class ChildTriggerForwarder : MonoBehaviour
{
   
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Boss")|| other.CompareTag("Creep"))
        {
            Vector3 childPos = transform.position;

            TriggerData triggerData = new(childPos, other);
            transform.root.SendMessage("OnChildTriggerEnter",triggerData,SendMessageOptions.DontRequireReceiver);
        }
    }
}
