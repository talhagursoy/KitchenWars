using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter {

    [SerializeField] private List<KitchenObjectSO> kitchenObjectSOList;


    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            int randomIndex = UnityEngine.Random.Range(0, kitchenObjectSOList.Count);
            KitchenObject.SpawnKitchenObject(kitchenObjectSOList[randomIndex], player);
        }
    }

}