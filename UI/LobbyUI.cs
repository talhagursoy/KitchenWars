using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;

    private void Awake() {
        createLobbyButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.CreateLobby("lobby name", false);
        });
        joinLobbyButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.QuickJoin();
        });
    }
}
