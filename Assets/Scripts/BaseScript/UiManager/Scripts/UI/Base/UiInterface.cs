using UnityEngine;

public enum StateUi
{
    defaulted = 0,
    Opening,
    closing ,
}
public interface IStateUi
{
    StateUi StateUi { get;  }
    void SetStateUi(StateUi value);
    StateUi GetStateUi();
}
