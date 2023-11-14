using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetworkOwnable
{
    public ulong GetClientId();
    public bool isIdOwner();
    public void SetClientId(ulong clientId);
}
