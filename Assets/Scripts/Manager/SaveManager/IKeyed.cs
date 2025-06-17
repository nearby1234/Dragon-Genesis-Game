using UnityEngine;

public interface IKeyed<TKey>
{
    TKey Key { get; }
}
