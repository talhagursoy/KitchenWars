using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour {


    public event EventHandler OnRecipeSpawned;

    public event EventHandler OnRecipeCompleted;

    public event EventHandler<OnrecipeSuccessEventArgs> OnRecipeSuccess;
    public class OnrecipeSuccessEventArgs : EventArgs {
        public ulong clientId;
    }
    public event EventHandler<OnrecipeFailedEventArgs> OnRecipeFailed;

    public class OnrecipeFailedEventArgs : EventArgs {
        public ulong clientId;
    }

    public static DeliveryManager Instance { get; private set; }


    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer = 2f;
    private float spawnRecipeTimerMax = 5f;
    private int waitingRecipesMax = 4;
    private int successfulRecipesAmount;


    private void Awake() {
        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update() {

        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax) {
        
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject, ulong clientId) {
        if (plateKitchenObject.IsPlateDirty() && clientId!=ulong.MaxValue) {
            DeliverIncorrectRecipeServerRpc(clientId);
            return;
        }
        List<int> plateItemIndexList = new List<int>();

        foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
            plateItemIndexList.Add(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
        }

        int[] plateItemIndexArray = plateItemIndexList.ToArray();
        CheckRecipeServerRpc(plateItemIndexArray, clientId);

    }
    [ServerRpc(RequireOwnership = false)]
    private void CheckRecipeServerRpc(int[] plateItemIndexList, ulong clientId){
        ClientRpcParams clientRpcParams = new ClientRpcParams{
            Send = new ClientRpcSendParams{
                TargetClientIds = new ulong[]{clientId}
            }
        };
        CheckRecipeClientRpc(plateItemIndexList, clientId, clientRpcParams);
    }

    [ClientRpc]
    private void CheckRecipeClientRpc(int[] plateItemIndexList,  ulong clientId, ClientRpcParams clientRpcParams = default){
        List<KitchenObjectSO> plateKitchenObjectSOList = new List<KitchenObjectSO>();

        foreach (int plateItemIndex in plateItemIndexList){
            plateKitchenObjectSOList.Add(KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(plateItemIndex));
        }


        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObjectSOList.Count) {
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObjectSOList) {
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound) {
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe) {
                    successfulRecipesAmount++;

                    waitingRecipeSOList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    DeliverCorrectRecipeServerRpc(clientId);
                    return;
                }
            }
        }
        DeliverIncorrectRecipeServerRpc(clientId);
    }


    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc(ulong clientId) {
        DeliverIncorrectRecipeClientRpc(clientId);
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc(ulong clientId) {
        OnRecipeFailed?.Invoke(this, new OnrecipeFailedEventArgs{
            clientId = clientId
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(ulong clientId) {
        DeliverCorrectRecipeClientRpc(clientId);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(ulong clientId) {
        OnRecipeSuccess?.Invoke(this, new OnrecipeSuccessEventArgs {
            clientId = clientId,
        });
    }

    public void RecipeTimeOut(RecipeSO recipeSO) {
        for(int i=0; i < waitingRecipeSOList.Count; i++) {
            if (waitingRecipeSOList[i] == recipeSO) {
                waitingRecipeSOList.RemoveAt(i);
                i = waitingRecipeSOList.Count;
            }
        }
        DeliverIncorrectRecipeServerRpc(NetworkManager.LocalClientId);
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount() {
        return successfulRecipesAmount;
    }

}
