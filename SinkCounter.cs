using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkCounter : BaseCounter{
    [SerializeField] private KitchenObjectSO cleanPlateKitchenObjectSO;
    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            return;
        }

        if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
            if (!plateKitchenObject.IsEmpty() || plateKitchenObject.IsPlateDirty()) {
                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                KitchenObject.SpawnKitchenObject(cleanPlateKitchenObjectSO, player);
            }
        }
    }
}
