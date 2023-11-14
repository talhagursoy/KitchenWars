using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent {

    public bool HasKitchenObject();

    public Transform GetKitchenObjectFollowTransform();

    public void SetKitchenObject(KitchenObject kitchenObject);

    public void ClearKitchenObject();

    public KitchenObject GetKitchenObject();

    public NetworkObject GetNetworkObject();

}