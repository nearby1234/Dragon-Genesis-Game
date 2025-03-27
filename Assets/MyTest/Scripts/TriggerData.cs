using UnityEngine;

public class TriggerData
{
    public Vector3 ChildPos;
    public Collider Collider;

    public TriggerData(Vector3 pos, Collider col)
    {
        ChildPos = pos;
        Collider = col;
    }
}
