using System;
using UnityEngine;

public interface IEnumKeyed<TEnum> where TEnum : Enum
{
    TEnum Key {  get; }
}
