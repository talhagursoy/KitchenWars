using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class KitchenCounterMultiplayerIdManager : MonoBehaviour
{
    [SerializeField] private List<KitchenCounterIdManager> kitchenCounterIdManagers;

    private void Start() {
        NetworkList<PlayerData> playerDataNetworkList = KitchenGameMultiplayer.Instance.GetPlayerDataNetworkList();
        foreach (PlayerData playerData in playerDataNetworkList) {
            foreach (KitchenCounterIdManager kitchenCounterIdManager in kitchenCounterIdManagers) {
                if (playerData.colorId == kitchenCounterIdManager.GetColorId()) {
                    kitchenCounterIdManager.SetClientId(playerData.clientId);
                }
            }
        }
    }
}
