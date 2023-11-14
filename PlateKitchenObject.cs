using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO kitchenObjectSO;
    }

    public event EventHandler OnPlateCleaned;

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;
    [SerializeField] private KitchenObjectSO dirtyPlateKitchenObjectSO;

    private List<KitchenObjectSO> kitchenObjectSOList;
    private Dictionary<string, bool> ingredientAddedDict = new Dictionary<string, bool>();

    protected override void Awake() {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
        InitializeIngredientAddedDict();
    }

    private void InitializeIngredientAddedDict() {
        foreach (KitchenObjectSO validIngredient in validKitchenObjectSOList) {
            ingredientAddedDict[validIngredient.objectName] = false;
        }
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) {
            return false;
        }

        string ingredientName = kitchenObjectSO.objectName;

        if (ingredientAddedDict.ContainsKey(ingredientName) && ingredientAddedDict[ingredientName]) {
            // Ingredient has already been added in some version fresh or expired
            return false;
        }
        else {
            AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
            return true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex) {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        kitchenObjectSOList.Add(kitchenObjectSO);
        ingredientAddedDict[kitchenObjectSO.objectName] = true;

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {
            kitchenObjectSO = kitchenObjectSO
        });
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return kitchenObjectSOList;
    }

    public bool IsPlateDirty() {
        return dirtyPlateKitchenObjectSO == kitchenObjectSO;
    }

    public bool IsEmpty() {
        return kitchenObjectSOList.Count == 0;
    }
}