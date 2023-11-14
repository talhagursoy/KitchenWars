using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter {
    public event EventHandler<OnPlateSpawnedEventArgs> OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    public class OnPlateSpawnedEventArgs : EventArgs {
        public KitchenObjectSO plateSO;
    }

    [SerializeField] private KitchenObjectSO cleanPlateKitchenObjectSO;
    [SerializeField] private KitchenObjectSO dirtyPlateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 5f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 5;
    private List<KitchenObjectSO> spawnedPlatesList;

    private void Awake() {
        spawnedPlatesList = new List<KitchenObjectSO>();
    }
    private void Update() {
        if (!IsServer) {
            return;
        }

        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax) {
            spawnPlateTimer = 0f;

            if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax) {
                SpawnRandomPlateServerRpc();
            }
        }
    }

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            if (platesSpawnedAmount > 0) {
                KitchenObjectSO plateSO = spawnedPlatesList[spawnedPlatesList.Count - 1];
                KitchenObject.SpawnKitchenObject(plateSO, player);
                spawnedPlatesList.RemoveAt(spawnedPlatesList.Count - 1);

                InteractLogicServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnRandomPlateServerRpc() {
        KitchenObjectSO plateSO = UnityEngine.Random.Range(0f, 1f) < 0.5f ? cleanPlateKitchenObjectSO : dirtyPlateKitchenObjectSO;
        SpawnPlateClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(plateSO));
    }

    [ClientRpc]
    private void SpawnPlateClientRpc(int kitchenObjectSOIndex) {
        spawnedPlatesList.Add(KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex));
        platesSpawnedAmount++;
        OnPlateSpawned?.Invoke(this, new OnPlateSpawnedEventArgs {
            plateSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex)
            
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        platesSpawnedAmount--;

        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }

    public KitchenObjectSO GetCleanPlateKitchenObjectSO() {
        return cleanPlateKitchenObjectSO;
    }
}