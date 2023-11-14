using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeliveryCounter : BaseCounter, INetworkOwnable {

    private ulong clientId= ulong.MaxValue;

    private void Start() {
        if(!IsServer) return;
    }

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {

                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject, clientId);

                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }

    public ulong GetClientId() {
        return clientId;
    }

    public void SetClientId(ulong clientId) {
        this.clientId = clientId;
    }

    public bool isIdOwner() {
        return clientId == NetworkManager.LocalClientId;
    }
}


