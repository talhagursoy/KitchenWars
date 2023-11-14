using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerReputationManager : NetworkBehaviour{

    [SerializeField] private Transform container;
    [SerializeField] private Transform reputationTemplate;
    private List<PlayerReputation> reputationList = new List<PlayerReputation>();


    [SerializeField] private float baseReputationMultiplier;
    private float currentReputationMultiplier = 1f;


    [SerializeField] private float baseReputationValueGain;
    [SerializeField] private float baseReputationValueDrain;

    [SerializeField] private float reputationMultiplierUpdateIntervalNormalized;

    private float lastMultiplierUpdateTimeNormalized = 0f;

    private KitchenGameManager kitchenGameManager;

    private void Awake() {
        reputationTemplate.gameObject.SetActive(false);
    }

    private void Start() {
        kitchenGameManager = KitchenGameManager.Instance;
        if (IsServer) {
            CreateReputationPrefabServerRpc();
            DeliveryManager.Instance.OnRecipeSuccess += DeliveryManagerInstance_OnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailed += DeliveryManagerInstance_OnRecipeFailed;
        }   
    }

    private void Update() {
        if (IsServer && kitchenGameManager.IsGamePlaying()) {
            float currentTimeNormalized = kitchenGameManager.GetGamePlayingTimerNormalized();
            if (currentTimeNormalized - lastMultiplierUpdateTimeNormalized >= reputationMultiplierUpdateIntervalNormalized) {
                lastMultiplierUpdateTimeNormalized = currentTimeNormalized;
                currentReputationMultiplier += baseReputationMultiplier;
                UpdateReputationMultiplierClientRpc(currentReputationMultiplier);
            }
        }
    }

    [ClientRpc]
    private void UpdateReputationMultiplierClientRpc(float reputationMultiplier) {
        this.currentReputationMultiplier = reputationMultiplier;
    }

    private void DeliveryManagerInstance_OnRecipeFailed(object sender, DeliveryManager.OnrecipeFailedEventArgs e) {
        ChangeReputationValueServerRpc(baseReputationValueDrain * currentReputationMultiplier, e.clientId);
    }
    private void DeliveryManagerInstance_OnRecipeSuccess(object sender, DeliveryManager.OnrecipeSuccessEventArgs e) {
        ChangeReputationValueServerRpc(baseReputationValueGain * currentReputationMultiplier, e.clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeReputationValueServerRpc(float value, ulong clientID) {
        ChangeReputationValueClientRpc(value, clientID);
    }

    [ClientRpc]
    private void ChangeReputationValueClientRpc(float value, ulong clientID) {
        foreach (PlayerReputation reputation in reputationList) {
            if (reputation.GetClientID() == clientID) {
                reputation.ChangeReputationValue(value);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateReputationPrefabServerRpc() {
        CreateReputationPrefabClientRpc();
    }

    [ClientRpc]
    private void CreateReputationPrefabClientRpc() {
        foreach (Transform child in container) {
            if (child == reputationTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (PlayerData playerData in KitchenGameMultiplayer.Instance.GetPlayerDataNetworkList()) {
            PlayerReputation playerReputation = Instantiate(reputationTemplate, container).GetComponent<PlayerReputation>();
            playerReputation.gameObject.SetActive(true);
            playerReputation.SetClientID(playerData.clientId);
            playerReputation.SetColorID(playerData.colorId);
            reputationList.Add(playerReputation);
        }
    }

}
