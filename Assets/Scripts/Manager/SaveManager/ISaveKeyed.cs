using UnityEngine;

public interface ISaveKeyed<TKey>
{
    TKey Key { get; }
}
