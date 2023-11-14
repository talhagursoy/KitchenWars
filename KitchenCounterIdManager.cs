using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class KitchenCounterIdManager : NetworkBehaviour
{
    [SerializeField] private int colorId;
    private NetworkVariable <ulong> clientId = new NetworkVariable<ulong>(100);
    [SerializeField] private List<BaseCounter> kitchenCounters;
    private void Awake() {
        clientId.OnValueChanged += ClientId_OnValueChanged;
    }

    private void ClientId_OnValueChanged(ulong previousValue, ulong newValue) {
        foreach (BaseCounter counter in kitchenCounters) { 
            counter.TryGetComponent<INetworkOwnable>(out INetworkOwnable ownable);
            if (ownable != null) { 
                ownable.SetClientId(newValue);
            }
        }
    }
    public int GetColorId() {
        return colorId;
    }
    public void SetClientId(ulong clientId) {
        this.clientId.Value = clientId;
    }
}
